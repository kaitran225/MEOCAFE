using Npgsql;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Data.Repositories;

public sealed class OrderDetailOptionRepository : IOrderDetailOptionRepository
{
    private readonly IDbConnectionFactory _factory;

    public OrderDetailOptionRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<IReadOnlyList<OrderDetailOption>> GetByOrderDetailIdAsync(int orderDetailId, CancellationToken cancellationToken = default)
    {
        var list = new List<OrderDetailOption>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, order_detail_id, option_type, option_value_id, note FROM order_detail_options WHERE order_detail_id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", orderDetailId);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(new OrderDetailOption
            {
                Id = reader.GetInt32(0),
                OrderDetailId = reader.GetInt32(1),
                OptionType = reader.GetString(2),
                OptionValueId = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                Note = reader.IsDBNull(4) ? null : reader.GetString(4)
            });
        return list;
    }

    public async Task AddAsync(OrderDetailOption option, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(
            "INSERT INTO order_detail_options (order_detail_id, option_type, option_value_id, note) VALUES (@OrderDetailId, @OptionType, @OptionValueId, @Note) RETURNING id", conn);
        cmd.Parameters.AddWithValue("@OrderDetailId", option.OrderDetailId);
        cmd.Parameters.AddWithValue("@OptionType", option.OptionType ?? "");
        cmd.Parameters.AddWithValue("@OptionValueId", (object?)option.OptionValueId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Note", (object?)option.Note ?? DBNull.Value);
        option.Id = (int)(await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false) ?? 0);
    }

    public async Task AddRangeAsync(IEnumerable<OrderDetailOption> options, CancellationToken cancellationToken = default)
    {
        foreach (var o in options)
            await AddAsync(o, cancellationToken).ConfigureAwait(false);
    }
}
