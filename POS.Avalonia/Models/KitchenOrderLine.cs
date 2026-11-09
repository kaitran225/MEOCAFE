using System.Collections.Generic;

namespace POS.Avalonia.Models;

public sealed class KitchenOrderLine
{
    public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public string? OptionsSummary { get; set; }
    /// <summary>Individual option tags for chip display.</summary>
    public List<string> OptionTags { get; set; } = new();
}
