using System.Collections.Generic;

namespace POS.Avalonia.Models;

/// <summary>Data model for receipt: header, lines, totals, footer (8.2).</summary>
public sealed class ReceiptData
{
    /// <summary>Order ID for reprint reference and optional QR/barcode (8.7).</summary>
    public int? OrderId { get; set; }
    public ReceiptHeader Header { get; set; } = new();
    public List<ReceiptLineItem> LineItems { get; set; } = new();
    public ReceiptTotals Totals { get; set; } = new();
    public ReceiptFooter Footer { get; set; } = new();
}

public sealed class ReceiptHeader
{
    public string BusinessName { get; set; } = "";
    public string Address { get; set; } = "";
    public string? TaxId { get; set; }
    public byte[]? Logo { get; set; }
}

public sealed class ReceiptLineItem
{
    public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public string? OptionsSummary { get; set; }
}

public sealed class ReceiptTotals
{
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public string PaymentMethod { get; set; } = "Cash";
}

public sealed class ReceiptFooter
{
    public string ThankYouText { get; set; } = "Thank you!";
    public string? TaxId { get; set; }
}
