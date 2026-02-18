namespace POS.Models
{
    public class Discount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description;
        public decimal Percentage { get; set; }
        public bool IsDisabled;
        public DateTime StartDate;
        public DateTime EndDate;

        public bool IsActive => !IsDisabled && DateTime.Now >= StartDate && DateTime.Now <= EndDate;
    }
}
