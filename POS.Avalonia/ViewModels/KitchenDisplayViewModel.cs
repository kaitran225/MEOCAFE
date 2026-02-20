using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Avalonia.Models;
using POS.Avalonia.Services;
using POS.Core.Contracts;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class KitchenDisplayViewModel : ViewModelBase, IDisposable
{
    private readonly IKitchenDisplayService _kitchenService;
    private readonly IOrderRepository _orderRepo;
    private System.Timers.Timer? _refreshTimer;

    [ObservableProperty] private ObservableCollection<KitchenOrderDisplay> _orders = new();
    [ObservableProperty] private KitchenOrderDisplay? _selectedOrder;
    [ObservableProperty] private bool _isLoading;
    /// <summary>Filter: null or "active" = sent + in_progress, "sent", "in_progress", "done".</summary>
    [ObservableProperty] private string _statusFilter = "active";
    [ObservableProperty] private string _autoRefreshSecondsText = "10";
    [ObservableProperty] private double _displayFontSize = 14;

    public KitchenDisplayViewModel(IKitchenDisplayService kitchenService, IOrderRepository orderRepo)
    {
        _kitchenService = kitchenService;
        _orderRepo = orderRepo;
        _ = LoadAsync();
        StartAutoRefresh();
    }

    private void StartAutoRefresh()
    {
        var sec = int.TryParse(AutoRefreshSecondsText, out var s) && s > 0 ? s : 10;
        _refreshTimer = new System.Timers.Timer(sec * 1000);
        _refreshTimer.Elapsed += async (_, _) => await LoadAsync();
        _refreshTimer.AutoReset = true;
        _refreshTimer.Start();
    }

    public void Dispose() => _refreshTimer?.Stop();

    partial void OnAutoRefreshSecondsTextChanged(string value)
    {
        _refreshTimer?.Stop();
        if (int.TryParse(value, out var sec) && sec > 0)
        { _refreshTimer = new System.Timers.Timer(sec * 1000); _refreshTimer.Elapsed += async (_, _) => await LoadAsync(); _refreshTimer.AutoReset = true; _refreshTimer.Start(); }
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var list = await _kitchenService.GetActiveOrdersAsync(StatusFilter, default).ConfigureAwait(true);
            Orders = new ObservableCollection<KitchenOrderDisplay>(list);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanStartPreparing))]
    private async Task StartPreparingAsync(KitchenOrderDisplay? order)
    {
        var o = order ?? SelectedOrder;
        if (o == null) return;
        await _orderRepo.UpdateKitchenStatusAsync(o.OrderId, "in_progress", default).ConfigureAwait(true);
        await LoadAsync();
    }

    private bool CanStartPreparing()
    {
        var o = SelectedOrder;
        return o != null && string.Equals(o.Status, "sent", StringComparison.OrdinalIgnoreCase);
    }

    [RelayCommand(CanExecute = nameof(CanMarkPrepared))]
    private async Task MarkPreparedAsync(KitchenOrderDisplay? order)
    {
        var o = order ?? SelectedOrder;
        if (o == null) return;
        await _orderRepo.UpdateKitchenStatusAsync(o.OrderId, "done", default).ConfigureAwait(true);
        Orders.Remove(o);
        SelectedOrder = null;
    }

    private bool CanMarkPrepared() => SelectedOrder != null;

    partial void OnSelectedOrderChanged(KitchenOrderDisplay? value)
    {
        MarkPreparedCommand.NotifyCanExecuteChanged();
        StartPreparingCommand.NotifyCanExecuteChanged();
    }

    partial void OnStatusFilterChanged(string value) => _ = LoadAsync();

    [RelayCommand]
    private void SetStatusFilter(string? filter)
    {
        if (!string.IsNullOrEmpty(filter)) StatusFilter = filter;
    }
}
