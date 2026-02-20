namespace POS.Core.Models;

public class Employee
{
    public int Id { get; set; }
    public string Fullname { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public string? Dob { get; set; }
    public string Role { get; set; } = string.Empty;
}
