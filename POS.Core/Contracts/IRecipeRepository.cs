using POS.Core.Models;

namespace POS.Core.Contracts;

public interface IRecipeRepository
{
    Task<DrinkRecipe?> GetByMenuItemIdAsync(int menuItemId, CancellationToken cancellationToken = default);
    Task<DrinkRecipe?> GetByMenuItemAndNoteAsync(int menuItemId, string? note, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DrinkRecipe>> GetAllDefaultsAsync(CancellationToken cancellationToken = default);
    Task AddOrUpdateAsync(DrinkRecipe recipe, CancellationToken cancellationToken = default);
    Task DeleteByIdAsync(int recipeId, CancellationToken cancellationToken = default);
}
