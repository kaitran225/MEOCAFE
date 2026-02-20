namespace POS.Core.Models;

public class Table
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; } = 2;
    public string? Zone { get; set; }
    public string Status { get; set; } = "empty";
}
