using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Localization;
using POS.Avalonia.Localization;
using POS.Avalonia.Models;
using POS.Avalonia.Services;
using POS.Core.Contracts;
using POS.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

/// <summary>
/// Dashboard: KPIs, best sellers, peak hours and low stock.
/// Every value is loaded from the repositories — no placeholder "—" tiles, no hardcoded bars.
/// </summary>
public partial class DashboardViewModel : ViewModelBase
{
    private const double MinBarHeight = 6;
    private const double MaxBarHeight = 104;
    private const double ShareTrackWidth = 84;
    private const int ChartFirstHour = 6;      // 06:00
    private const int ChartBucketHours = 2;
    private const int ChartBucketCount = 8;    // 06:00 → 22:00

    private readonly IAnalyticsRepository _analytics;
    private readonly IMenuItemRepository _menuItemRepo;
    private readonly IOrderRepository _orderRepo;
    private readonly IAppNavigator _navigator;
    private readonly IStringLocalizer<StringsResource> _loc;

    [ObservableProperty] private decimal _todayRevenue;
    [ObservableProperty] private int _todayOrders;
    [ObservableProperty] private decimal _averageTicket;
    [ObservableProperty] private int _openOrders;
    [ObservableProperty] private bool _isLoading;

    public ObservableCollection<TopProductRow> TopProducts { get; } = new();
    public ObservableCollection<HourlySalesBar> HourlySales { get; } = new();
    public ObservableCollection<MenuItem> LowStockItems { get; } = new();

    public bool NotLoading => !IsLoading;
    public bool TopProductsEmpty => TopProducts.Count == 0;
    public bool LowStockEmpty => LowStockItems.Count == 0;
    public int LowStockCount => LowStockItems.Count;
    public bool HourlySalesEmpty => HourlySales.Count == 0 || HourlySales.All(b => b.Revenue <= 0);
    public string TodayCaption => "Hôm nay, " + DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

    public DashboardViewModel(
        IAnalyticsRepository analytics,
        IMenuItemRepository menuItemRepo,
        IOrderRepository orderRepo,
        IAppNavigator navigator,
        IStringLocalizer<StringsResource> loc)
    {
        _analytics = analytics;
        _menuItemRepo = menuItemRepo;
        _orderRepo = orderRepo;
        _navigator = navigator;
        _loc = loc;
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        FormError = "";
        try
        {
            var today = DateTime.Today;
            var summary = await _analytics.GetSalesSummaryAsync(today, today.AddDays(1).AddSeconds(-1), default).ConfigureAwait(true);
            TodayRevenue = summary.TotalRevenue;
            TodayOrders = summary.OrderCount;
            AverageTicket = summary.AverageTicket;

            var pending = await _orderRepo.GetByKitchenStatusAsync("pending", default).ConfigureAwait(true);
            var sent = await _orderRepo.GetByKitchenStatusAsync("sent", default).ConfigureAwait(true);
            OpenOrders = pending.Count + sent.Count;

            var top = await _analytics.GetTopSellingByWeekAsync(default).ConfigureAwait(true);
            BuildTopProducts(top);

            var hourly = await _analytics.GetSalesByHourAsync(today, default).ConfigureAwait(true);
            BuildHourlySales(hourly);

            var low = await _menuItemRepo.GetLowStockAsync(default).ConfigureAwait(true);
            LowStockItems.Clear();
            foreach (var item in low ?? Enumerable.Empty<MenuItem>())
                LowStockItems.Add(item);
        }
        catch (Exception ex)
        {
            FormError = "Không tải được số liệu: " + MapCrudError(ex);
        }
        finally
        {
            IsLoading = false;
            NotifyDerivedState();
        }
    }

    private void BuildTopProducts(IReadOnlyList<KeyValuePair<string, float>> top)
    {
        TopProducts.Clear();
        var max = top.Count == 0 ? 0d : top.Max(x => (double)x.Value);
        var rank = 0;
        foreach (var item in top.Take(8))
        {
            rank++;
            TopProducts.Add(new TopProductRow
            {
                Rank = rank,
                Name = string.IsNullOrWhiteSpace(item.Key) ? "—" : item.Key,
                Revenue = (decimal)item.Value,
                ShareBarWidth = max <= 0 ? 0 : Math.Max(6, ShareTrackWidth * item.Value / max)
            });
        }
    }

    private void BuildHourlySales(IReadOnlyList<float>? hourly)
    {
        HourlySales.Clear();
        if (hourly is null || hourly.Count == 0)
            return;

        var totals = new double[ChartBucketCount];
        for (var i = 0; i < ChartBucketCount; i++)
        {
            var start = ChartFirstHour + i * ChartBucketHours;
            for (var h = start; h < start + ChartBucketHours && h < hourly.Count; h++)
                totals[i] += hourly[h];
        }

        var max = totals.Max();
        for (var i = 0; i < ChartBucketCount; i++)
        {
            var start = ChartFirstHour + i * ChartBucketHours;
            var ratio = max <= 0 ? 0 : totals[i] / max;
            HourlySales.Add(new HourlySalesBar
            {
                Label = start.ToString("00", CultureInfo.InvariantCulture) + "h",
                Revenue = (decimal)totals[i],
                BarHeight = MinBarHeight + (MaxBarHeight - MinBarHeight) * ratio,
                IsPeak = max > 0 && Math.Abs(totals[i] - max) < 0.01,
                Tooltip = $"{start:00}:00–{start + ChartBucketHours:00}:00 · {totals[i]:N0} ₫"
            });
        }
    }

    private void NotifyDerivedState()
    {
        OnPropertyChanged(nameof(NotLoading));
        OnPropertyChanged(nameof(TopProductsEmpty));
        OnPropertyChanged(nameof(LowStockEmpty));
        OnPropertyChanged(nameof(LowStockCount));
        OnPropertyChanged(nameof(HourlySalesEmpty));
    }

    private string Loc(string key, string fallback) => _loc[key].Value ?? fallback;

    [RelayCommand]
    private void OpenSales() => _navigator.OpenModule(typeof(OrderSelectionViewModel), Loc("OrderSelection", "Bán hàng"));

    [RelayCommand]
    private void OpenTables() => _navigator.OpenModule(typeof(TableMapViewModel), Loc("TableMap", "Sơ đồ bàn"));

    [RelayCommand]
    private void OpenInventory() => _navigator.OpenModule(typeof(InventoryViewModel), Loc("Inventory", "Kho hàng"));

    [RelayCommand]
    private void OpenDailySales() => _navigator.OpenModule(typeof(DailySalesViewModel), Loc("DailySales", "Báo cáo ngày"));
}
