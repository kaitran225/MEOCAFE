namespace POS.Avalonia.Models;

/// <summary>One row of the dashboard "top products" table. Widths are pre-computed so the
/// view needs no converters (see .rules/design.md: no converters for theme colors/layout math).</summary>
public sealed class TopProductRow
{
    public int Rank { get; init; }
    public string Name { get; init; } = "";
    public decimal Revenue { get; init; }

    /// <summary>Bar width in px inside the 84px track.</summary>
    public double ShareBarWidth { get; init; }
}

/// <summary>One bar of the dashboard "peak hours" chart, already sized in px.</summary>
public sealed class HourlySalesBar
{
    public string Label { get; init; } = "";
    public string Tooltip { get; init; } = "";
    public decimal Revenue { get; init; }
    public double BarHeight { get; init; }
    public bool IsPeak { get; init; }
}
