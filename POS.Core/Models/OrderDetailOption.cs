namespace POS.Core.Models;

/// <summary>Selected option for an order line (ice, sugar, topping ids, note).</summary>
public class OrderDetailOption
{
    public int Id { get; set; }
    public int OrderDetailId { get; set; }
    public string OptionType { get; set; } = string.Empty;
    public int? OptionValueId { get; set; }
    public string? Note { get; set; }
}
