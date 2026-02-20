using System.Threading;
using System.Threading.Tasks;

namespace POS.Avalonia.Services;

public interface IAuditLogger
{
    Task LogAsync(string action, string? entityType = null, int? entityId = null, string? details = null, CancellationToken cancellationToken = default);
}
