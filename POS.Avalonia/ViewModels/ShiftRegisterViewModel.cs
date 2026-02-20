using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Avalonia.Services;
using POS.Core.Contracts;
using POS.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class ShiftRegisterViewModel : ViewModelBase
{
    private readonly CurrentUserService _currentUser;
    private readonly IShiftSessionRepository _sessionRepo;

    [ObservableProperty] private ShiftSession? _currentShift;
    [ObservableProperty] private string _openingCashText = "0";
    [ObservableProperty] private string _closingCashText = "0";
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private ObservableCollection<ShiftSession> _sessionsToday = new();
    public bool HasCurrentShift => CurrentShift != null;
    public bool NoCurrentShift => !HasCurrentShift;

    public string Title => "Shift register";

    public ShiftRegisterViewModel(CurrentUserService currentUser, IShiftSessionRepository sessionRepo)
    {
        _currentUser = currentUser;
        _sessionRepo = sessionRepo;
        _ = LoadCurrentAsync();
    }

    [RelayCommand]
    private async Task LoadCurrentAsync()
    {
        IsLoading = true;
        try
        {
            var user = _currentUser.Current?.Username ?? "";
            CurrentShift = !string.IsNullOrEmpty(user) ? await _sessionRepo.GetCurrentAsync(user, default).ConfigureAwait(true) : null;
            OnPropertyChanged(nameof(HasCurrentShift)); OnPropertyChanged(nameof(NoCurrentShift));
            var today = DateTime.Today;
            var list = !string.IsNullOrEmpty(user) ? await _sessionRepo.GetByDateAsync(today, default).ConfigureAwait(true) : Array.Empty<ShiftSession>();
            SessionsToday = new ObservableCollection<ShiftSession>(list);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ClockInAsync()
    {
        var user = _currentUser.Current?.Username;
        if (string.IsNullOrEmpty(user)) return;
        if (!decimal.TryParse(OpeningCashText, out var cash)) cash = 0;
        await _sessionRepo.StartShiftAsync(user, cash, default).ConfigureAwait(true);
        await LoadCurrentAsync().ConfigureAwait(true);
    }

    [RelayCommand]
    private async Task ClockOutAsync()
    {
        if (CurrentShift == null) return;
        if (!decimal.TryParse(ClosingCashText, out var cash)) cash = 0;
        await _sessionRepo.EndShiftAsync(CurrentShift.Id, cash, default).ConfigureAwait(true);
        CurrentShift = null;
        OnPropertyChanged(nameof(HasCurrentShift)); OnPropertyChanged(nameof(NoCurrentShift));
        await LoadCurrentAsync().ConfigureAwait(true);
    }
}
