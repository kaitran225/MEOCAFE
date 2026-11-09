using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Core.Contracts;
using POS.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class ShiftManagementViewModel : ViewModelBase
{
    private readonly IShiftSessionRepository _sessionRepo;

    [ObservableProperty] private ObservableCollection<ShiftSession> _sessions = new();
    [ObservableProperty] private string _selectedDateText = "";
    [ObservableProperty] private bool _isLoading;

    public string Title => "Shift management";
    public string SessionCountText => Sessions.Count == 0 ? "No sessions" : $"{Sessions.Count} session(s)";

    public ShiftManagementViewModel(IShiftSessionRepository sessionRepo)
    {
        _sessionRepo = sessionRepo;
        _selectedDateText = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var date = DateTime.TryParse(SelectedDateText, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d)
                ? d.Date
                : DateTime.Today;
            var list = await _sessionRepo.GetByDateAsync(date, default).ConfigureAwait(true);
            Sessions = new ObservableCollection<ShiftSession>(list.OrderBy(s => s.StartAt).ToList());
            OnPropertyChanged(nameof(SessionCountText));
        }
        finally
        {
            IsLoading = false;
        }
    }
}
