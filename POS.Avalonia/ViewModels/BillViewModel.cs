using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace POS.Avalonia.ViewModels;

public partial class BillViewModel : ViewModelBase
{
    public string Label { get; set; } = "";
    public ObservableCollection<BillLineViewModel> Lines { get; } = new();

    [ObservableProperty] private decimal _subtotal;
    [ObservableProperty] private decimal _discountAmount;
    [ObservableProperty] private decimal _taxAmount;
    [ObservableProperty] private decimal _grandTotal;
    public decimal TaxRate { get; set; } = 0.1m;
    public int PointsRedeemed { get; set; }

    public string ServiceType { get; set; } = "Dine-in";
    public int? TableId { get; set; }
    public string TableName { get; set; } = "";
    public string? DeliveryAddress { get; set; }
    public decimal DeliveryFee { get; set; }

    public void RecalcTotals()
    {
        Subtotal = 0;
        foreach (var line in Lines)
            Subtotal += line.LineTotal;
        TaxAmount = Math.Round((Subtotal - DiscountAmount) * TaxRate, 2);
        GrandTotal = Subtotal - DiscountAmount + TaxAmount + DeliveryFee;
    }
}
