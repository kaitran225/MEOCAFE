using System.Threading;
using System.Threading.Tasks;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Avalonia.Services;

public sealed class AuditLogger : IAuditLogger
{
    private readonly IAuditLogRepository _repo;
    private readonly CurrentUserService _currentUser;

    public AuditLogger(IAuditLogRepository repo, CurrentUserService currentUser)
    {
        _repo = repo;
        _currentUser = currentUser;
    }

    public async Task LogAsync(string action, string? entityType = null, int? entityId = null, string? details = null, CancellationToken cancellationToken = default)
    {
        var user = _currentUser.Current?.Username ?? "system";
        await _repo.AddAsync(new AuditLogEntry { Username = user, Action = action, EntityType = entityType, EntityId = entityId, Details = details }, cancellationToken).ConfigureAwait(false);
    }
}
