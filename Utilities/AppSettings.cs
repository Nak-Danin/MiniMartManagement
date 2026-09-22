using System.Text.Json;

namespace MiniMartManagement.Utilities
{
    /// <summary>
    /// Loads simple configuration (connection string) from appsettings.json.
    /// Kept deliberately small - this is a config reader, not a full DI/config framework.
    /// </summary>
    public static class AppSettings
    {
        private static readonly Lazy<string> _connectionString = new(LoadConnectionString);

        public static string ConnectionString => _connectionString.Value;

        private static string LoadConnectionString()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(
                    "appsettings.json was not found next to the application executable. " +
                    "Make sure it is set to 'Copy if newer' in project properties.", path);
            }

            string json = File.ReadAllText(path);
            using var doc = JsonDocument.Parse(json);

            string? connectionString = doc.RootElement
                .GetProperty("ConnectionStrings")
                .GetProperty("MiniMartDb")
                .GetString();

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("MiniMartDb connection string is missing from appsettings.json.");
            }

            return connectionString;
        }
    }
}
