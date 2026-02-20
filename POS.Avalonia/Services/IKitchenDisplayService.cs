using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using POS.Avalonia.Models;

namespace POS.Avalonia.Services;

public interface IKitchenDisplayService
{
    /// <summary>Get orders for kitchen display. statusFilter: null = all active (sent + in_progress), "sent", "in_progress", or "done".</summary>
    Task<IReadOnlyList<KitchenOrderDisplay>> GetActiveOrdersAsync(string? statusFilter = null, CancellationToken cancellationToken = default);
    /// <summary>Get single order display for reprint (8.17).</summary>
    Task<KitchenOrderDisplay?> GetOrderDisplayAsync(int orderId, CancellationToken cancellationToken = default);
    /// <summary>Format kitchen order as text for reprint (8.17).</summary>
    string FormatKitchenOrderAsText(KitchenOrderDisplay order);
}
