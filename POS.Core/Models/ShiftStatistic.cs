namespace POS.Core.Models;

public class ShiftStatistic
{
    public string EmployeesUsername { get; set; } = string.Empty;
    public int Total { get; set; }
    public int Late { get; set; }
    public int OnTime { get; set; }
}
