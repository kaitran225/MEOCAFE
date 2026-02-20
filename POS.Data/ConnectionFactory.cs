using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Npgsql;
using POS.Core.Contracts;

namespace POS.Data;

public sealed class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public NpgsqlConnectionFactory(IConfiguration configuration)
    {
        var host = configuration["DB_HOST"] ?? "localhost";
        var port = configuration["DB_PORT"] ?? "5432";
        var user = configuration["DB_USER"] ?? "postgres";
        var password = configuration["DB_PASSWORD"] ?? "123456";
        var database = configuration["DB_NAME"] ?? "dev_meo_cf";
        _connectionString = $"Host={host};Port={port};Username={user};Password={password};Database={database}";
    }

    public DbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}
