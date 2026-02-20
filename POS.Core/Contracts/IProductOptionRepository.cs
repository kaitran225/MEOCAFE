using POS.Core.Models;

namespace POS.Core.Contracts;

public interface IProductOptionRepository
{
    Task<IReadOnlyList<ProductOptionGroup>> GetOptionGroupsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductOptionGroup>> GetOptionGroupsForProductAsync(int menuItemId, CancellationToken cancellationToken = default);
    Task<ProductOptionValue?> GetOptionValueByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddOptionGroupAsync(ProductOptionGroup group, CancellationToken cancellationToken = default);
    Task AddOptionValueAsync(ProductOptionValue value, CancellationToken cancellationToken = default);
}
