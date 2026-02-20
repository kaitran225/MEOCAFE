using POS.Core.Models;

namespace POS.Core.Contracts;

public interface IOrderRepository
{
    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetByKitchenStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> AddAsync(Order order, CancellationToken cancellationToken = default);
    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
    Task DeleteAsync(int orderId, CancellationToken cancellationToken = default);
    Task UpdateKitchenStatusAsync(int orderId, string status, CancellationToken cancellationToken = default);
}
