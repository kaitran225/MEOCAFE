namespace POS.Core.Models;

public class DrinkRecipe
{
    public int Id { get; set; }
    public int MenuItemId { get; set; }
    /// <summary>Null or empty = default recipe for the drink.</summary>
    public string? VariantNote { get; set; }
    public List<string> Ingredients { get; set; } = new();
    public List<string> Steps { get; set; } = new();
}
