using POS.Core.Models;

namespace POS.Avalonia.Models;

/// <summary>Table + optional elapsed time for table map card.</summary>
public class TableMapCardItem
{
    public Table Table { get; init; } = null!;
    /// <summary>Elapsed time since order at this table (e.g. "12:34"); null if no active order.</summary>
    public string? ElapsedDisplay { get; init; }
}
