namespace POS.Core.Models;

public class Refund
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = "Cash";
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
}
