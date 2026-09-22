using Microsoft.Data.SqlClient;
using MiniMartManagement.Database;
using MiniMartManagement.Interfaces;
using MiniMartManagement.Models;

namespace MiniMartManagement.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public CategoryRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public List<Category> GetAll()
        {
            var categories = new List<Category>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "SELECT category_id, category_name, description, is_active FROM Categories ORDER BY category_name",
                connection);

            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                categories.Add(MapCategory(reader));
            }

            return categories;
        }

        public Category? GetById(int categoryId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "SELECT category_id, category_name, description, is_active FROM Categories WHERE category_id = @id",
                connection);
            command.Parameters.AddWithValue("@id", categoryId);

            connection.Open();
            using var reader = command.ExecuteReader();
            return reader.Read() ? MapCategory(reader) : null;
        }

        public void Add(Category category)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(@"
                INSERT INTO Categories (category_name, description, is_active)
                OUTPUT INSERTED.category_id
                VALUES (@name, @description, @isActive)", connection);

            command.Parameters.AddWithValue("@name", category.Name);
            command.Parameters.AddWithValue("@description", (object?)category.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@isActive", category.IsActive);

            connection.Open();
            category.Id = (int)command.ExecuteScalar();
        }

        public void Update(Category category)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(@"
                UPDATE Categories
                SET category_name = @name, description = @description
                WHERE category_id = @id", connection);

            command.Parameters.AddWithValue("@name", category.Name);
            command.Parameters.AddWithValue("@description", (object?)category.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@id", category.Id);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void SetActive(int categoryId, bool isActive)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "UPDATE Categories SET is_active = @isActive WHERE category_id = @id", connection);
            command.Parameters.AddWithValue("@isActive", isActive);
            command.Parameters.AddWithValue("@id", categoryId);

            connection.Open();
            command.ExecuteNonQuery();
        }

        private static Category MapCategory(SqlDataReader reader)
        {
            var category = new Category(
                name: reader.GetString(reader.GetOrdinal("category_name")),
                description: reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
                isActive: reader.GetBoolean(reader.GetOrdinal("is_active")));

            category.Id = reader.GetInt32(reader.GetOrdinal("category_id"));
            return category;
        }
    }
}
