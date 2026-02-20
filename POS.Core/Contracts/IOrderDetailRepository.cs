using POS.Core.Models;

namespace POS.Core.Contracts;

public interface IOrderDetailRepository
{
    Task<IReadOnlyList<OrderDetail>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);
    Task AddAsync(OrderDetail detail, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<OrderDetail> details, CancellationToken cancellationToken = default);
}
