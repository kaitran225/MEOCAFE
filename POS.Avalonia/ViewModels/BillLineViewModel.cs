using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using POS.Core.Models;

namespace POS.Avalonia.ViewModels;

public partial class BillLineViewModel : ViewModelBase
{
    [ObservableProperty] private string _productName = "";
    [ObservableProperty] private int _quantity;
    [ObservableProperty] private decimal _unitPrice;
    [ObservableProperty] private decimal _lineTotal;
    public int MenuItemId { get; set; }
    public bool IsCombo { get; set; }
    public BillViewModel? ParentBill { get; set; }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CustomizationSummary))]
    private string _iceLevel = "Normal";
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CustomizationSummary))]
    private string _sugarLevel = "Normal";
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CustomizationSummary))]
    private string _size = "M";
    [ObservableProperty] private string _note = "";
    public string CustomizationSummary
    {
        get
        {
            var parts = new[] { IceLevel, SugarLevel, Size }.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            return parts.Count == 0 ? "" : string.Join(", ", parts);
        }
    }

    public void RecalcTotal()
    {
        LineTotal = Quantity * UnitPrice;
        ParentBill?.RecalcTotals();
    }

    [RelayCommand]
    private void IncrementQty()
    {
        Quantity++;
        RecalcTotal();
    }

    [RelayCommand]
    private void DecrementQty()
    {
        if (Quantity > 1) { Quantity--; RecalcTotal(); }
    }
}
