using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using POS.Avalonia.Models;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Avalonia.Services;

public sealed class ReceiptService : IReceiptService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IOrderDetailRepository _detailRepo;
    private readonly IOrderDetailOptionRepository _optionRepo;
    private readonly IMenuItemRepository _menuItemRepo;
    private readonly IProductOptionRepository _productOptionRepo;
    private readonly IConfiguration _config;

    public ReceiptService(
        IOrderRepository orderRepo,
        IOrderDetailRepository detailRepo,
        IOrderDetailOptionRepository optionRepo,
        IMenuItemRepository menuItemRepo,
        IProductOptionRepository productOptionRepo,
        IConfiguration config)
    {
        _orderRepo = orderRepo;
        _detailRepo = detailRepo;
        _optionRepo = optionRepo;
        _menuItemRepo = menuItemRepo;
        _productOptionRepo = productOptionRepo;
        _config = config;
    }

    public async Task<ReceiptData> BuildReceiptFromOrderAsync(int orderId, string paymentMethod = "Cash", CancellationToken cancellationToken = default)
    {
        var order = await _orderRepo.GetByIdAsync(orderId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"Order {orderId} not found.");
        var details = await _detailRepo.GetByOrderIdAsync(orderId, cancellationToken).ConfigureAwait(false);

        var receipt = new ReceiptData
        {
            OrderId = orderId,
            Header = new ReceiptHeader
            {
                BusinessName = _config["Receipt:BusinessName"] ?? "MEOCAFE",
                Address = _config["Receipt:Address"] ?? "",
                TaxId = _config["Receipt:TaxId"]
            },
            Footer = new ReceiptFooter
            {
                ThankYouText = _config["Receipt:ThankYou"] ?? "Thank you!",
                TaxId = _config["Receipt:TaxId"]
            }
        };

        decimal subtotal = 0;
        foreach (var d in details)
        {
            var menuItem = await _menuItemRepo.GetByIdAsync(d.MenuItemId, cancellationToken).ConfigureAwait(false);
            var options = await _optionRepo.GetByOrderDetailIdAsync(d.Id, cancellationToken).ConfigureAwait(false);
            var optionParts = new List<string>();
            foreach (var opt in options)
            {
                var part = opt.OptionType;
                if (opt.OptionValueId.HasValue)
                {
                    var val = await _productOptionRepo.GetOptionValueByIdAsync(opt.OptionValueId.Value, cancellationToken).ConfigureAwait(false);
                    if (val != null) part = $"{opt.OptionType}: {val.Name}";
                }
                if (!string.IsNullOrWhiteSpace(opt.Note)) part += $" ({opt.Note})";
                optionParts.Add(part);
            }
            var lineTotal = d.Quantity * d.Price;
            subtotal += lineTotal;
            receipt.LineItems.Add(new ReceiptLineItem
            {
                ProductName = menuItem?.Name ?? $"Item #{d.MenuItemId}",
                Quantity = d.Quantity,
                UnitPrice = d.Price,
                LineTotal = lineTotal,
                OptionsSummary = optionParts.Count == 0 ? null : string.Join(", ", optionParts)
            });
        }

        var taxRate = decimal.TryParse(_config["Tax:Rate"], out var tr) ? tr : 0.1m;
        var taxAmount = Math.Round((subtotal * taxRate), 2);
        receipt.Totals = new ReceiptTotals
        {
            Subtotal = subtotal,
            DiscountAmount = 0,
            TaxAmount = taxAmount,
            GrandTotal = order.TotalPrice,
            PaymentMethod = paymentMethod
        };
        return receipt;
    }

    public string FormatReceiptAsText(ReceiptData data)
    {
        var lines = new List<string>();
        lines.Add(data.Header.BusinessName);
        if (!string.IsNullOrWhiteSpace(data.Header.Address)) lines.Add(data.Header.Address);
        if (!string.IsNullOrWhiteSpace(data.Header.TaxId)) lines.Add("Tax ID: " + data.Header.TaxId);
        lines.Add("--------------------------------");
        foreach (var line in data.LineItems)
        {
            lines.Add($"{line.ProductName} x{line.Quantity} @ {line.UnitPrice:N2} = {line.LineTotal:N2}");
            if (!string.IsNullOrWhiteSpace(line.OptionsSummary)) lines.Add("  " + line.OptionsSummary);
        }
        lines.Add("--------------------------------");
        lines.Add($"Subtotal:   {data.Totals.Subtotal:N2}");
        lines.Add($"Discount:   {data.Totals.DiscountAmount:N2}");
        lines.Add($"Tax:        {data.Totals.TaxAmount:N2}");
        lines.Add($"Total:      {data.Totals.GrandTotal:N2}");
        lines.Add($"Payment:    {data.Totals.PaymentMethod}");
        lines.Add("--------------------------------");
        if (data.OrderId.HasValue) lines.Add("Order #" + data.OrderId.Value);
        lines.Add(data.Footer.ThankYouText);
        if (!string.IsNullOrWhiteSpace(data.Footer.TaxId)) lines.Add("Tax ID: " + data.Footer.TaxId);
        return string.Join(Environment.NewLine, lines);
    }
}
