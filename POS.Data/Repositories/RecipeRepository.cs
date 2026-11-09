using System.Data.Common;
using POS.Data;
using System.Text.Json;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Data.Repositories;

public sealed class RecipeRepository : IRecipeRepository
{
    private readonly IDbConnectionFactory _factory;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public RecipeRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<DrinkRecipe?> GetByMenuItemIdAsync(int menuItemId, CancellationToken cancellationToken = default)
    {
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand(
            "SELECT id, menu_item_id, variant_note, ingredients, steps FROM drink_recipes WHERE menu_item_id = @Id AND variant_note IS NULL LIMIT 1");
        cmd.Parameters.AddWithValue("@Id", menuItemId);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        return await ReadRecipeAsync(reader, cancellationToken).ConfigureAwait(false);
    }

    public async Task<DrinkRecipe?> GetByMenuItemAndNoteAsync(int menuItemId, string? note, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(note))
            return await GetByMenuItemIdAsync(menuItemId, cancellationToken).ConfigureAwait(false);

        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand(
            "SELECT id, menu_item_id, variant_note, ingredients, steps FROM drink_recipes WHERE menu_item_id = @Id AND variant_note IS NOT NULL AND LOWER(TRIM(variant_note)) = LOWER(TRIM(@Note)) LIMIT 1");
        cmd.Parameters.AddWithValue("@Id", menuItemId);
        cmd.Parameters.AddWithValue("@Note", note ?? "");
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        var variant = await ReadRecipeAsync(reader, cancellationToken).ConfigureAwait(false);
        if (variant != null) return variant;
        return await GetByMenuItemIdAsync(menuItemId, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<DrinkRecipe>> GetAllDefaultsAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<DrinkRecipe>();
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand(
            "SELECT id, menu_item_id, variant_note, ingredients, steps FROM drink_recipes WHERE variant_note IS NULL ORDER BY menu_item_id");
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            var r = MapRecipe(reader);
            if (r != null) list.Add(r);
        }
        return list;
    }

    public async Task AddOrUpdateAsync(DrinkRecipe recipe, CancellationToken cancellationToken = default)
    {
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        var ingredientsJson = JsonSerializer.Serialize(recipe.Ingredients ?? new List<string>());
        var stepsJson = JsonSerializer.Serialize(recipe.Steps ?? new List<string>());
        if (recipe.Id > 0)
        {
            await using var cmd = conn.CreateTextCommand(
                "UPDATE drink_recipes SET ingredients = @Ingredients, steps = @Steps WHERE id = @Id");
            cmd.Parameters.AddWithValue("@Ingredients", ingredientsJson);
            cmd.Parameters.AddWithValue("@Steps", stepsJson);
            cmd.Parameters.AddWithValue("@Id", recipe.Id);
            await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            return;
        }
        var variantNote = string.IsNullOrWhiteSpace(recipe.VariantNote) ? null : recipe.VariantNote.Trim();
        await using var selCmd = conn.CreateTextCommand(
            "SELECT id FROM drink_recipes WHERE menu_item_id = @MenuItemId AND (variant_note IS NULL AND @VariantNote IS NULL OR variant_note = @VariantNote) LIMIT 1");
        selCmd.Parameters.AddWithValue("@MenuItemId", recipe.MenuItemId);
        selCmd.Parameters.AddWithValue("@VariantNote", (object?)variantNote ?? DBNull.Value);
        var existingId = await selCmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        if (existingId != null)
        {
            await using var updCmd = conn.CreateTextCommand(
                "UPDATE drink_recipes SET ingredients = @Ingredients, steps = @Steps WHERE id = @Id");
            updCmd.Parameters.AddWithValue("@Ingredients", ingredientsJson);
            updCmd.Parameters.AddWithValue("@Steps", stepsJson);
            updCmd.Parameters.AddWithValue("@Id", existingId);
            await updCmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            recipe.Id = Convert.ToInt32(existingId);
        }
        else
        {
            await using var insCmd = conn.CreateTextCommand(
                "INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps) VALUES (@MenuItemId, @VariantNote, @Ingredients, @Steps) RETURNING id");
            insCmd.Parameters.AddWithValue("@MenuItemId", recipe.MenuItemId);
            insCmd.Parameters.AddWithValue("@VariantNote", (object?)variantNote ?? DBNull.Value);
            insCmd.Parameters.AddWithValue("@Ingredients", ingredientsJson);
            insCmd.Parameters.AddWithValue("@Steps", stepsJson);
            var id = await insCmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
            if (id != null) recipe.Id = Convert.ToInt32(id);
        }
    }

    public async Task DeleteByIdAsync(int recipeId, CancellationToken cancellationToken = default)
    {
        await using var conn = _factory.CreateConnection();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var cmd = conn.CreateTextCommand("DELETE FROM drink_recipes WHERE id = @Id");
        cmd.Parameters.AddWithValue("@Id", recipeId);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static async Task<DrinkRecipe?> ReadRecipeAsync(DbDataReader reader, CancellationToken cancellationToken)
    {
        if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false)) return null;
        return MapRecipe(reader);
    }

    private static DrinkRecipe? MapRecipe(DbDataReader reader)
    {
        var ingredientsJson = reader.IsDBNull(3) ? "[]" : reader.GetString(3);
        var stepsJson = reader.IsDBNull(4) ? "[]" : reader.GetString(4);
        var ingredients = JsonSerializer.Deserialize<List<string>>(ingredientsJson, JsonOptions) ?? new List<string>();
        var steps = JsonSerializer.Deserialize<List<string>>(stepsJson, JsonOptions) ?? new List<string>();
        return new DrinkRecipe
        {
            Id = reader.GetInt32(0),
            MenuItemId = reader.GetInt32(1),
            VariantNote = reader.IsDBNull(2) ? null : reader.GetString(2),
            Ingredients = ingredients,
            Steps = steps
        };
    }
}
