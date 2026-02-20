using Npgsql;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Data.Repositories;

public sealed class ProductOptionRepository : IProductOptionRepository
{
    private readonly IDbConnectionFactory _factory;

    public ProductOptionRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<IReadOnlyList<ProductOptionGroup>> GetOptionGroupsAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<ProductOptionGroup>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, name, option_type FROM product_option_groups ORDER BY id", conn);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(new ProductOptionGroup { Id = reader.GetInt32(0), Name = reader.GetString(1), OptionType = reader.GetString(2) });
        return list;
    }

    public async Task<IReadOnlyList<ProductOptionGroup>> GetOptionGroupsForProductAsync(int menuItemId, CancellationToken cancellationToken = default)
    {
        var list = new List<ProductOptionGroup>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(
            "SELECT g.id, g.name, g.option_type FROM menu_item_option_groups m JOIN product_option_groups g ON m.option_group_id = g.id WHERE m.menu_item_id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", menuItemId);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(new ProductOptionGroup { Id = reader.GetInt32(0), Name = reader.GetString(1), OptionType = reader.GetString(2) });
        return list;
    }

    public async Task<ProductOptionValue?> GetOptionValueByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, option_group_id, name, extra_price FROM product_option_values WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            return new ProductOptionValue
            {
                Id = reader.GetInt32(0),
                OptionGroupId = reader.GetInt32(1),
                Name = reader.GetString(2),
                ExtraPrice = reader.IsDBNull(3) ? null : reader.GetDecimal(3)
            };
        return null;
    }

    public async Task AddOptionGroupAsync(ProductOptionGroup group, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("INSERT INTO product_option_groups (name, option_type) VALUES (@Name, @OptionType) RETURNING id", conn);
        cmd.Parameters.AddWithValue("@Name", group.Name ?? "");
        cmd.Parameters.AddWithValue("@OptionType", group.OptionType ?? "");
        group.Id = (int)(await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false) ?? 0);
    }

    public async Task AddOptionValueAsync(ProductOptionValue value, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("INSERT INTO product_option_values (option_group_id, name, extra_price) VALUES (@GroupId, @Name, @ExtraPrice) RETURNING id", conn);
        cmd.Parameters.AddWithValue("@GroupId", value.OptionGroupId);
        cmd.Parameters.AddWithValue("@Name", value.Name ?? "");
        cmd.Parameters.AddWithValue("@ExtraPrice", (object?)value.ExtraPrice ?? DBNull.Value);
        value.Id = (int)(await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false) ?? 0);
    }
}
