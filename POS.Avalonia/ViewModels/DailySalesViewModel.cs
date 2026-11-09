using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class DailySalesViewModel : ViewModelBase
{
    private readonly IAnalyticsRepository _analytics;

    [ObservableProperty] private decimal _todayRevenue;
    [ObservableProperty] private int _todayOrderCount;
    [ObservableProperty] private decimal _todayAvgTicket;
    [ObservableProperty] private ObservableCollection<KeyValuePair<string, decimal>> _topProducts = new();
    [ObservableProperty] private bool _isLoading;
    public bool NotLoading => !IsLoading;
    public bool HasTopProducts => TopProducts.Count > 0;

    public DailySalesViewModel(IAnalyticsRepository analytics)
    {
        _analytics = analytics;
        LoadCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var start = DateTime.Today;
            var end = start.AddDays(1).AddTicks(-1);
            var summary = await _analytics.GetSalesSummaryAsync(start, end, default).ConfigureAwait(true);
            TodayRevenue = summary.TotalRevenue;
            TodayOrderCount = summary.OrderCount;
            TodayAvgTicket = summary.AverageTicket;
            var top = await _analytics.GetTopSellingByWeekAsync(default);
            var list = new ObservableCollection<KeyValuePair<string, decimal>>();
            foreach (var kv in top)
                list.Add(new KeyValuePair<string, decimal>(kv.Key, (decimal)kv.Value));
            TopProducts = list;
            OnPropertyChanged(nameof(NotLoading));
            OnPropertyChanged(nameof(HasTopProducts));
        }
        finally
        {
            IsLoading = false;
            OnPropertyChanged(nameof(NotLoading));
        }
    }
}
