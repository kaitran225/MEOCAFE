using POS.Core.Models;

namespace POS.Core.Contracts;

public interface IProblemReportRepository
{
    Task<IEnumerable<ProblemReport>> GetAllAsync(CancellationToken ct);
    Task<long> InsertAsync(ProblemReport report, CancellationToken ct);
    Task UpdateStatusAsync(long id, string status, string? resolution, CancellationToken ct);
}
