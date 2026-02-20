namespace POS.Core.Models;

/// <summary>Option type: Ice, Sugar, Size, Topping.</summary>
public class ProductOptionGroup
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string OptionType { get; set; } = string.Empty; // Ice, Sugar, Size, Topping
    public List<ProductOptionValue> Values { get; set; } = new();
}
