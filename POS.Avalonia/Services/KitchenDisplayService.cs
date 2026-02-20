using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using POS.Avalonia.Models;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Avalonia.Services;

public sealed class KitchenDisplayService : IKitchenDisplayService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IOrderDetailRepository _detailRepo;
    private readonly IOrderDetailOptionRepository _optionRepo;
    private readonly IMenuItemRepository _menuItemRepo;

    public KitchenDisplayService(
        IOrderRepository orderRepo,
        IOrderDetailRepository detailRepo,
        IOrderDetailOptionRepository optionRepo,
        IMenuItemRepository menuItemRepo)
    {
        _orderRepo = orderRepo;
        _detailRepo = detailRepo;
        _optionRepo = optionRepo;
        _menuItemRepo = menuItemRepo;
    }

    public async Task<IReadOnlyList<KitchenOrderDisplay>> GetActiveOrdersAsync(string? statusFilter = null, CancellationToken cancellationToken = default)
    {
        var statuses = string.IsNullOrEmpty(statusFilter) || statusFilter == "active"
            ? new[] { "sent", "in_progress" }
            : new[] { statusFilter };
        var orders = new List<Order>();
        foreach (var s in statuses)
        {
            var list = await _orderRepo.GetByKitchenStatusAsync(s, cancellationToken).ConfigureAwait(false);
            orders.AddRange(list);
        }
        orders.Sort((a, b) => a.CreatedAt.CompareTo(b.CreatedAt));
        var result = new List<KitchenOrderDisplay>();
        foreach (var order in orders)
        {
            var details = await _detailRepo.GetByOrderIdAsync(order.Id, cancellationToken).ConfigureAwait(false);
            var lines = new List<KitchenOrderLine>();
            foreach (var d in details)
            {
                var menuItem = await _menuItemRepo.GetByIdAsync(d.MenuItemId, cancellationToken).ConfigureAwait(false);
                var opts = await _optionRepo.GetByOrderDetailIdAsync(d.Id, cancellationToken).ConfigureAwait(false);
                var parts = opts.Select(o => $"{o.OptionType}: {o.Note ?? ""}").Where(s => !string.IsNullOrWhiteSpace(s.TrimEnd(':'))).ToList();
                lines.Add(new KitchenOrderLine
                {
                    ProductName = menuItem?.Name ?? $"#{d.MenuItemId}",
                    Quantity = d.Quantity,
                    OptionsSummary = parts.Count == 0 ? null : string.Join(", ", parts)
                });
            }
            result.Add(new KitchenOrderDisplay
            {
                OrderId = order.Id,
                CreatedAt = order.CreatedAt,
                Status = order.KitchenStatus,
                Lines = lines
            });
        }
        return result;
    }

    public async Task<KitchenOrderDisplay?> GetOrderDisplayAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepo.GetByIdAsync(orderId, cancellationToken).ConfigureAwait(false);
        if (order == null) return null;
        var details = await _detailRepo.GetByOrderIdAsync(orderId, cancellationToken).ConfigureAwait(false);
        var lines = new List<KitchenOrderLine>();
        foreach (var d in details)
        {
            var menuItem = await _menuItemRepo.GetByIdAsync(d.MenuItemId, cancellationToken).ConfigureAwait(false);
            var opts = await _optionRepo.GetByOrderDetailIdAsync(d.Id, cancellationToken).ConfigureAwait(false);
            var parts = opts.Select(o => $"{o.OptionType}: {o.Note ?? ""}").Where(s => !string.IsNullOrWhiteSpace(s.TrimEnd(':'))).ToList();
            lines.Add(new KitchenOrderLine
            {
                ProductName = menuItem?.Name ?? $"#{d.MenuItemId}",
                Quantity = d.Quantity,
                OptionsSummary = parts.Count == 0 ? null : string.Join(", ", parts)
            });
        }
        return new KitchenOrderDisplay { OrderId = order.Id, CreatedAt = order.CreatedAt, Status = order.KitchenStatus, Lines = lines };
    }

    public string FormatKitchenOrderAsText(KitchenOrderDisplay order)
    {
        var lines = new List<string>
        {
            "-------- KITCHEN --------",
            $"Order #{order.OrderId}  {order.CreatedAt:g}",
            $"Status: {order.Status}",
            "------------------------"
        };
        foreach (var line in order.Lines)
        {
            lines.Add($"{line.Quantity}x {line.ProductName}");
            if (!string.IsNullOrWhiteSpace(line.OptionsSummary)) lines.Add("   " + line.OptionsSummary);
        }
        lines.Add("------------------------");
        return string.Join(System.Environment.NewLine, lines);
    }
}
