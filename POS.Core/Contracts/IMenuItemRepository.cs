using POS.Core.Models;

namespace POS.Core.Contracts;

public interface IMenuItemRepository
{
    Task<IReadOnlyList<MenuItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<MenuItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(MenuItem menuItem, CancellationToken cancellationToken = default);
    Task UpdateAsync(MenuItem menuItem, CancellationToken cancellationToken = default);
    Task DeleteAsync(int menuItemId, CancellationToken cancellationToken = default);
    Task DecreaseQuantityAsync(int menuItemId, int amount, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MenuItem>> GetLowStockAsync(CancellationToken cancellationToken = default);
}
