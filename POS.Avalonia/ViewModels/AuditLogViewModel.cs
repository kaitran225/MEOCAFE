using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Core.Contracts;
using POS.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class AuditLogViewModel : ViewModelBase
{
    private readonly IAuditLogRepository _repo;

    [ObservableProperty] private ObservableCollection<AuditLogEntry> _entries = new();
    [ObservableProperty] private DateTime? _filterFrom;
    [ObservableProperty] private DateTime? _filterTo;
    [ObservableProperty] private string _filterUser = "";
    [ObservableProperty] private string _filterAction = "";
    [ObservableProperty] private bool _isLoading;

    public AuditLogViewModel(IAuditLogRepository repo)
    {
        _repo = repo;
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var list = await _repo.GetAsync(FilterFrom, FilterTo, string.IsNullOrWhiteSpace(FilterUser) ? null : FilterUser, string.IsNullOrWhiteSpace(FilterAction) ? null : FilterAction, default).ConfigureAwait(true);
            Entries = new ObservableCollection<AuditLogEntry>(list);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
