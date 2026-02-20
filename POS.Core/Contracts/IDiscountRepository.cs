using POS.Core.Models;

namespace POS.Core.Contracts;

public interface IDiscountRepository
{
    Task<IReadOnlyList<Discount>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Discount?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Discount?> GetByMenuItemIdAsync(int menuItemId, CancellationToken cancellationToken = default);
    Task AddAsync(Discount discount, CancellationToken cancellationToken = default);
    Task UpdateAsync(Discount discount, CancellationToken cancellationToken = default);
    Task DeleteAsync(int discountId, CancellationToken cancellationToken = default);
}
