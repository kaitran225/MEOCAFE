using Npgsql;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Data.Repositories;

public sealed class OrderDetailRepository : IOrderDetailRepository
{
    private readonly IDbConnectionFactory _factory;

    public OrderDetailRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<IReadOnlyList<OrderDetail>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var list = new List<OrderDetail>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, order_id, menu_item_id, quantity, price, is_combo FROM order_details WHERE order_id = @OrderId", conn);
        cmd.Parameters.AddWithValue("@OrderId", orderId);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(new OrderDetail
            {
                Id = reader.GetInt32(0),
                OrderId = reader.GetInt32(1),
                MenuItemId = reader.GetInt32(2),
                Quantity = reader.GetInt32(3),
                Price = reader.GetDecimal(4),
                IsCombo = reader.GetBoolean(5)
            });
        return list;
    }

    public async Task AddAsync(OrderDetail detail, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(
            "INSERT INTO order_details (order_id, menu_item_id, quantity, price, is_combo) VALUES (@OrderId, @MenuItemId, @Quantity, @Price, @IsCombo) RETURNING id", conn);
        cmd.Parameters.AddWithValue("@OrderId", detail.OrderId);
        cmd.Parameters.AddWithValue("@MenuItemId", detail.MenuItemId);
        cmd.Parameters.AddWithValue("@Quantity", detail.Quantity);
        cmd.Parameters.AddWithValue("@Price", detail.Price);
        cmd.Parameters.AddWithValue("@IsCombo", detail.IsCombo);
        detail.Id = (int)(await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false) ?? 0);
    }

    public async Task AddRangeAsync(IEnumerable<OrderDetail> details, CancellationToken cancellationToken = default)
    {
        foreach (var d in details)
            await AddAsync(d, cancellationToken).ConfigureAwait(false);
    }
}
