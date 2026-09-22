using Microsoft.Data.SqlClient;
using MiniMartManagement.Database;
using MiniMartManagement.Interfaces;
using MiniMartManagement.Models;

namespace MiniMartManagement.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public UserRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private const string BaseSelect = @"
            SELECT u.user_id, u.username, u.password_hash, u.role, u.is_active, u.created_at,
                   e.employee_id, e.first_name, e.last_name, e.phone, e.email, e.address, e.hire_date, e.status
            FROM Users u
            LEFT JOIN Employees e ON u.user_id = e.user_id";

        public User? GetByUsername(string username)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(BaseSelect + " WHERE u.username = @username", connection);
            command.Parameters.AddWithValue("@username", username);

            connection.Open();
            using var reader = command.ExecuteReader();
            return reader.Read() ? MapUser(reader) : null;
        }

        public User? GetById(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(BaseSelect + " WHERE u.user_id = @userId", connection);
            command.Parameters.AddWithValue("@userId", userId);

            connection.Open();
            using var reader = command.ExecuteReader();
            return reader.Read() ? MapUser(reader) : null;
        }

        public void UpdatePasswordHash(int userId, string newPasswordHash)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "UPDATE Users SET password_hash = @hash WHERE user_id = @userId", connection);
            command.Parameters.AddWithValue("@hash", newPasswordHash);
            command.Parameters.AddWithValue("@userId", userId);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void SetActive(int userId, bool isActive)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                "UPDATE Users SET is_active = @isActive WHERE user_id = @userId", connection);
            command.Parameters.AddWithValue("@isActive", isActive);
            command.Parameters.AddWithValue("@userId", userId);

            connection.Open();
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Builds the correct concrete User subtype from a row of the BaseSelect
        /// query above. Internal so EmployeeRepository can reuse it too.
        /// </summary>
        internal static User MapUser(SqlDataReader reader)
        {
            string role = reader.GetString(reader.GetOrdinal("role"));
            int userId = reader.GetInt32(reader.GetOrdinal("user_id"));
            string username = reader.GetString(reader.GetOrdinal("username"));
            string passwordHash = reader.GetString(reader.GetOrdinal("password_hash"));
            bool isActive = reader.GetBoolean(reader.GetOrdinal("is_active"));
            DateTime createdAt = reader.GetDateTime(reader.GetOrdinal("created_at"));

            User user;

            if (role == "Admin")
            {
                user = new Admin(username, passwordHash, isActive, createdAt);
            }
            else
            {
                int employeeIdOrdinal = reader.GetOrdinal("employee_id");
                if (reader.IsDBNull(employeeIdOrdinal))
                {
                    throw new InvalidOperationException(
                        $"User '{username}' has role Employee but no matching Employees record was found.");
                }

                var employee = new Employee(
                    username, passwordHash, isActive, createdAt,
                    firstName: reader.GetString(reader.GetOrdinal("first_name")),
                    lastName: reader.GetString(reader.GetOrdinal("last_name")),
                    phone: reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString(reader.GetOrdinal("phone")),
                    email: reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                    address: reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader.GetString(reader.GetOrdinal("address")),
                    hireDate: DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("hire_date"))),
                    status: Enum.Parse<EmployeeStatus>(reader.GetString(reader.GetOrdinal("status"))));

                employee.EmployeeId = reader.GetInt32(employeeIdOrdinal);
                user = employee;
            }

            user.Id = userId;
            return user;
        }
    }
}
