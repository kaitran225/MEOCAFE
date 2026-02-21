using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Avalonia.Services;
using POS.Core.Contracts;
using POS.Core.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class TableMapViewModel : ViewModelBase
{
    private readonly ITableRepository _tableRepo;
    private readonly PendingTableService _pendingTable;
    private readonly CurrentUserService _currentUser;

    [ObservableProperty] private ObservableCollection<Table> _tables = new();
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isTableEditOpen;
    [ObservableProperty] private Table? _selectedTableForEdit;
    [ObservableProperty] private string _tableFormName = "";
    [ObservableProperty] private string _tableFormCapacity = "2";
    [ObservableProperty] private string _tableFormZone = "";

    public string Title => "Table map";
    public bool IsManager => _currentUser.IsManager;

    public TableMapViewModel(ITableRepository tableRepo, PendingTableService pendingTable, CurrentUserService currentUser)
    {
        _tableRepo = tableRepo;
        _pendingTable = pendingTable;
        _currentUser = currentUser;
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

    [RelayCommand]
    private void OpenAddTable()
    {
        SelectedTableForEdit = null;
        TableFormName = "";
        TableFormCapacity = "2";
        TableFormZone = "";
        IsTableEditOpen = true;
    }

    [RelayCommand]
    private void OpenEditTable(Table? table)
    {
        if (table == null) return;
        SelectedTableForEdit = table;
        TableFormName = table.Name ?? "";
        TableFormCapacity = table.Capacity.ToString();
        TableFormZone = table.Zone ?? "";
        IsTableEditOpen = true;
    }

    [RelayCommand]
    private async Task SaveTableAsync()
    {
        if (string.IsNullOrWhiteSpace(TableFormName)) return;
        var capacity = int.TryParse(TableFormCapacity, out var cap) ? cap : 2;
        if (SelectedTableForEdit != null)
        {
            SelectedTableForEdit.Name = TableFormName.Trim();
            SelectedTableForEdit.Capacity = capacity;
            SelectedTableForEdit.Zone = string.IsNullOrWhiteSpace(TableFormZone) ? null : TableFormZone.Trim();
            await _tableRepo.UpdateAsync(SelectedTableForEdit, default).ConfigureAwait(true);
        }
        else
        {
            var t = new Table { Name = TableFormName.Trim(), Capacity = capacity, Zone = string.IsNullOrWhiteSpace(TableFormZone) ? null : TableFormZone.Trim(), Status = "empty" };
            await _tableRepo.AddAsync(t, default).ConfigureAwait(true);
            Tables.Add(t);
        }
        IsTableEditOpen = false;
        _ = LoadAsync();
    }

    [RelayCommand]
    private void CloseTableEdit() => IsTableEditOpen = false;

    [RelayCommand]
    private async Task DeleteTableAsync(Table? table)
    {
        if (table == null) return;
        await _tableRepo.DeleteAsync(table.Id, default).ConfigureAwait(true);
        var toRemove = Tables.FirstOrDefault(t => t.Id == table.Id);
        if (toRemove != null) Tables.Remove(toRemove);
    }

    [RelayCommand]
    private async Task ReserveTableAsync(Table? table)
    {
        if (table == null) return;
        await _tableRepo.UpdateStatusAsync(table.Id, "reserved", default).ConfigureAwait(true);
        table.Status = "reserved";
    }
}
