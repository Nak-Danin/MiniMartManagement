using Microsoft.Data.SqlClient;
using MiniMartManagement.Database;
using MiniMartManagement.Interfaces;
using MiniMartManagement.Models;

namespace MiniMartManagement.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public SaleRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Inserts the Sale, its SaleItems, and decreases Inventory for each
        /// item - all inside one transaction. The inventory UPDATE includes
        /// "AND quantity >= @qty" in its WHERE clause, so the stock check and
        /// the decrement happen as a single atomic operation: if two people
        /// somehow completed a sale for the last item at the same moment,
        /// the second one fails safely here instead of allowing negative stock.
        /// </summary>
        public void CompleteSale(Sale sale)
        {
            if (sale.Items.Count == 0)
            {
                throw new InvalidOperationException("A sale must contain at least one item.");
            }

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                using var insertSale = new SqlCommand(@"
                    INSERT INTO Sales (employee_id, sale_date, subtotal, discount, total, payment, change_amount)
                    OUTPUT INSERTED.sale_id
                    VALUES (@employeeId, @saleDate, @subtotal, @discount, @total, @payment, @changeAmount)",
                    connection, transaction);

                insertSale.Parameters.AddWithValue("@employeeId", sale.EmployeeId);
                insertSale.Parameters.AddWithValue("@saleDate", sale.SaleDate);
                insertSale.Parameters.AddWithValue("@subtotal", sale.Subtotal);
                insertSale.Parameters.AddWithValue("@discount", sale.Discount);
                insertSale.Parameters.AddWithValue("@total", sale.Total);
                insertSale.Parameters.AddWithValue("@payment", sale.Payment);
                insertSale.Parameters.AddWithValue("@changeAmount", sale.ChangeAmount);

                int newSaleId = (int)insertSale.ExecuteScalar();

                foreach (var item in sale.Items)
                {
                    using var insertItem = new SqlCommand(@"
                        INSERT INTO SaleItems (sale_id, product_id, quantity, unit_price, subtotal)
                        VALUES (@saleId, @productId, @quantity, @unitPrice, @subtotal)",
                        connection, transaction);

                    insertItem.Parameters.AddWithValue("@saleId", newSaleId);
                    insertItem.Parameters.AddWithValue("@productId", item.ProductId);
                    insertItem.Parameters.AddWithValue("@quantity", item.Quantity);
                    insertItem.Parameters.AddWithValue("@unitPrice", item.UnitPrice);
                    insertItem.Parameters.AddWithValue("@subtotal", item.Subtotal);
                    insertItem.ExecuteNonQuery();

                    using var decreaseStock = new SqlCommand(@"
                        UPDATE Inventory
                        SET quantity = quantity - @quantity, last_updated = @now
                        WHERE product_id = @productId AND quantity >= @quantity",
                        connection, transaction);

                    decreaseStock.Parameters.AddWithValue("@quantity", item.Quantity);
                    decreaseStock.Parameters.AddWithValue("@now", DateTime.Now);
                    decreaseStock.Parameters.AddWithValue("@productId", item.ProductId);

                    int rowsAffected = decreaseStock.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        throw new InvalidOperationException(
                            $"Insufficient stock for product ID {item.ProductId}. Sale cancelled.");
                    }
                }

                transaction.Commit();
                sale.Id = newSaleId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public Sale? GetById(int saleId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var saleCommand = new SqlCommand(@"
                SELECT sale_id, employee_id, sale_date, subtotal, discount, total, payment, change_amount
                FROM Sales WHERE sale_id = @saleId", connection);
            saleCommand.Parameters.AddWithValue("@saleId", saleId);

            connection.Open();

            Sale? sale;
            using (var reader = saleCommand.ExecuteReader())
            {
                if (!reader.Read())
                {
                    return null;
                }
                sale = MapSaleHeader(reader);
            }

            LoadItemsInto(sale, connection);
            return sale;
        }

        public List<Sale> GetByEmployeeId(int employeeId)
        {
            return LoadSales(
                "WHERE employee_id = @employeeId ORDER BY sale_date DESC",
                command => command.Parameters.AddWithValue("@employeeId", employeeId));
        }

        public List<Sale> GetByDateRange(DateTime from, DateTime to)
        {
            return LoadSales(
                "WHERE sale_date >= @from AND sale_date < @to ORDER BY sale_date DESC",
                command =>
                {
                    command.Parameters.AddWithValue("@from", from);
                    command.Parameters.AddWithValue("@to", to);
                });
        }

        public List<Sale> GetAll()
        {
            return LoadSales("ORDER BY sale_date DESC", _ => { });
        }

        private List<Sale> LoadSales(string whereAndOrderClause, Action<SqlCommand> configureParameters)
        {
            var sales = new List<Sale>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(@$"
                SELECT sale_id, employee_id, sale_date, subtotal, discount, total, payment, change_amount
                FROM Sales
                {whereAndOrderClause}", connection);
            configureParameters(command);

            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    sales.Add(MapSaleHeader(reader));
                }
            }

            // Load line items for each sale header (simple N+1 approach -
            // fine at MiniMart's scale, easy to explain in a report).
            foreach (var sale in sales)
            {
                LoadItemsInto(sale, connection);
            }

            return sales;
        }

        private static void LoadItemsInto(Sale sale, SqlConnection connection)
        {
            using var itemsCommand = new SqlCommand(@"
                SELECT sale_item_id, sale_id, product_id, quantity, unit_price, subtotal
                FROM SaleItems WHERE sale_id = @saleId", connection);
            itemsCommand.Parameters.AddWithValue("@saleId", sale.Id);

            using var reader = itemsCommand.ExecuteReader();
            while (reader.Read())
            {
                var item = new SaleItem(
                    productId: reader.GetInt32(reader.GetOrdinal("product_id")),
                    quantity: reader.GetInt32(reader.GetOrdinal("quantity")),
                    unitPrice: reader.GetDecimal(reader.GetOrdinal("unit_price")));

                item.Id = reader.GetInt32(reader.GetOrdinal("sale_item_id"));
                item.SaleId = reader.GetInt32(reader.GetOrdinal("sale_id"));
                sale.AddItem(item);
            }
        }

        private static Sale MapSaleHeader(SqlDataReader reader)
        {
            var sale = new Sale(
                employeeId: reader.GetInt32(reader.GetOrdinal("employee_id")),
                saleDate: reader.GetDateTime(reader.GetOrdinal("sale_date")));

            sale.Id = reader.GetInt32(reader.GetOrdinal("sale_id"));

            decimal discount = reader.GetDecimal(reader.GetOrdinal("discount"));
            decimal payment = reader.GetDecimal(reader.GetOrdinal("payment"));

            // Items are loaded separately afterwards (LoadItemsInto). Discount/Payment
            // are restored directly here rather than through ApplyDiscount/SetPayment,
            // since those validate against Subtotal which depends on items not yet loaded.
            sale.RestoreFinancials(discount, payment);

            return sale;
        }
    }
}
