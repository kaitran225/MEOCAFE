using Npgsql;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Data.Repositories;

public sealed class DiscountRepository : IDiscountRepository
{
    private readonly IDbConnectionFactory _factory;

    public DiscountRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<IReadOnlyList<Discount>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<Discount>();
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, name, description, percentage, is_disabled, start_date, end_date FROM discounts ORDER BY id", conn);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            list.Add(Map(reader));
        return list;
    }

    public async Task<Discount?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("SELECT id, name, description, percentage, is_disabled, start_date, end_date FROM discounts WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            return Map(reader);
        return null;
    }

    public async Task<Discount?> GetByMenuItemIdAsync(int menuItemId, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(
            "SELECT d.id, d.name, d.description, d.percentage, d.is_disabled, d.start_date, d.end_date FROM menu_item_discount mid JOIN discounts d ON mid.discount_id = d.id WHERE mid.menu_item_id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", menuItemId);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            return Map(reader);
        return null;
    }

    public async Task AddAsync(Discount discount, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(
            "INSERT INTO discounts (name, description, percentage, is_disabled, start_date, end_date) VALUES (@Name, @Description, @Percentage, @IsDisabled, @StartDate, @EndDate) RETURNING id", conn);
        cmd.Parameters.AddWithValue("@Name", discount.Name ?? "");
        cmd.Parameters.AddWithValue("@Description", (object?)discount.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Percentage", discount.Percentage);
        cmd.Parameters.AddWithValue("@IsDisabled", discount.IsDisabled);
        cmd.Parameters.AddWithValue("@StartDate", discount.StartDate);
        cmd.Parameters.AddWithValue("@EndDate", discount.EndDate);
        discount.Id = (int)(await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false) ?? 0);
    }

    public async Task UpdateAsync(Discount discount, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand(
            "UPDATE discounts SET name = @Name, description = @Description, percentage = @Percentage, is_disabled = @IsDisabled, start_date = @StartDate, end_date = @EndDate WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@Name", discount.Name ?? "");
        cmd.Parameters.AddWithValue("@Description", (object?)discount.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Percentage", discount.Percentage);
        cmd.Parameters.AddWithValue("@IsDisabled", discount.IsDisabled);
        cmd.Parameters.AddWithValue("@StartDate", discount.StartDate);
        cmd.Parameters.AddWithValue("@EndDate", discount.EndDate);
        cmd.Parameters.AddWithValue("@Id", discount.Id);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteAsync(int discountId, CancellationToken cancellationToken = default)
    {
        await using var conn = (NpgsqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = new NpgsqlCommand("DELETE FROM discounts WHERE id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", discountId);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static Discount Map(NpgsqlDataReader r) => new()
    {
        Id = r.GetInt32(0),
        Name = r.GetString(1),
        Description = r.IsDBNull(2) ? null : r.GetString(2),
        Percentage = r.GetDecimal(3),
        IsDisabled = r.GetBoolean(4),
        StartDate = r.GetDateTime(5),
        EndDate = r.GetDateTime(6)
    };
}
