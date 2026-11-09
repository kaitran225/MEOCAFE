using Microsoft.Extensions.Configuration;

namespace POS.Data;

public static class SqliteDatabaseEnsurer
{
    public static string EnsureExists(IConfiguration configuration)
    {
        var path = DatabasePathResolver.Resolve(configuration);
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        if (!File.Exists(path))
            SqliteConnectionFactoryBootstrap.CreateEmptyDatabase(path);

        return path;
    }
}

internal static class SqliteConnectionFactoryBootstrap
{
    public static void CreateEmptyDatabase(string path)
    {
        using var conn = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={path}");
        conn.Open();
    }
}
