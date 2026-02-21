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

    public async Task AddAsync(Table table, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("INSERT INTO tables (name, capacity, zone, status) VALUES (@Name, @Capacity, @Zone, COALESCE(@Status, 'empty')) RETURNING id", conn);
        cmd.Parameters.AddWithValue("@Name", table.Name ?? "");
        cmd.Parameters.AddWithValue("@Capacity", table.Capacity);
        cmd.Parameters.AddWithValue("@Zone", (object?)table.Zone ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Status", table.Status ?? "empty");
        table.Id = (int)(await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false) ?? 0);
    }

    public async Task UpdateAsync(Table table, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("UPDATE tables SET name = @Name, capacity = @Capacity, zone = @Zone, status = @Status WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@Name", table.Name ?? "");
        cmd.Parameters.AddWithValue("@Capacity", table.Capacity);
        cmd.Parameters.AddWithValue("@Zone", (object?)table.Zone ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Status", table.Status ?? "empty");
        cmd.Parameters.AddWithValue("@Id", table.Id);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("DELETE FROM tables WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
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
