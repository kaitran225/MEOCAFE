namespace POS.Avalonia.Models;

public sealed class KitchenOrderLine
{
    public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public string? OptionsSummary { get; set; }
}
