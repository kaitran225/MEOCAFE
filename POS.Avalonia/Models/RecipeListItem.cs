using System;
using System.Collections.Generic;
using System.Linq;
using POS.Core.Models;

namespace POS.Avalonia.Models;

/// <summary>Default recipe + menu item name for the Recipes tab list.</summary>
public sealed class RecipeListItem
{
    public DrinkRecipe Recipe { get; set; } = new();
    public string MenuItemName { get; set; } = "";
    public string? CategoryName { get; set; }
    public int CategoryId { get; set; }
    public byte[]? ImageBytes { get; set; }

    /// <summary>Steps with 1-based numbers for timeline display.</summary>
    public IReadOnlyList<StepEntry> StepsWithNumber =>
        (Recipe.Steps ?? new List<string>())
            .Select((s, i) => new StepEntry
            {
                Number = i + 1,
                Text = s ?? "",
                Warning = ExtractWarning(s)
            })
            .ToList();

    private static string? ExtractWarning(string? step)
    {
        if (string.IsNullOrWhiteSpace(step)) return null;
        var idx = step.IndexOf("không đổ chung", StringComparison.OrdinalIgnoreCase);
        if (idx < 0) idx = step.IndexOf("khong do chung", StringComparison.OrdinalIgnoreCase);
        if (idx < 0) return null;
        var end = step.IndexOfAny(new[] { '.', '!', '\n', '\r' }, idx);
        var text = (end > idx ? step.Substring(idx, end - idx) : step.Substring(idx)).Trim();
        return text.Length == 0 ? null : char.ToUpperInvariant(text[0]) + text.Substring(1) + ".";
    }
}
