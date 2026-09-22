using Microsoft.Data.SqlClient;
using MiniMartManagement.Database;
using MiniMartManagement.Interfaces;
using MiniMartManagement.Models;

namespace MiniMartManagement.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public ProductRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private const string BaseSelect = @"
            SELECT product_id, category_id, product_code, product_name,
                   purchase_price, selling_price, unit, min_stock, is_active, created_at, image_path
            FROM Products";

        public List<Product> GetAll()
        {
            var products = new List<Product>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(BaseSelect + " ORDER BY product_name", connection);

            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                products.Add(MapProduct(reader));
            }

            return products;
        }

        public Product? GetById(int productId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(BaseSelect + " WHERE product_id = @id", connection);
            command.Parameters.AddWithValue("@id", productId);

            connection.Open();
            using var reader = command.ExecuteReader();
            return reader.Read() ? MapProduct(reader) : null;
        }

        public Product? GetByCode(string productCode)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(BaseSelect + " WHERE product_code = @code", connection);
            command.Parameters.AddWithValue("@code", productCode);

            connection.Open();
            using var reader = command.ExecuteReader();
            return reader.Read() ? MapProduct(reader) : null;
        }

        public List<Product> Search(string keyword)
        {
            var products = new List<Product>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                BaseSelect + " WHERE product_name LIKE @keyword OR product_code LIKE @keyword ORDER BY product_name",
                connection);
            command.Parameters.AddWithValue("@keyword", $"%{keyword}%");

            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                products.Add(MapProduct(reader));
            }

            return products;
        }

        public void Add(Product product)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(@"
                INSERT INTO Products
                    (category_id, product_code, product_name, purchase_price, selling_price, unit, min_stock, is_active, created_at, image_path)
                OUTPUT INSERTED.product_id
                VALUES
                    (@categoryId, @code, @name, @purchasePrice, @sellingPrice, @unit, @minStock, @isActive, @createdAt, @imagePath)",
                connection);

            AddProductParameters(command, product);
            command.Parameters.AddWithValue("@createdAt", product.CreatedAt);

            connection.Open();
            product.Id = (int)command.ExecuteScalar();
        }

        public void Update(Product product)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(@"
                UPDATE Products
                SET category_id = @categoryId, product_name = @name, purchase_price = @purchasePrice,
                    selling_price = @sellingPrice, unit = @unit, min_stock = @minStock, image_path = @imagePath
                WHERE product_id = @id", connection);

            command.Parameters.AddWithValue("@categoryId", product.CategoryId);
            command.Parameters.AddWithValue("@name", product.Name);
            command.Parameters.AddWithValue("@purchasePrice", product.PurchasePrice);
            command.Parameters.AddWithValue("@sellingPrice", product.SellingPrice);
            command.Parameters.AddWithValue("@unit", product.Unit);
            command.Parameters.AddWithValue("@minStock", product.MinStock);
            command.Parameters.AddWithValue("@imagePath", (object?)product.ImagePath ?? DBNull.Value);
            command.Parameters.AddWithValue("@id", product.Id);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void SetActive(int productId, bool isActive)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "UPDATE Products SET is_active = @isActive WHERE product_id = @id", connection);
            command.Parameters.AddWithValue("@isActive", isActive);
            command.Parameters.AddWithValue("@id", productId);

            connection.Open();
            command.ExecuteNonQuery();
        }

        private static void AddProductParameters(SqlCommand command, Product product)
        {
            command.Parameters.AddWithValue("@categoryId", product.CategoryId);
            command.Parameters.AddWithValue("@code", product.ProductCode);
            command.Parameters.AddWithValue("@name", product.Name);
            command.Parameters.AddWithValue("@purchasePrice", product.PurchasePrice);
            command.Parameters.AddWithValue("@sellingPrice", product.SellingPrice);
            command.Parameters.AddWithValue("@unit", product.Unit);
            command.Parameters.AddWithValue("@minStock", product.MinStock);
            command.Parameters.AddWithValue("@isActive", product.IsActive);
            command.Parameters.AddWithValue("@imagePath", (object?)product.ImagePath ?? DBNull.Value);
        }

        private static Product MapProduct(SqlDataReader reader)
        {
            int imagePathOrdinal = reader.GetOrdinal("image_path");
            var product = new Product(
                categoryId: reader.GetInt32(reader.GetOrdinal("category_id")),
                productCode: reader.GetString(reader.GetOrdinal("product_code")),
                name: reader.GetString(reader.GetOrdinal("product_name")),
                purchasePrice: reader.GetDecimal(reader.GetOrdinal("purchase_price")),
                sellingPrice: reader.GetDecimal(reader.GetOrdinal("selling_price")),
                unit: reader.GetString(reader.GetOrdinal("unit")),
                minStock: reader.GetInt32(reader.GetOrdinal("min_stock")),
                isActive: reader.GetBoolean(reader.GetOrdinal("is_active")),
                createdAt: reader.GetDateTime(reader.GetOrdinal("created_at")),
                imagePath: reader.IsDBNull(imagePathOrdinal) ? null : reader.GetString(imagePathOrdinal));

            product.Id = reader.GetInt32(reader.GetOrdinal("product_id"));
            return product;
        }
    }
}
