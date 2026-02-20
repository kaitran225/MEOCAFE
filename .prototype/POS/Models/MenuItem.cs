namespace POS.Models
{
    public class MenuItem : Item
    {
        public decimal CapitalPrice;
        public bool IsPlaceholder { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public Discount? Discount{ get; set; } 
    }
}
