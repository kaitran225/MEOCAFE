namespace POS.Core.Models;

public class Order
{
    public int Id { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal Percentage { get; set; }
    public decimal GrandTotal { get; set; }
    public string? Note { get; set; }
    public string? PhoneNumber { get; set; }
    public int? CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    /// <summary>Kitchen display: pending, sent, done.</summary>
    public string KitchenStatus { get; set; } = "pending";
    public int? TableId { get; set; }
    public string ServiceType { get; set; } = "Dine-in";
    public string? DeliveryAddress { get; set; }
    public decimal DeliveryFee { get; set; }
    public List<OrderDetail> OrderDetails { get; set; } = new();
}
