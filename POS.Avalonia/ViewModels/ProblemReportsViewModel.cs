using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Avalonia.Services;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Avalonia.ViewModels;

public partial class ProblemReportsViewModel : ViewModelBase
{
    private readonly IProblemReportRepository _repo;
    private readonly CurrentUserService _currentUser;

    [ObservableProperty] private ObservableCollection<ProblemReport> _reports = new();
    [ObservableProperty] private ObservableCollection<ProblemReport> _filteredReports = new();
    [ObservableProperty] private bool _isFormOpen;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _title = "";
    [ObservableProperty] private string _description = "";
    [ObservableProperty] private string _category = "Equipment";
    [ObservableProperty] private string _priority = "Medium";
    [ObservableProperty] private string _searchText = "";
    [ObservableProperty] private string _filterPriority = "All";

    [ObservableProperty] private int _totalCount;
    [ObservableProperty] private int _openCount;
    [ObservableProperty] private int _resolvedCount;
    [ObservableProperty] private int _highPriorityCount;

    public ProblemReportsViewModel(IProblemReportRepository repo, CurrentUserService currentUser)
    {
        _repo = repo;
        _currentUser = currentUser;
        _ = LoadAsync();
    }

    partial void OnSearchTextChanged(string value) => ApplyFilters();
    partial void OnFilterPriorityChanged(string value) => ApplyFilters();

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var all = await _repo.GetAllAsync(default).ConfigureAwait(true);
            Reports = new ObservableCollection<ProblemReport>(all);
            UpdateKpis();
            ApplyFilters();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void UpdateKpis()
    {
        TotalCount = Reports.Count;
        OpenCount = Reports.Count(r => r.Status == "Open");
        ResolvedCount = Reports.Count(r => r.Status == "Resolved");
        HighPriorityCount = Reports.Count(r => r.Priority == "High");
    }

    private void ApplyFilters()
    {
        var filtered = Reports.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var term = SearchText.Trim();
            filtered = filtered.Where(r =>
                r.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                r.Category.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                r.ReportedBy.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        if (FilterPriority != "All")
            filtered = filtered.Where(r => r.Priority == FilterPriority);

        FilteredReports = new ObservableCollection<ProblemReport>(filtered);
    }

    [RelayCommand]
    private void OpenForm()
    {
        Title = "";
        Description = "";
        Category = "Equipment";
        Priority = "Medium";
        IsFormOpen = true;
    }

    [RelayCommand]
    private void CloseForm() => IsFormOpen = false;

    [RelayCommand]
    private void SetPriority(string priority) => Priority = priority;

    [RelayCommand]
    private void SetFilterPriority(string priority) => FilterPriority = priority;

    [RelayCommand]
    private async Task SaveAsync()
    {
        ClearMessages();
        if (string.IsNullOrWhiteSpace(Title))
        {
            FormError = "Vui lòng nhập tiêu đề.";
            return;
        }

        var report = new ProblemReport
        {
            Title = Title.Trim(),
            Description = Description.Trim(),
            Category = Category.Trim(),
            Priority = Priority,
            Status = "Open",
            ReportedBy = _currentUser.Current?.Fullname ?? "Unknown",
            ReportedAt = DateTime.UtcNow
        };

        var ok = await RunCrudAsync(() => _repo.InsertAsync(report, default)).ConfigureAwait(true);
        if (!ok) return;
        IsFormOpen = false;
        await LoadAsync().ConfigureAwait(true);
    }

    [RelayCommand]
    private async Task ResolveAsync(ProblemReport? report)
    {
        if (report == null) return;
        await _repo.UpdateStatusAsync(report.Id, "Resolved", "Resolved by staff", default).ConfigureAwait(true);
        await LoadAsync().ConfigureAwait(true);
    }
}
