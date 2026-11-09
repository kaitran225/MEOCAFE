using System.Data.Common;
using POS.Data;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Data.Repositories;

public sealed class WasteRepository : IWasteRepository
{
    private readonly IDbConnectionFactory _factory;

    public WasteRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<IngredientWaste>> GetAllAsync(CancellationToken ct)
    {
        var list = new List<IngredientWaste>();
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(ct).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand(
            "SELECT id, item_name, quantity, unit, reason, logged_by, logged_at, notes FROM ingredient_waste ORDER BY logged_at DESC LIMIT 500");
        await using var reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
        while (await reader.ReadAsync(ct).ConfigureAwait(false))
            list.Add(new IngredientWaste
            {
                Id = reader.GetInt64(0),
                ItemName = reader.GetString(1),
                Quantity = reader.GetDecimal(2),
                Unit = reader.GetString(3),
                Reason = reader.GetString(4),
                LoggedBy = reader.GetString(5),
                LoggedAt = reader.ReadDateTime(6),
                Notes = reader.IsDBNull(7) ? null : reader.GetString(7)
            });
        return list;
    }

    public async Task<long> InsertAsync(IngredientWaste entry, CancellationToken ct)
    {
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(ct).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand(
            "INSERT INTO ingredient_waste (item_name, quantity, unit, reason, logged_by, logged_at, notes) VALUES (@ItemName, @Quantity, @Unit, @Reason, @LoggedBy, @LoggedAt, @Notes) RETURNING id");
        cmd.Parameters.AddWithValue("@ItemName", entry.ItemName);
        cmd.Parameters.AddWithValue("@Quantity", entry.Quantity);
        cmd.Parameters.AddWithValue("@Unit", entry.Unit);
        cmd.Parameters.AddWithValue("@Reason", entry.Reason);
        cmd.Parameters.AddWithValue("@LoggedBy", entry.LoggedBy);
        cmd.Parameters.AddWithValue("@LoggedAt", entry.LoggedAt);
        cmd.Parameters.AddWithValue("@Notes", (object?)entry.Notes ?? DBNull.Value);
        var result = await cmd.ExecuteScalarAsync(ct).ConfigureAwait(false);
        return Convert.ToInt64(result);
    }
}
