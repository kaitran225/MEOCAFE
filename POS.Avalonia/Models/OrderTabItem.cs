using POS.Core.Models;

namespace POS.Avalonia.Models;

/// <summary>Tab in order view: either "Recipes" or a category.</summary>
public sealed class OrderTabItem
{
    public bool IsRecipesTab { get; set; }
    public Category? Category { get; set; }
    public string DisplayName => IsRecipesTab ? "Recipes" : (Category?.Name ?? "");
}
