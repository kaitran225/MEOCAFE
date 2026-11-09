namespace POS.Core.Models;

public class ProblemReport
{
    public long Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public string Priority { get; set; } = "Medium";
    public string Status { get; set; } = "Open";
    public string ReportedBy { get; set; } = "";
    public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
    public string? Resolution { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
