using POS.Core.Models;

namespace POS.Core.Contracts;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLogEntry entry, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditLogEntry>> GetAsync(DateTime? from, DateTime? to, string? username, string? action, CancellationToken cancellationToken = default);
}
