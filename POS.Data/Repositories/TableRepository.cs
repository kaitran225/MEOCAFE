using Npgsql;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Data.Repositories;

public sealed class TableRepository : ITableRepository
{
    private readonly IDbConnectionFactory _factory;

    public TableRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<IReadOnlyList<Table>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<Table>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, name, capacity, zone, COALESCE(status, 'empty') FROM tables ORDER BY name", conn);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(new Table
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Capacity = reader.GetInt32(2),
                Zone = reader.IsDBNull(3) ? null : reader.GetString(3),
                Status = reader.IsDBNull(4) ? "empty" : reader.GetString(4)
            });
        return list;
    }

    public async Task<Table?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, name, capacity, zone, COALESCE(status, 'empty') FROM tables WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            return new Table { Id = reader.GetInt32(0), Name = reader.GetString(1), Capacity = reader.GetInt32(2), Zone = reader.IsDBNull(3) ? null : reader.GetString(3), Status = reader.IsDBNull(4) ? "empty" : reader.GetString(4) };
        return null;
    }

    public async Task UpdateStatusAsync(int id, string status, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("UPDATE tables SET status = @Status WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@Status", status);
        cmd.Parameters.AddWithValue("@Id", id);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }
}
