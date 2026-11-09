using POS.Core.Models;

namespace POS.Core.Contracts;

public interface IComboRepository
{
    Task<IReadOnlyList<ComboMenuItem>> GetComboMenuItemsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ComboItem>> GetComboItemsByComboIdAsync(int comboId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Item>> GetComboItemsAsync(int comboId, CancellationToken cancellationToken = default);
    Task AddComboMenuItemAsync(ComboMenuItem combo, CancellationToken cancellationToken = default);
    Task UpdateComboMenuItemAsync(ComboMenuItem combo, CancellationToken cancellationToken = default);
    Task DeleteComboMenuItemAsync(int comboId, CancellationToken cancellationToken = default);
    Task AddComboItemAsync(ComboItem item, CancellationToken cancellationToken = default);
    Task DeleteComboItemAsync(int comboId, int menuItemId, CancellationToken cancellationToken = default);
}
