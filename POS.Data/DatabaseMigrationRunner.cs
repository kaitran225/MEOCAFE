using DbUp;
using DbUp.Engine;
using System.Globalization;
using System.IO;
using System.Linq;

namespace POS.Data;

public static class DatabaseMigrationRunner
{
    public static void Run(string connectionString)
    {
        var migrationsPath = Path.Combine(AppContext.BaseDirectory, "Migrations");
        var scripts = Directory.GetFiles(migrationsPath, "*.sql")
            .Select(path =>
            {
                var fileName = Path.GetFileName(path);
                var version = GetVersion(fileName);
                return new SqlScript(
                    fileName,
                    File.ReadAllText(path),
                    new SqlScriptOptions { RunGroupOrder = version });
            })
            .ToArray();

        if (scripts.Length == 0)
            throw new InvalidOperationException($"No database migrations were found in '{migrationsPath}'.");

        var upgrader = DeployChanges.To
            .SqliteDatabase(connectionString)
            .WithScripts(scripts)
            .WithTransactionPerScript()
            .LogToConsole();

        var result = upgrader.Build().PerformUpgrade();
        if (!result.Successful)
            throw new InvalidOperationException("Database migration failed.", result.Error);
    }

    private static int GetVersion(string fileName)
    {
        var separatorIndex = fileName.IndexOf("__", StringComparison.Ordinal);
        if (!fileName.StartsWith("V", StringComparison.Ordinal)
            || separatorIndex <= 1
            || !int.TryParse(
                fileName.Substring(1, separatorIndex - 1),
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out var version))
        {
            throw new InvalidOperationException(
                $"Migration filename '{fileName}' must use the format V<number>__description.sql.");
        }

        return version;
    }
}
