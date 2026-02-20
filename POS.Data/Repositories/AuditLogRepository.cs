using Npgsql;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Data.Repositories;

public sealed class AuditLogRepository : IAuditLogRepository
{
    private readonly IDbConnectionFactory _factory;

    public AuditLogRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task AddAsync(AuditLogEntry entry, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("INSERT INTO audit_log (username, action, entity_type, entity_id, details) VALUES (@User, @Action, @EntityType, @EntityId, @Details)", conn);
        cmd.Parameters.AddWithValue("@User", entry.Username);
        cmd.Parameters.AddWithValue("@Action", entry.Action);
        cmd.Parameters.AddWithValue("@EntityType", (object?)entry.EntityType ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@EntityId", (object?)entry.EntityId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Details", (object?)entry.Details ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<AuditLogEntry>> GetAsync(DateTime? from, DateTime? to, string? username, string? action, CancellationToken cancellationToken = default)
    {
        var list = new List<AuditLogEntry>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        var sql = "SELECT id, username, action, entity_type, entity_id, details, created_at FROM audit_log WHERE 1=1";
        if (from.HasValue) sql += " AND created_at >= @From";
        if (to.HasValue) sql += " AND created_at <= @To";
        if (!string.IsNullOrWhiteSpace(username)) sql += " AND username = @User";
        if (!string.IsNullOrWhiteSpace(action)) sql += " AND action = @Action";
        sql += " ORDER BY created_at DESC LIMIT 500";
        await using var cmd = new NpgsqlCommand(sql, conn);
        if (from.HasValue) cmd.Parameters.AddWithValue("@From", from.Value);
        if (to.HasValue) cmd.Parameters.AddWithValue("@To", to.Value);
        if (!string.IsNullOrWhiteSpace(username)) cmd.Parameters.AddWithValue("@User", username);
        if (!string.IsNullOrWhiteSpace(action)) cmd.Parameters.AddWithValue("@Action", action);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(new AuditLogEntry
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                Action = reader.GetString(2),
                EntityType = reader.IsDBNull(3) ? null : reader.GetString(3),
                EntityId = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                Details = reader.IsDBNull(5) ? null : reader.GetString(5),
                CreatedAt = reader.GetDateTime(6)
            });
        return list;
    }
}
