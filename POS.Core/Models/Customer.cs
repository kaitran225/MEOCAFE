namespace POS.Core.Models;

public class Customer
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public int Point { get; set; }
}
