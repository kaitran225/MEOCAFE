namespace POS.Core.Models;

public class IngredientWaste
{
    public long Id { get; set; }
    public string ItemName { get; set; } = "";
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = "";
    public string Reason { get; set; } = "";
    public string LoggedBy { get; set; } = "";
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}
