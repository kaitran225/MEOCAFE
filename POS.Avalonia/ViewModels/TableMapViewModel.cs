using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Avalonia.Services;
using POS.Core.Contracts;
using POS.Core.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class TableMapViewModel : ViewModelBase
{
    private readonly ITableRepository _tableRepo;
    private readonly PendingTableService _pendingTable;

    [ObservableProperty] private ObservableCollection<Table> _tables = new();
    [ObservableProperty] private bool _isLoading;

    public string Title => "Table map";

    public TableMapViewModel(ITableRepository tableRepo, PendingTableService pendingTable)
    {
        _tableRepo = tableRepo;
        _pendingTable = pendingTable;
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var list = await _tableRepo.GetAllAsync(default).ConfigureAwait(true);
            Tables = new ObservableCollection<Table>(list);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void NewOrderAtTable(Table? table)
    {
        if (table == null) return;
        _pendingTable.SetAndRequestNavigateToOrder(table);
    }
}
