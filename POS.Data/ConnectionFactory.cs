using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using POS.Core.Contracts;

namespace POS.Data;

public sealed class SqliteConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(IConfiguration configuration)
    {
        var path = DatabasePathResolver.Resolve(configuration);
        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = path,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared
        }.ConnectionString;
        DatabaseFilePath = path;
    }

    public string DatabaseFilePath { get; }

    public DbConnection CreateConnection() => new SqliteConnection(_connectionString);
}
