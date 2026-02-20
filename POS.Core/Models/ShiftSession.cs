namespace POS.Core.Models;

public class ShiftSession
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public decimal OpeningCash { get; set; }
    public decimal? ClosingCash { get; set; }
}
