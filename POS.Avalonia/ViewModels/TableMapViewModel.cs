using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Avalonia.Models;
using POS.Avalonia.Services;
using POS.Core.Contracts;
using POS.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class TableMapViewModel : ViewModelBase
{
    private readonly ITableRepository _tableRepo;
    private readonly IOrderRepository _orderRepo;
    private readonly PendingTableService _pendingTable;
    private readonly IAppNavigator _navigator;
    private readonly CurrentUserService _currentUser;

    [ObservableProperty] private ObservableCollection<Table> _tables = new();
    [ObservableProperty] private ObservableCollection<TableMapCardItem> _cards = new();
    [ObservableProperty] private ObservableCollection<TableMapCardItem> _filteredCards = new();
    [ObservableProperty] private ObservableCollection<string> _zones = new();
    [ObservableProperty] private string _selectedZone = "All";
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isTableEditOpen;
    [ObservableProperty] private Table? _selectedTableForEdit;
    [ObservableProperty] private string _tableFormName = "";
    [ObservableProperty] private string _tableFormCapacity = "2";
    [ObservableProperty] private string _tableFormZone = "";

    public string Title => "Table map";
    public bool IsManager => true;
    public bool NoTables => FilteredCards.Count == 0;

    public TableMapViewModel(ITableRepository tableRepo, IOrderRepository orderRepo, PendingTableService pendingTable, IAppNavigator navigator, CurrentUserService currentUser)
    {
        _tableRepo = tableRepo;
        _orderRepo = orderRepo;
        _pendingTable = pendingTable;
        _navigator = navigator;
        _currentUser = currentUser;
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var tableList = await _tableRepo.GetAllAsync(default).ConfigureAwait(true);
            Tables = new ObservableCollection<Table>(tableList);
            var orders = await _orderRepo.GetAllAsync(default).ConfigureAwait(true);
            var orderByTable = orders
                .Where(o => o.TableId.HasValue)
                .GroupBy(o => o.TableId!.Value)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(o => o.CreatedAt).First());
            var cardList = tableList.Select(t =>
            {
                string? elapsed = null;
                if (orderByTable.TryGetValue(t.Id, out var order))
                {
                    var span = DateTime.UtcNow - order.CreatedAt;
                    elapsed = $"{(int)span.TotalMinutes:D2}:{span.Seconds:D2}";
                }
                return new TableMapCardItem { Table = t, ElapsedDisplay = elapsed };
            }).ToList();
            Cards = new ObservableCollection<TableMapCardItem>(cardList);
            var zoneList = tableList.Select(t => t.Zone ?? "—").Where(z => !string.IsNullOrEmpty(z)).Distinct().OrderBy(z => z).ToList();
            Zones = new ObservableCollection<string>(new[] { "All" }.Concat(zoneList));
            ApplyZoneFilter();
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSelectedZoneChanged(string value) => ApplyZoneFilter();

    private void ApplyZoneFilter()
    {
        var filtered = string.IsNullOrEmpty(SelectedZone) || SelectedZone == "All"
            ? Cards
            : new ObservableCollection<TableMapCardItem>(Cards.Where(c => string.Equals(c.Table.Zone ?? "—", SelectedZone, StringComparison.OrdinalIgnoreCase)));
        FilteredCards = filtered;
        OnPropertyChanged(nameof(NoTables));
    }

    [RelayCommand]
    private void NewOrderAtTable(TableMapCardItem? card)
    {
        if (card?.Table == null) return;
        _navigator.OpenOrderForTable(card.Table);
    }

    [RelayCommand]
    private void NewOrderAtTableByTable(Table? table)
    {
        if (table == null) return;
        _navigator.OpenOrderForTable(table);
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
            var t = new Table { Name = TableFormName.Trim(), Capacity = capacity, Zone = string.IsNullOrWhiteSpace(TableFormZone) ? null : TableFormZone.Trim(), Status = "available" };
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
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task ReserveTableAsync(Table? table)
    {
        if (table == null) return;
        await _tableRepo.UpdateStatusAsync(table.Id, "reserved", default).ConfigureAwait(true);
        table.Status = "reserved";
    }
}
