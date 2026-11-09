namespace POS.Core.Models;

public class MenuItem : Item
{
    public decimal CapitalPrice { get; set; }
    public bool IsPlaceholder { get; set; }
    public int CategoryId { get; set; }
    public int ReorderLevel { get; set; }
    public Category? Category { get; set; }
    public Discount? Discount { get; set; }
}
