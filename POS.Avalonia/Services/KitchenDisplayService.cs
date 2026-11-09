using POS.Avalonia.Models;
using POS.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace POS.Avalonia.Services;

/// <summary>
/// Builds the kitchen ticket view of an order: what the kitchen prints, not a customer receipt.
/// </summary>
public sealed class KitchenDisplayService : IKitchenDisplayService
{
    /// <summary>An order waiting longer than this is flagged as late on the ticket.</summary>
    private static readonly TimeSpan LateAfter = TimeSpan.FromMinutes(15);

    private readonly IOrderRepository _orderRepo;
    private readonly IOrderDetailRepository _detailRepo;
    private readonly IMenuItemRepository _menuItemRepo;

    public KitchenDisplayService(
        IOrderRepository orderRepo,
        IOrderDetailRepository detailRepo,
        IMenuItemRepository menuItemRepo)
    {
        _orderRepo = orderRepo;
        _detailRepo = detailRepo;
        _menuItemRepo = menuItemRepo;
    }

    public async Task<KitchenOrderDisplay?> GetOrderDisplayAsync(int orderId, CancellationToken cancellationToken)
    {
        var order = await _orderRepo.GetByIdAsync(orderId, cancellationToken).ConfigureAwait(false);
        if (order is null) return null;

        var details = await _detailRepo.GetByOrderIdAsync(orderId, cancellationToken).ConfigureAwait(false);
        var elapsed = DateTime.Now - order.CreatedAt;
        if (elapsed < TimeSpan.Zero) elapsed = TimeSpan.Zero;

        var display = new KitchenOrderDisplay
        {
            OrderId = order.Id,
            CreatedAt = order.CreatedAt,
            Status = order.KitchenStatus,
            ServiceType = order.ServiceType,
            TableName = order.TableId.HasValue ? "Bàn " + order.TableId.Value : null,
            ElapsedDisplay = $"{(int)elapsed.TotalMinutes:00}:{elapsed.Seconds:00}",
            IsLate = elapsed > LateAfter && !string.Equals(order.KitchenStatus, "done", StringComparison.OrdinalIgnoreCase)
        };

        foreach (var detail in details)
        {
            var menuItem = await _menuItemRepo.GetByIdAsync(detail.MenuItemId, cancellationToken).ConfigureAwait(false);
            display.Lines.Add(new KitchenOrderLine
            {
                ProductName = menuItem?.Name ?? $"Món #{detail.MenuItemId}",
                Quantity = detail.Quantity
            });
        }

        return display;
    }

    public string FormatKitchenOrderAsText(KitchenOrderDisplay display)
    {
        var sb = new StringBuilder();
        sb.AppendLine("PHIẾU BẾP #" + display.OrderId);
        if (!string.IsNullOrWhiteSpace(display.TableName)) sb.AppendLine(display.TableName);
        sb.AppendLine(display.ServiceType + " · " + display.CreatedAt.ToString("HH:mm", CultureInfo.InvariantCulture));
        if (display.IsLate) sb.AppendLine("TRỄ · đã chờ " + display.ElapsedDisplay);
        sb.AppendLine("--------------------------------");

        foreach (var line in display.Lines)
        {
            sb.AppendLine($"{line.Quantity} x {line.ProductName}");
            if (!string.IsNullOrWhiteSpace(line.OptionsSummary)) sb.AppendLine("   " + line.OptionsSummary);
        }

        sb.AppendLine("--------------------------------");
        sb.AppendLine(display.Lines.Sum(l => l.Quantity) + " phần");
        return sb.ToString().TrimEnd();
    }
}
