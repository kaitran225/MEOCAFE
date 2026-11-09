using POS.Core.Models;

namespace POS.Core.Contracts;

public interface IOrderDetailOptionRepository
{
    Task<IReadOnlyList<OrderDetailOption>> GetByOrderDetailIdAsync(int orderDetailId, CancellationToken cancellationToken = default);
    Task AddAsync(OrderDetailOption option, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<OrderDetailOption> options, CancellationToken cancellationToken = default);
}
