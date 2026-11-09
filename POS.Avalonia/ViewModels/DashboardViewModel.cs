using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Core.Contracts;
using POS.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly IAnalyticsRepository _analytics;
    private readonly IMenuItemRepository _menuItemRepo;

    [ObservableProperty] private decimal _totalSales;
    [ObservableProperty] private decimal _averageTicket;
    [ObservableProperty] private ObservableCollection<KeyValuePair<string, decimal>> _topProducts = new();
    [ObservableProperty] private ObservableCollection<MenuItem> _lowStockItems = new();
    [ObservableProperty] private bool _isLoading;
    public bool NotLoading => !IsLoading;
    public bool TopProductsEmpty => TopProducts.Count == 0;
    public bool LowStockEmpty => LowStockItems.Count == 0;
    public int LowStockCount => LowStockItems.Count;

    public string Title => "Dashboard";

    public DashboardViewModel(IAnalyticsRepository analytics, IMenuItemRepository menuItemRepo)
    {
        _analytics = analytics;
        _menuItemRepo = menuItemRepo;
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            TotalSales = await _analytics.GetTotalSalesAsync(default).ConfigureAwait(true);
            AverageTicket = await _analytics.GetAverageRevenuePerTransactionAsync(default).ConfigureAwait(true);
            var top = await _analytics.GetTopSellingByWeekAsync(default).ConfigureAwait(true);
            var list = new ObservableCollection<KeyValuePair<string, decimal>>();
            foreach (var kv in top)
                list.Add(new KeyValuePair<string, decimal>(kv.Key, (decimal)kv.Value));
            TopProducts = list;
            var low = await _menuItemRepo.GetLowStockAsync(default).ConfigureAwait(true);
            LowStockItems = new ObservableCollection<MenuItem>(low ?? Enumerable.Empty<MenuItem>());
            OnPropertyChanged(nameof(TopProductsEmpty));
            OnPropertyChanged(nameof(LowStockEmpty));
            OnPropertyChanged(nameof(LowStockCount));
        }
        finally
        {
            IsLoading = false;
            OnPropertyChanged(nameof(NotLoading));
        }
    }
}
