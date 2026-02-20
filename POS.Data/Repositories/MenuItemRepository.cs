using Npgsql;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Data.Repositories;

public sealed class MenuItemRepository : IMenuItemRepository
{
    private readonly IDbConnectionFactory _factory;

    public MenuItemRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<IReadOnlyList<MenuItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<MenuItem>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, name, sell_price, capital_price, quantity, image, category_id, COALESCE(reorder_level, 0) FROM menu_items ORDER BY id ASC", conn);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(MapMenuItem(reader));
        return list;
    }

    public async Task<MenuItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, name, sell_price, capital_price, quantity, image, category_id, COALESCE(reorder_level, 0) FROM menu_items WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            return MapMenuItem(reader);
        return null;
    }

    public async Task AddAsync(MenuItem menuItem, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(
            "INSERT INTO menu_items (name, sell_price, capital_price, quantity, image, category_id, reorder_level) VALUES (@Name, @SellPrice, @CapitalPrice, @Quantity, @Image, @CategoryId, @ReorderLevel) RETURNING id", conn);
        cmd.Parameters.AddWithValue("@Name", menuItem.Name ?? "");
        cmd.Parameters.AddWithValue("@SellPrice", menuItem.SellPrice);
        cmd.Parameters.AddWithValue("@CapitalPrice", menuItem.CapitalPrice);
        cmd.Parameters.AddWithValue("@Quantity", menuItem.Quantity);
        cmd.Parameters.AddWithValue("@Image", (object?)menuItem.Image ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CategoryId", menuItem.CategoryId);
        cmd.Parameters.AddWithValue("@ReorderLevel", menuItem.ReorderLevel);
        menuItem.Id = (int)(await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false) ?? 0);
    }

    public async Task UpdateAsync(MenuItem menuItem, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(
            "UPDATE menu_items SET name = @Name, sell_price = @SellPrice, capital_price = @CapitalPrice, quantity = @Quantity, image = @Image, reorder_level = @ReorderLevel WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@Name", menuItem.Name ?? "");
        cmd.Parameters.AddWithValue("@SellPrice", menuItem.SellPrice);
        cmd.Parameters.AddWithValue("@CapitalPrice", menuItem.CapitalPrice);
        cmd.Parameters.AddWithValue("@Quantity", menuItem.Quantity);
        cmd.Parameters.AddWithValue("@Image", (object?)menuItem.Image ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ReorderLevel", menuItem.ReorderLevel);
        cmd.Parameters.AddWithValue("@Id", menuItem.Id);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task DecreaseQuantityAsync(int menuItemId, int amount, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("UPDATE menu_items SET quantity = GREATEST(0, quantity - @Amount) WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@Amount", amount);
        cmd.Parameters.AddWithValue("@Id", menuItemId);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<MenuItem>> GetLowStockAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<MenuItem>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, name, sell_price, capital_price, quantity, image, category_id, COALESCE(reorder_level, 0) FROM menu_items WHERE quantity <= COALESCE(reorder_level, 0) AND reorder_level > 0 ORDER BY quantity ASC", conn);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(MapMenuItem(reader));
        return list;
    }

    public async Task DeleteAsync(int menuItemId, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("DELETE FROM menu_items WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", menuItemId);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static MenuItem MapMenuItem(NpgsqlDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        Name = reader.GetString(1),
        SellPrice = reader.GetDecimal(2),
        CapitalPrice = reader.GetDecimal(3),
        Quantity = reader.GetInt32(4),
        Image = reader.IsDBNull(5) ? null : (byte[]?)reader.GetValue(5),
        CategoryId = reader.GetInt32(6),
        ReorderLevel = reader.FieldCount > 7 ? reader.GetInt32(7) : 0
    };
}
