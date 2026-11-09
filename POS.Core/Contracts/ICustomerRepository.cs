using POS.Core.Models;

namespace POS.Core.Contracts;

public interface ICustomerRepository
{
    Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Customer?> GetByPhoneAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Customer>> SearchAsync(string? name, string? phone, CancellationToken cancellationToken = default);
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
    Task DeleteByPhoneAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task AddPointsAsync(string phoneNumber, decimal points, CancellationToken cancellationToken = default);
}
