using Npgsql;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Data.Repositories;

public sealed class ComboRepository : IComboRepository
{
    private readonly IDbConnectionFactory _factory;

    public ComboRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<IReadOnlyList<ComboMenuItem>> GetComboMenuItemsAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<ComboMenuItem>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, name, combo_price, description, quantity, image FROM combo_menu_items ORDER BY id", conn);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(new ComboMenuItem
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                SellPrice = reader.GetDecimal(2),
                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                Quantity = reader.GetInt32(4),
                Image = reader.IsDBNull(5) ? null : (byte[]?)reader.GetValue(5)
            });
        return list;
    }

    public async Task<IReadOnlyList<ComboItem>> GetComboItemsByComboIdAsync(int comboId, CancellationToken cancellationToken = default)
    {
        var list = new List<ComboItem>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, combo_id, menu_item_id FROM combo_items WHERE combo_id = @Id ORDER BY id", conn);
        cmd.Parameters.AddWithValue("@Id", comboId);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(new ComboItem { Id = reader.GetInt32(0), ComboMenuItemId = reader.GetInt32(1), MenuItemId = reader.GetInt32(2) });
        return list;
    }

    public async Task<IReadOnlyList<Item>> GetComboItemsAsync(int comboId, CancellationToken cancellationToken = default)
    {
        var list = new List<Item>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(
            "SELECT m.id, m.name, m.sell_price, m.quantity, m.image FROM combo_items c JOIN menu_items m ON c.menu_item_id = m.id WHERE c.combo_id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", comboId);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(new Item
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                SellPrice = reader.GetDecimal(2),
                Quantity = reader.GetInt32(3),
                Image = reader.IsDBNull(4) ? null : (byte[]?)reader.GetValue(4)
            });
        return list;
    }

    public async Task AddComboMenuItemAsync(ComboMenuItem combo, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(
            "INSERT INTO combo_menu_items (name, combo_price, description, quantity, image) VALUES (@Name, @Price, @Description, @Quantity, @Image) RETURNING id", conn);
        cmd.Parameters.AddWithValue("@Name", combo.Name ?? "");
        cmd.Parameters.AddWithValue("@Price", combo.SellPrice);
        cmd.Parameters.AddWithValue("@Description", (object?)combo.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Quantity", combo.Quantity);
        cmd.Parameters.AddWithValue("@Image", (object?)combo.Image ?? DBNull.Value);
        combo.Id = (int)(await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false) ?? 0);
    }

    public async Task UpdateComboMenuItemAsync(ComboMenuItem combo, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(
            "UPDATE combo_menu_items SET name = @Name, combo_price = @Price, description = @Description, quantity = @Quantity, image = @Image WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@Name", combo.Name ?? "");
        cmd.Parameters.AddWithValue("@Price", combo.SellPrice);
        cmd.Parameters.AddWithValue("@Description", (object?)combo.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Quantity", combo.Quantity);
        cmd.Parameters.AddWithValue("@Image", (object?)combo.Image ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Id", combo.Id);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteComboMenuItemAsync(int comboId, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("DELETE FROM combo_menu_items WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", comboId);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task AddComboItemAsync(ComboItem item, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("INSERT INTO combo_items (combo_id, menu_item_id) VALUES (@ComboId, @MenuItemId)", conn);
        cmd.Parameters.AddWithValue("@ComboId", item.ComboMenuItemId);
        cmd.Parameters.AddWithValue("@MenuItemId", item.MenuItemId);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteComboItemAsync(int comboId, int menuItemId, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("DELETE FROM combo_items WHERE combo_id = @ComboId AND menu_item_id = @MenuItemId", conn);
        cmd.Parameters.AddWithValue("@ComboId", comboId);
        cmd.Parameters.AddWithValue("@MenuItemId", menuItemId);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }
}
