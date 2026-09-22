using Microsoft.Data.SqlClient;
using MiniMartManagement.Database;
using MiniMartManagement.Interfaces;
using MiniMartManagement.Models;

namespace MiniMartManagement.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public EmployeeRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private const string BaseSelect = @"
            SELECT u.user_id, u.username, u.password_hash, u.role, u.is_active, u.created_at,
                   e.employee_id, e.first_name, e.last_name, e.phone, e.email, e.address, e.hire_date, e.status
            FROM Employees e
            JOIN Users u ON u.user_id = e.user_id";

        public Employee? GetByEmployeeId(int employeeId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(BaseSelect + " WHERE e.employee_id = @employeeId", connection);
            command.Parameters.AddWithValue("@employeeId", employeeId);

            connection.Open();
            using var reader = command.ExecuteReader();
            return reader.Read() ? (Employee)UserRepository.MapUser(reader) : null;
        }

        public Employee? GetByUserId(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(BaseSelect + " WHERE u.user_id = @userId", connection);
            command.Parameters.AddWithValue("@userId", userId);

            connection.Open();
            using var reader = command.ExecuteReader();
            return reader.Read() ? (Employee)UserRepository.MapUser(reader) : null;
        }

        public List<Employee> GetAll()
        {
            var employees = new List<Employee>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(BaseSelect + " ORDER BY e.first_name, e.last_name", connection);

            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                employees.Add((Employee)UserRepository.MapUser(reader));
            }

            return employees;
        }

        public List<Employee> Search(string keyword)
        {
            var employees = new List<Employee>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(
                BaseSelect + @" WHERE e.first_name LIKE @keyword OR e.last_name LIKE @keyword OR u.username LIKE @keyword
                                 ORDER BY e.first_name, e.last_name",
                connection);
            command.Parameters.AddWithValue("@keyword", $"%{keyword}%");

            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                employees.Add((Employee)UserRepository.MapUser(reader));
            }

            return employees;
        }

        /// <summary>
        /// Inserts the Users row and the Employees row together. If the
        /// Employees insert fails (e.g. bad data), the Users insert is
        /// rolled back too - we never want a login with no profile.
        /// </summary>
        public void Add(Employee employee)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                using var insertUser = new SqlCommand(@"
                    INSERT INTO Users (username, password_hash, role, is_active, created_at)
                    OUTPUT INSERTED.user_id
                    VALUES (@username, @passwordHash, 'Employee', @isActive, @createdAt)",
                    connection, transaction);

                insertUser.Parameters.AddWithValue("@username", employee.Username);
                insertUser.Parameters.AddWithValue("@passwordHash", employee.PasswordHash);
                insertUser.Parameters.AddWithValue("@isActive", employee.IsActive);
                insertUser.Parameters.AddWithValue("@createdAt", employee.CreatedAt);

                int newUserId = (int)insertUser.ExecuteScalar();

                using var insertEmployee = new SqlCommand(@"
                    INSERT INTO Employees (user_id, first_name, last_name, phone, email, address, hire_date, status)
                    OUTPUT INSERTED.employee_id
                    VALUES (@userId, @firstName, @lastName, @phone, @email, @address, @hireDate, @status)",
                    connection, transaction);

                insertEmployee.Parameters.AddWithValue("@userId", newUserId);
                insertEmployee.Parameters.AddWithValue("@firstName", employee.FirstName);
                insertEmployee.Parameters.AddWithValue("@lastName", employee.LastName);
                insertEmployee.Parameters.AddWithValue("@phone", (object?)employee.Phone ?? DBNull.Value);
                insertEmployee.Parameters.AddWithValue("@email", (object?)employee.Email ?? DBNull.Value);
                insertEmployee.Parameters.AddWithValue("@address", (object?)employee.Address ?? DBNull.Value);
                insertEmployee.Parameters.AddWithValue("@hireDate", employee.HireDate.ToDateTime(TimeOnly.MinValue));
                insertEmployee.Parameters.AddWithValue("@status", employee.Status.ToString());

                int newEmployeeId = (int)insertEmployee.ExecuteScalar();

                transaction.Commit();

                employee.Id = newUserId;
                employee.EmployeeId = newEmployeeId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void Update(Employee employee)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand(@"
                UPDATE Employees
                SET first_name = @firstName, last_name = @lastName, phone = @phone,
                    email = @email, address = @address
                WHERE employee_id = @employeeId", connection);

            command.Parameters.AddWithValue("@firstName", employee.FirstName);
            command.Parameters.AddWithValue("@lastName", employee.LastName);
            command.Parameters.AddWithValue("@phone", (object?)employee.Phone ?? DBNull.Value);
            command.Parameters.AddWithValue("@email", (object?)employee.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@address", (object?)employee.Address ?? DBNull.Value);
            command.Parameters.AddWithValue("@employeeId", employee.EmployeeId);

            connection.Open();
            command.ExecuteNonQuery();
        }

        /// <summary>Updates Employees.status and Users.is_active together so a deactivated employee is reliably blocked from login.</summary>
        public void SetStatus(int employeeId, EmployeeStatus status)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                using var updateEmployee = new SqlCommand(
                    "UPDATE Employees SET status = @status WHERE employee_id = @employeeId",
                    connection, transaction);
                updateEmployee.Parameters.AddWithValue("@status", status.ToString());
                updateEmployee.Parameters.AddWithValue("@employeeId", employeeId);
                updateEmployee.ExecuteNonQuery();

                using var updateUser = new SqlCommand(@"
                    UPDATE Users
                    SET is_active = @isActive
                    WHERE user_id = (SELECT user_id FROM Employees WHERE employee_id = @employeeId)",
                    connection, transaction);
                updateUser.Parameters.AddWithValue("@isActive", status == EmployeeStatus.Active);
                updateUser.Parameters.AddWithValue("@employeeId", employeeId);
                updateUser.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
