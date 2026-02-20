using DbUp;

namespace POS.Data;

public static class DatabaseMigrationRunner
{
    public static void Run(string connectionString)
    {
        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsFromFileSystem(Path.Combine(AppContext.BaseDirectory, "Migrations"))
            .WithTransaction()
            .LogToConsole();

        var result = upgrader.Build().PerformUpgrade();
        if (!result.Successful)
            throw new InvalidOperationException("Database migration failed.", result.Error);
    }
}
