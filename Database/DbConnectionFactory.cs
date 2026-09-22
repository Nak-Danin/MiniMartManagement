using Microsoft.Data.SqlClient;
using MiniMartManagement.Utilities;

namespace MiniMartManagement.Database
{
    /// <summary>
    /// Single place responsible for creating SqlConnection objects.
    /// Repositories depend on this instead of building connection strings themselves.
    /// </summary>
    public class DbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory() : this(AppSettings.ConnectionString) { }

        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
