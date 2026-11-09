namespace POS.Avalonia.ViewModels;

public sealed class PaymentEntryViewModel
{
    public string Method { get; set; } = "Cash";
    public decimal Amount { get; set; }
}
