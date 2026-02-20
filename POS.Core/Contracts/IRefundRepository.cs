using POS.Core.Models;

namespace POS.Core.Contracts;

public interface IRefundRepository
{
    Task<int> AddAsync(Refund refund, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Refund>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);
}
