using POS.Core.Models;

namespace POS.Core.Contracts;

public interface IShiftSessionRepository
{
    Task<int> StartShiftAsync(string username, decimal openingCash, CancellationToken cancellationToken = default);
    Task EndShiftAsync(int sessionId, decimal closingCash, CancellationToken cancellationToken = default);
    Task<ShiftSession?> GetCurrentAsync(string username, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ShiftSession>> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default);
}
