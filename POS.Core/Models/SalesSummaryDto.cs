namespace POS.Core.Models;

public class SalesSummaryDto
{
    public decimal TotalRevenue { get; set; }
    public int OrderCount { get; set; }
    public decimal AverageTicket { get; set; }
}
