using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace POS.Data;

public static class DatabaseCheckpoint
{
    public static void WalCheckpointTruncate(string databasePath)
    {
        var builder = new SqliteConnectionStringBuilder { DataSource = databasePath };
        using var conn = new SqliteConnection(builder.ConnectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "PRAGMA wal_checkpoint(TRUNCATE);";
        cmd.ExecuteNonQuery();
    }
}
