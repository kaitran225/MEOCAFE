using Npgsql;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Data.Repositories;

public sealed class ShiftSessionRepository : IShiftSessionRepository
{
    private readonly IDbConnectionFactory _factory;

    public ShiftSessionRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<int> StartShiftAsync(string username, decimal openingCash, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("INSERT INTO shift_sessions (username, opening_cash) VALUES (@User, @Cash) RETURNING id", conn);
        cmd.Parameters.AddWithValue("@User", username);
        cmd.Parameters.AddWithValue("@Cash", openingCash);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false));
    }

    public async Task EndShiftAsync(int sessionId, decimal closingCash, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("UPDATE shift_sessions SET end_at = CURRENT_TIMESTAMP, closing_cash = @Cash WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@Cash", closingCash);
        cmd.Parameters.AddWithValue("@Id", sessionId);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<ShiftSession?> GetCurrentAsync(string username, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, username, start_at, end_at, opening_cash, closing_cash FROM shift_sessions WHERE username = @User AND end_at IS NULL ORDER BY start_at DESC LIMIT 1", conn);
        cmd.Parameters.AddWithValue("@User", username);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            return new ShiftSession
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                StartAt = reader.GetDateTime(2),
                EndAt = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                OpeningCash = reader.GetDecimal(4),
                ClosingCash = reader.IsDBNull(5) ? null : reader.GetDecimal(5)
            };
        return null;
    }

    public async Task<IReadOnlyList<ShiftSession>> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var list = new List<ShiftSession>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, username, start_at, end_at, opening_cash, closing_cash FROM shift_sessions WHERE start_at::date = @Day ORDER BY start_at", conn);
        cmd.Parameters.AddWithValue("@Day", date.Date);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(new ShiftSession
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                StartAt = reader.GetDateTime(2),
                EndAt = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                OpeningCash = reader.GetDecimal(4),
                ClosingCash = reader.IsDBNull(5) ? null : reader.GetDecimal(5)
            });
        return list;
    }
}
