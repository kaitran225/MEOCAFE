using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Core.Contracts;
using POS.Core.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class InventoryViewModel : ViewModelBase
{
    private readonly IMenuItemRepository _menuItemRepo;

    [ObservableProperty] private ObservableCollection<MenuItem> _items = new();
    [ObservableProperty] private ObservableCollection<MenuItem> _lowStockItems = new();
    [ObservableProperty] private MenuItem? _selectedItem;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isAdjustOpen;
    [ObservableProperty] private string _adjustQuantityText = "0";
    [ObservableProperty] private string _adjustReason = "";
    [ObservableProperty] private bool _isStockInOpen;
    [ObservableProperty] private string _stockInQuantityText = "0";
    [ObservableProperty] private string _stockInReference = "";

    public InventoryViewModel(IMenuItemRepository menuItemRepo)
    {
        _menuItemRepo = menuItemRepo;
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var all = await _menuItemRepo.GetAllAsync(default).ConfigureAwait(true);
            var low = await _menuItemRepo.GetLowStockAsync(default).ConfigureAwait(true);
            Items = new ObservableCollection<MenuItem>(all);
            LowStockItems = new ObservableCollection<MenuItem>(low);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void OpenAdjust(MenuItem? item)
    {
        if (item == null) return;
        SelectedItem = item;
        AdjustQuantityText = "0";
        AdjustReason = "";
        IsAdjustOpen = true;
    }

    [RelayCommand]
    private async Task ConfirmAdjustAsync()
    {
        if (SelectedItem == null) return;
        if (!int.TryParse(AdjustQuantityText, out var delta)) return;
        var current = await _menuItemRepo.GetByIdAsync(SelectedItem.Id, default).ConfigureAwait(true);
        if (current == null) return;
        current.Quantity = Math.Max(0, current.Quantity + delta);
        await _menuItemRepo.UpdateAsync(current, default).ConfigureAwait(true);
        IsAdjustOpen = false;
        _ = LoadAsync();
    }

    [RelayCommand]
    private void CloseAdjust() => IsAdjustOpen = false;

    [RelayCommand]
    private void OpenStockIn(MenuItem? item)
    {
        if (item == null) return;
        SelectedItem = item;
        StockInQuantityText = "0";
        StockInReference = "";
        IsStockInOpen = true;
    }

    [RelayCommand]
    private async Task ConfirmStockInAsync()
    {
        if (SelectedItem == null) return;
        if (!int.TryParse(StockInQuantityText, out var qty) || qty <= 0) return;
        var current = await _menuItemRepo.GetByIdAsync(SelectedItem.Id, default).ConfigureAwait(true);
        if (current == null) return;
        current.Quantity += qty;
        await _menuItemRepo.UpdateAsync(current, default).ConfigureAwait(true);
        IsStockInOpen = false;
        _ = LoadAsync();
    }

    [RelayCommand]
    private void CloseStockIn() => IsStockInOpen = false;
}
