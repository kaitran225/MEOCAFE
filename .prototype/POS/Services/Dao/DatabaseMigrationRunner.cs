using DbUp;
using DbUp.Engine;

namespace POS.Services.Dao;

/// <summary>
/// Flyway-style migrations via DbUp: runs versioned SQL scripts from Migrations folder.
/// Tracks applied scripts in schemaversions table; only new scripts are executed.
/// </summary>
public static class DatabaseMigrationRunner
{
    public static bool Run(string connectionString)
    {
        var migrationsPath = Path.Combine(AppContext.BaseDirectory, "Migrations");
        if (!Directory.Exists(migrationsPath))
            return true;

        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsFromFileSystem(migrationsPath)
            .WithTransaction()
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();
        return result.Successful;
    }
}
