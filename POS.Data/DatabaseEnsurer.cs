using Npgsql;

namespace POS.Data;

/// <summary>
/// Ensures the target database exists by connecting to "postgres" and creating it if missing.
/// </summary>
public static class DatabaseEnsurer
{
    public static void EnsureExists(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var database = builder.Database;
        if (string.IsNullOrWhiteSpace(database))
            return;
        builder.Database = "postgres";
        var adminCs = builder.ToString();
        using var conn = new NpgsqlConnection(adminCs);
        conn.Open();
        using (var cmd = new NpgsqlCommand("SELECT 1 FROM pg_database WHERE datname = @name", conn))
        {
            cmd.Parameters.AddWithValue("name", database);
            if (cmd.ExecuteScalar() != null)
                return;
        }
        var safeName = "\"" + database.Replace("\"", "\"\"") + "\"";
        using (var create = new NpgsqlCommand("CREATE DATABASE " + safeName, conn))
            create.ExecuteNonQuery();
    }
}
