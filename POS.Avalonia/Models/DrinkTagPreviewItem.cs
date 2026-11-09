using System;
using System.Collections.Generic;

namespace POS.Avalonia.Models;

/// <summary>One drink tag for preview and print (one per drink unit).</summary>
public sealed class DrinkTagPreviewItem
{
    public string ProductName { get; set; } = "";
    public string Size { get; set; } = "";
    public string Detail { get; set; } = "";
    public string Note { get; set; } = "";

    public string ToTagText()
    {
        var lines = new List<string> { "--- Drink ---", ProductName, "Size: " + Size };
        if (!string.IsNullOrWhiteSpace(Detail)) lines.Add(Detail);
        if (!string.IsNullOrWhiteSpace(Note)) lines.Add(Note);
        return string.Join(Environment.NewLine, lines);
    }
}
