using System;
using System.Collections.Generic;

namespace POS.Avalonia.Models;

public sealed class KitchenOrderDisplay
{
    public int OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "";
    public string? TableName { get; set; }
    public string ServiceType { get; set; } = "Dine-in";
    public string? ServerName { get; set; }
    /// <summary>Elapsed time since creation, e.g. "02:30".</summary>
    public string ElapsedDisplay { get; set; } = "";
    public bool IsLate { get; set; }
    public List<KitchenOrderLine> Lines { get; set; } = new();
}
