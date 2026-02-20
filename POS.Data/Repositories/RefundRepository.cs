using Npgsql;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Data.Repositories;

public sealed class RefundRepository : IRefundRepository
{
    private readonly IDbConnectionFactory _factory;

    public RefundRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<int> AddAsync(Refund refund, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("INSERT INTO refunds (order_id, amount, method, reason) VALUES (@OrderId, @Amount, @Method, @Reason) RETURNING id", conn);
        cmd.Parameters.AddWithValue("@OrderId", refund.OrderId);
        cmd.Parameters.AddWithValue("@Amount", refund.Amount);
        cmd.Parameters.AddWithValue("@Method", refund.Method ?? "Cash");
        cmd.Parameters.AddWithValue("@Reason", (object?)refund.Reason ?? DBNull.Value);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false));
    }

    public async Task<IReadOnlyList<Refund>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var list = new List<Refund>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, order_id, amount, method, reason, created_at FROM refunds WHERE order_id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", orderId);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(new Refund { Id = reader.GetInt32(0), OrderId = reader.GetInt32(1), Amount = reader.GetDecimal(2), Method = reader.GetString(3), Reason = reader.IsDBNull(4) ? null : reader.GetString(4), CreatedAt = reader.GetDateTime(5) });
        return list;
    }
}
