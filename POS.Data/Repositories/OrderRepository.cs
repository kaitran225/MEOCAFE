using Npgsql;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Data.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly IDbConnectionFactory _factory;

    public OrderRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<Order>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, total_price, phone_number, customer_id, created_at, note, COALESCE(kitchen_status, 'pending'), table_id, COALESCE(service_type, 'Dine-in'), delivery_address, COALESCE(delivery_fee, 0) FROM orders ORDER BY created_at DESC", conn);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(MapOrder(reader));
        return list;
    }

    public async Task<IReadOnlyList<Order>> GetByKitchenStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        var list = new List<Order>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, total_price, phone_number, customer_id, created_at, note, COALESCE(kitchen_status, 'pending'), table_id, COALESCE(service_type, 'Dine-in'), delivery_address, COALESCE(delivery_fee, 0) FROM orders WHERE COALESCE(kitchen_status, 'pending') = @Status ORDER BY created_at ASC", conn);
        cmd.Parameters.AddWithValue("@Status", status);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(MapOrder(reader));
        return list;
    }

    public async Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, total_price, phone_number, customer_id, created_at, note, COALESCE(kitchen_status, 'pending'), table_id, COALESCE(service_type, 'Dine-in'), delivery_address, COALESCE(delivery_fee, 0) FROM orders WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            return MapOrder(reader);
        return null;
    }

    public async Task<int> AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(
            "INSERT INTO orders (total_price, phone_number, customer_id, note, table_id, service_type, delivery_address, delivery_fee) VALUES (@TotalPrice, @PhoneNumber, @CustomerId, @Note, @TableId, @ServiceType, @DeliveryAddress, @DeliveryFee) RETURNING id", conn);
        cmd.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);
        cmd.Parameters.AddWithValue("@PhoneNumber", (object?)order.PhoneNumber ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CustomerId", (object?)order.CustomerId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Note", (object?)order.Note ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@TableId", (object?)order.TableId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ServiceType", order.ServiceType ?? "Dine-in");
        cmd.Parameters.AddWithValue("@DeliveryAddress", (object?)order.DeliveryAddress ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DeliveryFee", order.DeliveryFee);
        var id = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return Convert.ToInt32(id);
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(
            "UPDATE orders SET total_price = @TotalPrice, phone_number = @PhoneNumber, customer_id = @CustomerId, note = @Note WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);
        cmd.Parameters.AddWithValue("@PhoneNumber", (object?)order.PhoneNumber ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CustomerId", (object?)order.CustomerId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Note", (object?)order.Note ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Id", order.Id);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteAsync(int orderId, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("DELETE FROM orders WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", orderId);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task UpdateKitchenStatusAsync(int orderId, string status, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("UPDATE orders SET kitchen_status = @Status WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@Status", status);
        cmd.Parameters.AddWithValue("@Id", orderId);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static Order MapOrder(NpgsqlDataReader reader)
    {
        var o = new Order
        {
            Id = reader.GetInt32(0),
            TotalPrice = reader.GetDecimal(1),
            PhoneNumber = reader.IsDBNull(2) ? null : reader.GetString(2),
            CustomerId = reader.IsDBNull(3) ? null : reader.GetInt32(3),
            CreatedAt = reader.GetDateTime(4),
            Note = reader.IsDBNull(5) ? null : reader.GetString(5),
            KitchenStatus = reader.IsDBNull(6) ? "pending" : reader.GetString(6)
        };
        if (reader.FieldCount > 7) o.TableId = reader.IsDBNull(7) ? null : reader.GetInt32(7);
        if (reader.FieldCount > 8) o.ServiceType = reader.IsDBNull(8) ? "Dine-in" : reader.GetString(8);
        if (reader.FieldCount > 9) o.DeliveryAddress = reader.IsDBNull(9) ? null : reader.GetString(9);
        if (reader.FieldCount > 10) o.DeliveryFee = reader.GetDecimal(10);
        return o;
    }
}
