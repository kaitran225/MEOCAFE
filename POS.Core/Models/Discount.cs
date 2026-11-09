namespace POS.Core.Models;

public class Discount
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Percentage { get; set; }
    public bool IsDisabled { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive => !IsDisabled && DateTime.Now >= StartDate && DateTime.Now <= EndDate;
}
