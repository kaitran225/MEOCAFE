using System;
using System.Collections.Generic;
using System.Linq;
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
    private readonly ICategoryRepository _categoryRepo;

    [ObservableProperty] private ObservableCollection<MenuItem> _items = new();
    [ObservableProperty] private ObservableCollection<MenuItem> _filteredItems = new();
    [ObservableProperty] private ObservableCollection<MenuItem> _pagedItems = new();
    [ObservableProperty] private ObservableCollection<MenuItem> _lowStockItems = new();
    [ObservableProperty] private ObservableCollection<Category> _categories = new();
    [ObservableProperty] private Category? _selectedCategory;
    [ObservableProperty] private string _searchText = "";
    [ObservableProperty] private int _currentPage = 1;
    [ObservableProperty] private int _pageSize = 25;
    [ObservableProperty] private ObservableCollection<int> _selectedIds = new();
    [ObservableProperty] private MenuItem? _selectedItem;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isAdjustOpen;
    [ObservableProperty] private string _adjustQuantityText = "0";
    [ObservableProperty] private string _adjustReason = "";
    [ObservableProperty] private bool _isStockInOpen;
    [ObservableProperty] private string _stockInQuantityText = "0";
    [ObservableProperty] private string _stockInReference = "";

    public InventoryViewModel(IMenuItemRepository menuItemRepo, ICategoryRepository categoryRepo)
    {
        _menuItemRepo = menuItemRepo;
        _categoryRepo = categoryRepo;
        _ = LoadAsync();
    }

    public int TotalFiltered => FilteredItems.Count;
    public bool NoItems => FilteredItems.Count == 0;
    public int TotalPages => Math.Max(1, (int)Math.Ceiling((double)TotalFiltered / PageSize));
    public bool HasPrevPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;

    partial void OnSelectedCategoryChanged(Category? value) => ApplyFilter();
    partial void OnSearchTextChanged(string value) => ApplyFilter();
    partial void OnCurrentPageChanged(int value) => ApplyPaging();

    private void ApplyFilter()
    {
        var q = Items.AsEnumerable();
        if (SelectedCategory != null)
            q = q.Where(i => i.CategoryId == SelectedCategory.Id);
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var term = SearchText.Trim();
            q = q.Where(i => (i.Name?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false));
        }
        FilteredItems = new ObservableCollection<MenuItem>(q.ToList());
        CurrentPage = 1;
        ApplyPaging();
        NotifyPagination();
        OnPropertyChanged(nameof(NoItems));
    }

    private void NotifyPagination()
    {
        OnPropertyChanged(nameof(TotalFiltered));
        OnPropertyChanged(nameof(TotalPages));
        OnPropertyChanged(nameof(HasPrevPage));
        OnPropertyChanged(nameof(HasNextPage));
    }

    private void ApplyPaging()
    {
        var skip = (CurrentPage - 1) * PageSize;
        var page = FilteredItems.Skip(skip).Take(PageSize).ToList();
        PagedItems = new ObservableCollection<MenuItem>(page);
    }

    [RelayCommand]
    private void SelectCategory(Category? category) => SelectedCategory = category;

    [RelayCommand]
    private void SelectAllCategories() => SelectedCategory = null;

    [RelayCommand]
    private void ToggleSelectItem(MenuItem? item)
    {
        if (item == null) return;
        if (SelectedIds.Contains(item.Id)) SelectedIds.Remove(item.Id);
        else SelectedIds.Add(item.Id);
    }

    [RelayCommand]
    private void ToggleSelectAll()
    {
        if (SelectedIds.Count >= PagedItems.Count)
            SelectedIds.Clear();
        else
        {
            SelectedIds.Clear();
            foreach (var i in PagedItems) SelectedIds.Add(i.Id);
        }
    }

    public bool IsSelected(MenuItem item) => SelectedIds.Contains(item.Id);

    [RelayCommand]
    private void PrevPage()
    {
        if (CurrentPage > 1) CurrentPage--;
        NotifyPagination();
    }

    [RelayCommand]
    private void NextPage()
    {
        if (CurrentPage < TotalPages) CurrentPage++;
        NotifyPagination();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var cats = await _categoryRepo.GetAllAsync(default).ConfigureAwait(true);
            var all = await _menuItemRepo.GetAllAsync(default).ConfigureAwait(true);
            var low = await _menuItemRepo.GetLowStockAsync(default).ConfigureAwait(true);
            Categories = new ObservableCollection<Category>(cats);
            var list = all.ToList();
            foreach (var i in list)
                i.Category = cats.FirstOrDefault(c => c.Id == i.CategoryId);
            Items = new ObservableCollection<MenuItem>(list);
            LowStockItems = new ObservableCollection<MenuItem>(low);
            ApplyFilter();
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
        ClearMessages();
        if (SelectedItem == null) return;
        if (!int.TryParse(AdjustQuantityText, out var delta))
        {
            FormError = "Số lượng điều chỉnh không hợp lệ.";
            return;
        }
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
        ClearMessages();
        if (SelectedItem == null) return;
        if (!int.TryParse(StockInQuantityText, out var qty) || qty <= 0)
        {
            FormError = "Nhập số lượng nhập kho lớn hơn 0.";
            return;
        }
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
