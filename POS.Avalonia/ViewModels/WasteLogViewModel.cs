using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Avalonia.Services;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Avalonia.ViewModels;

public partial class WasteLogViewModel : ViewModelBase
{
    private readonly IWasteRepository _wasteRepo;
    private readonly CurrentUserService _currentUser;

    [ObservableProperty] private ObservableCollection<IngredientWaste> _wasteLogs = new();
    [ObservableProperty] private bool _isFormOpen;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _itemName = "";
    [ObservableProperty] private string _quantity = "";
    [ObservableProperty] private string _unit = "";
    [ObservableProperty] private string _reason = "";
    [ObservableProperty] private string _notes = "";

    public WasteLogViewModel(IWasteRepository wasteRepo, CurrentUserService currentUser)
    {
        _wasteRepo = wasteRepo;
        _currentUser = currentUser;
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var all = await _wasteRepo.GetAllAsync(default).ConfigureAwait(true);
            WasteLogs = new ObservableCollection<IngredientWaste>(all);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void OpenForm()
    {
        ItemName = "";
        Quantity = "";
        Unit = "";
        Reason = "";
        Notes = "";
        IsFormOpen = true;
    }

    [RelayCommand]
    private void CloseForm() => IsFormOpen = false;

    [RelayCommand]
    private async Task SaveAsync()
    {
        ClearMessages();
        if (string.IsNullOrWhiteSpace(ItemName))
        {
            FormError = "Vui lòng nhập tên nguyên liệu.";
            return;
        }

        if (!decimal.TryParse(Quantity, out var qty) || qty <= 0)
        {
            FormError = "Số lượng phải lớn hơn 0.";
            return;
        }

        var entry = new IngredientWaste
        {
            ItemName = ItemName.Trim(),
            Quantity = qty,
            Unit = Unit.Trim(),
            Reason = Reason.Trim(),
            LoggedBy = _currentUser.Current?.Fullname ?? "Unknown",
            LoggedAt = DateTime.UtcNow,
            Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim()
        };

        var ok = await RunCrudAsync(() => _wasteRepo.InsertAsync(entry, default)).ConfigureAwait(true);
        if (!ok) return;
        IsFormOpen = false;
        await LoadAsync().ConfigureAwait(true);
    }
}
