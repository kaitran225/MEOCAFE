namespace POS.Core.Models;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal SellPrice { get; set; }
    public int Quantity { get; set; }
    public byte[]? Image { get; set; }
}
