using POS.Core.Models;

namespace POS.Core.Contracts;

public interface ITableRepository
{
    Task<IReadOnlyList<Table>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Table?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Table table, CancellationToken cancellationToken = default);
    Task UpdateAsync(Table table, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(int id, string status, CancellationToken cancellationToken = default);
}
