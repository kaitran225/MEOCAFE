using Npgsql;
using Windows.Storage;

namespace POS.Services.Dao
{
    public class DatabaseManager
    {
        private ApplicationDataContainer dbConfig = ApplicationData.Current.LocalSettings;

        private string host;
        private string port;
        private string username;
        private string password;
        private string database;

        private readonly string _connectionString;

        public NpgsqlConnection Connection { get; private set; }
        public NpgsqlCommand Command { get; private set; }

        public DatabaseManager()
        {
            host = dbConfig.Values["DB_HOST"]?.ToString() ?? Config.GetSetting("DB_HOST") ?? "localhost";
            port = dbConfig.Values["DB_PORT"]?.ToString() ?? Config.GetSetting("DB_PORT") ?? "5792";
            username = dbConfig.Values["DB_USER"]?.ToString() ?? Config.GetSetting("DB_USER") ?? "postgres";
            password = dbConfig.Values["DB_PASSWORD"]?.ToString() ?? Config.GetSetting("DB_PASSWORD") ?? "postgres";
            database = dbConfig.Values["DB_NAME"]?.ToString() ?? Config.GetSetting("DB_NAME") ?? "db";

            _connectionString = $"Host={host};Port={port};Username={username};Password={password};Database={database}";
        }

        public void Connect()
        {
            Connection = new NpgsqlConnection(_connectionString);
            Connection.Open();
            Command = Connection.CreateCommand();
        }

        public void Disconnect()
        {
            if (Connection != null && Connection.State == System.Data.ConnectionState.Open)
            {
                Connection.Close();
            }
        }
        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}
