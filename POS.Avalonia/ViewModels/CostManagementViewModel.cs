using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class CostManagementViewModel : ViewModelBase
{
    [ObservableProperty] private decimal _totalCost;
    [ObservableProperty] private decimal _totalRevenue;
    [ObservableProperty] private decimal _profitMargin;
    [ObservableProperty] private ObservableCollection<CostItem> _costItems = new();
    [ObservableProperty] private bool _isLoading;
    public bool NotLoading => !IsLoading;
    public bool HasItems => CostItems.Count > 0;
    public int ItemsTracked => CostItems.Count;

    public CostManagementViewModel()
    {
        LoadCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            await Task.Delay(200);

            var items = new ObservableCollection<CostItem>
            {
                new("Arabica Coffee Beans", "Raw Materials", 180_000, 450_000, 60.0m),
                new("Robusta Coffee Beans", "Raw Materials", 120_000, 350_000, 65.7m),
                new("Fresh Milk", "Ingredients", 45_000, 120_000, 62.5m),
                new("Matcha Powder", "Ingredients", 250_000, 580_000, 56.9m),
                new("Syrup (Vanilla)", "Ingredients", 85_000, 200_000, 57.5m),
                new("Plastic Cups", "Packaging", 15_000, 25_000, 40.0m),
                new("Paper Straws", "Packaging", 8_000, 12_000, 33.3m),
                new("Sugar", "Ingredients", 20_000, 80_000, 75.0m),
                new("Ice", "Utilities", 10_000, 35_000, 71.4m),
                new("Condensed Milk", "Ingredients", 35_000, 95_000, 63.2m),
            };

            CostItems = items;

            decimal cost = 0, rev = 0;
            foreach (var item in items)
            {
                cost += item.Cost;
                rev += item.Revenue;
            }
            TotalCost = cost;
            TotalRevenue = rev;
            ProfitMargin = rev > 0 ? (rev - cost) / rev * 100 : 0;

            OnPropertyChanged(nameof(NotLoading));
            OnPropertyChanged(nameof(HasItems));
            OnPropertyChanged(nameof(ItemsTracked));
        }
        finally
        {
            IsLoading = false;
            OnPropertyChanged(nameof(NotLoading));
        }
    }
}

public class CostItem
{
    public string Name { get; }
    public string Category { get; }
    public decimal Cost { get; }
    public decimal Revenue { get; }
    public decimal Margin { get; }

    public CostItem(string name, string category, decimal cost, decimal revenue, decimal margin)
    {
        Name = name;
        Category = category;
        Cost = cost;
        Revenue = revenue;
        Margin = margin;
    }
}
