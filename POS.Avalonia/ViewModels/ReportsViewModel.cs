using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Core.Contracts;
using POS.Core.Models;
using System;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class ReportsViewModel : ViewModelBase
{
    private readonly IAnalyticsRepository _analytics;

    [ObservableProperty] private DateTime _dateFrom = DateTime.Today.AddDays(-7);
    [ObservableProperty] private DateTime _dateTo = DateTime.Today;
    [ObservableProperty] private SalesSummaryDto? _summary;
    [ObservableProperty] private bool _isLoading;
    public bool HasSummary => Summary != null;

    public ReportsViewModel(IAnalyticsRepository analytics) => _analytics = analytics;

    [RelayCommand]
    private async Task RunSalesSummaryAsync()
    {
        IsLoading = true;
        try
        {
            var to = DateTo.Date.AddDays(1).AddTicks(-1);
            Summary = await _analytics.GetSalesSummaryAsync(DateFrom.Date, to, default).ConfigureAwait(true);
            OnPropertyChanged(nameof(HasSummary));
        }
        finally
        {
            IsLoading = false;
        }
    }
}
