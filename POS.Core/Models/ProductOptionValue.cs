namespace POS.Core.Models;

public class ProductOptionValue
{
    public int Id { get; set; }
    public int OptionGroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal? ExtraPrice { get; set; }
}
