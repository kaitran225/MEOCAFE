using System;
using System.Collections.Generic;

namespace POS.Avalonia.Models;

public sealed class KitchenOrderDisplay
{
    public int OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "";
    public List<KitchenOrderLine> Lines { get; set; } = new();
}
