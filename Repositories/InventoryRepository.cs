using Microsoft.Data.SqlClient;
using MiniMartManagement.Database;
using MiniMartManagement.Interfaces;
using MiniMartManagement.Models;

namespace MiniMartManagement.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public InventoryRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private const string BaseSelect =
            "SELECT inventory_id, product_id, quantity, last_updated FROM Inventory";

        public Inventory? GetByProductId(int productId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(BaseSelect + " WHERE product_id = @productId", connection);
            command.Parameters.AddWithValue("@productId", productId);

            connection.Open();
            using var reader = command.ExecuteReader();
            return reader.Read() ? MapInventory(reader) : null;
        }

        public List<Inventory> GetAll()
        {
            var results = new List<Inventory>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(BaseSelect, connection);

            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                results.Add(MapInventory(reader));
            }

            return results;
        }

        public List<Inventory> GetLowStock()
        {
            var results = new List<Inventory>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(@"
                SELECT i.inventory_id, i.product_id, i.quantity, i.last_updated
                FROM Inventory i
                JOIN Products p ON p.product_id = i.product_id
                WHERE i.quantity > 0 AND i.quantity <= p.min_stock", connection);

            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                results.Add(MapInventory(reader));
            }

            return results;
        }

        public List<Inventory> GetOutOfStock()
        {
            var results = new List<Inventory>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(BaseSelect + " WHERE quantity <= 0", connection);

            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                results.Add(MapInventory(reader));
            }

            return results;
        }

        public void Add(Inventory inventory)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(@"
                INSERT INTO Inventory (product_id, quantity, last_updated)
                OUTPUT INSERTED.inventory_id
                VALUES (@productId, @quantity, @lastUpdated)", connection);

            command.Parameters.AddWithValue("@productId", inventory.ProductId);
            command.Parameters.AddWithValue("@quantity", inventory.Quantity);
            command.Parameters.AddWithValue("@lastUpdated", inventory.LastUpdated);

            connection.Open();
            inventory.Id = (int)command.ExecuteScalar();
        }

        public void AdjustQuantity(int productId, int newQuantity)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(@"
                UPDATE Inventory
                SET quantity = @quantity, last_updated = @lastUpdated
                WHERE product_id = @productId", connection);

            command.Parameters.AddWithValue("@quantity", newQuantity);
            command.Parameters.AddWithValue("@lastUpdated", DateTime.Now);
            command.Parameters.AddWithValue("@productId", productId);

            connection.Open();
            command.ExecuteNonQuery();
        }

        private static Inventory MapInventory(SqlDataReader reader)
        {
            var inventory = new Inventory(
                productId: reader.GetInt32(reader.GetOrdinal("product_id")),
                quantity: reader.GetInt32(reader.GetOrdinal("quantity")),
                lastUpdated: reader.GetDateTime(reader.GetOrdinal("last_updated")));

            inventory.Id = reader.GetInt32(reader.GetOrdinal("inventory_id"));
            return inventory;
        }
    }
}
