using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Avalonia.Services;
using POS.Core.Contracts;
using POS.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class MenuManagementViewModel : ViewModelBase
{
    private readonly ICategoryRepository _categoryRepo;
    private readonly IMenuItemRepository _menuItemRepo;
    private readonly IStoragePickerService _storagePicker;

    [ObservableProperty] private ObservableCollection<Category> _categories = new();
    [ObservableProperty] private ObservableCollection<MenuItem> _products = new();
    [ObservableProperty] private ObservableCollection<MenuItem> _filteredProducts = new();
    [ObservableProperty] private Category? _selectedCategory;
    [ObservableProperty] private MenuItem? _selectedProduct;
    [ObservableProperty] private Category? _categoryEditTarget;
    [ObservableProperty] private string _searchText = "";
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isProductEditOpen;
    [ObservableProperty] private bool _isCategoryEditOpen;
    [ObservableProperty] private bool _isDeleteProductConfirmOpen;
    [ObservableProperty] private bool _isDeleteCategoryConfirmOpen;
    [ObservableProperty] private MenuItem? _deleteProductTarget;
    [ObservableProperty] private Category? _deleteCategoryTarget;
    [ObservableProperty] private string _productName = "";
    [ObservableProperty] private decimal _productPrice;
    [ObservableProperty] private int _productQuantity;
    [ObservableProperty] private int _productReorderLevel;
    [ObservableProperty] private Category? _productFormCategory;
    [ObservableProperty] private string _categoryName = "";
    [ObservableProperty] private string _exportImportMessage = "";

    public string Title => "Quản lý thực đơn";
    public int TotalProducts => Products.Count;
    public int TotalCategories => Categories.Count;
    public int LowStockCount => Products.Count(p => p.Quantity <= p.ReorderLevel);

    public MenuManagementViewModel(ICategoryRepository categoryRepo, IMenuItemRepository menuItemRepo, IStoragePickerService storagePicker)
    {
        _categoryRepo = categoryRepo;
        _menuItemRepo = menuItemRepo;
        _storagePicker = storagePicker;
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var cats = await _categoryRepo.GetAllAsync(default).ConfigureAwait(true);
            var items = await _menuItemRepo.GetAllAsync(default).ConfigureAwait(true);
            Categories = new ObservableCollection<Category>(cats);
            var productList = items.ToList();
            foreach (var p in productList)
                p.Category = cats.FirstOrDefault(c => c.Id == p.CategoryId);
            Products = new ObservableCollection<MenuItem>(productList);
            ApplyFilter();
            OnPropertyChanged(nameof(TotalProducts));
            OnPropertyChanged(nameof(TotalCategories));
            OnPropertyChanged(nameof(LowStockCount));
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSelectedCategoryChanged(Category? value) => ApplyFilter();
    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        var q = Products.AsEnumerable();
        if (SelectedCategory != null)
            q = q.Where(p => p.CategoryId == SelectedCategory.Id);
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var term = SearchText.Trim();
            q = q.Where(p => p.Name?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false);
        }
        FilteredProducts = new ObservableCollection<MenuItem>(q.ToList());
    }

    [RelayCommand]
    private void SelectCategory(Category? category) => SelectedCategory = category;

    [RelayCommand]
    private void SelectAllCategories() => SelectedCategory = null;

    [RelayCommand]
    private void OpenAddProduct()
    {
        ClearMessages();
        SelectedProduct = null;
        ProductName = "";
        ProductPrice = 0;
        ProductQuantity = 0;
        ProductReorderLevel = 0;
        ProductFormCategory = Categories.FirstOrDefault();
        IsProductEditOpen = true;
    }

    [RelayCommand]
    private void OpenEditProduct(MenuItem? item)
    {
        if (item == null) return;
        ClearMessages();
        SelectedProduct = item;
        ProductName = item.Name ?? "";
        ProductPrice = item.SellPrice;
        ProductQuantity = item.Quantity;
        ProductReorderLevel = item.ReorderLevel;
        ProductFormCategory = Categories.FirstOrDefault(c => c.Id == item.CategoryId);
        IsProductEditOpen = true;
    }

    [RelayCommand]
    private async Task SaveProductAsync()
    {
        ClearMessages();
        if (string.IsNullOrWhiteSpace(ProductName))
        {
            FormError = "Vui lòng nhập tên món.";
            return;
        }

        if (ProductFormCategory is null || ProductFormCategory.Id <= 0)
        {
            FormError = "Vui lòng chọn danh mục.";
            return;
        }

        var categoryId = ProductFormCategory.Id;
        var ok = await RunCrudAsync(async () =>
        {
            if (SelectedProduct != null)
            {
                SelectedProduct.Name = ProductName.Trim();
                SelectedProduct.SellPrice = ProductPrice;
                SelectedProduct.Quantity = ProductQuantity;
                SelectedProduct.ReorderLevel = ProductReorderLevel;
                SelectedProduct.CategoryId = categoryId;
                await _menuItemRepo.UpdateAsync(SelectedProduct, default).ConfigureAwait(true);
            }
            else
            {
                var m = new MenuItem
                {
                    Name = ProductName.Trim(),
                    SellPrice = ProductPrice,
                    Quantity = ProductQuantity,
                    ReorderLevel = ProductReorderLevel,
                    CategoryId = categoryId,
                    CapitalPrice = 0
                };
                await _menuItemRepo.AddAsync(m, default).ConfigureAwait(true);
            }
        }).ConfigureAwait(true);

        if (!ok) return;
        IsProductEditOpen = false;
        await LoadAsync().ConfigureAwait(true);
    }

    [RelayCommand]
    private void CloseProductEdit() => IsProductEditOpen = false;

    [RelayCommand]
    private void RequestDeleteProduct(MenuItem? item)
    {
        if (item == null) return;
        DeleteProductTarget = item;
        IsDeleteProductConfirmOpen = true;
    }

    [RelayCommand]
    private void CancelDeleteProduct()
    {
        IsDeleteProductConfirmOpen = false;
        DeleteProductTarget = null;
    }

    [RelayCommand]
    private async Task ConfirmDeleteProductAsync()
    {
        if (DeleteProductTarget == null) return;
        var id = DeleteProductTarget.Id;
        var ok = await RunCrudAsync(() => _menuItemRepo.DeleteAsync(id, default)).ConfigureAwait(true);
        if (!ok) return;
        IsDeleteProductConfirmOpen = false;
        DeleteProductTarget = null;
        await LoadAsync().ConfigureAwait(true);
    }

    [RelayCommand]
    private void OpenAddCategory()
    {
        ClearMessages();
        CategoryEditTarget = null;
        CategoryName = "";
        IsCategoryEditOpen = true;
    }

    [RelayCommand]
    private void OpenEditCategory(Category? category)
    {
        if (category == null) return;
        ClearMessages();
        CategoryEditTarget = category;
        CategoryName = category.Name ?? "";
        IsCategoryEditOpen = true;
    }

    [RelayCommand]
    private async Task SaveCategoryAsync()
    {
        ClearMessages();
        if (string.IsNullOrWhiteSpace(CategoryName))
        {
            FormError = "Vui lòng nhập tên danh mục.";
            return;
        }

        var ok = await RunCrudAsync(async () =>
        {
            if (CategoryEditTarget != null)
            {
                CategoryEditTarget.Name = CategoryName.Trim();
                await _categoryRepo.UpdateAsync(CategoryEditTarget, default).ConfigureAwait(true);
            }
            else
            {
                var c = new Category { Name = CategoryName.Trim() };
                await _categoryRepo.AddAsync(c, default).ConfigureAwait(true);
            }
        }).ConfigureAwait(true);

        if (!ok) return;
        IsCategoryEditOpen = false;
        await LoadAsync().ConfigureAwait(true);
    }

    [RelayCommand]
    private void CloseCategoryEdit() => IsCategoryEditOpen = false;

    [RelayCommand]
    private void RequestDeleteCategory(Category? category)
    {
        if (category == null) return;
        if (Products.Any(p => p.CategoryId == category.Id))
        {
            FormError = "Không xóa được: còn món trong danh mục này.";
            return;
        }

        DeleteCategoryTarget = category;
        IsDeleteCategoryConfirmOpen = true;
    }

    [RelayCommand]
    private void CancelDeleteCategory()
    {
        IsDeleteCategoryConfirmOpen = false;
        DeleteCategoryTarget = null;
    }

    [RelayCommand]
    private async Task ConfirmDeleteCategoryAsync()
    {
        if (DeleteCategoryTarget == null) return;
        var id = DeleteCategoryTarget.Id;
        var ok = await RunCrudAsync(() => _categoryRepo.DeleteAsync(id, default)).ConfigureAwait(true);
        if (!ok) return;
        IsDeleteCategoryConfirmOpen = false;
        DeleteCategoryTarget = null;
        await LoadAsync().ConfigureAwait(true);
    }

    [RelayCommand]
    private async Task ExportProductsAsync()
    {
        ExportImportMessage = "";
        var path = await _storagePicker.PickSaveFileAsync("products", ".csv", default).ConfigureAwait(true);
        if (string.IsNullOrEmpty(path)) return;
        IsLoading = true;
        try
        {
            var cats = await _categoryRepo.GetAllAsync(default).ConfigureAwait(true);
            var items = await _menuItemRepo.GetAllAsync(default).ConfigureAwait(true);
            var catDict = cats.ToDictionary(c => c.Id, c => c.Name ?? "");
            var sb = new StringBuilder();
            sb.AppendLine("CategoryName,ProductName,Price,Quantity,ReorderLevel");
            foreach (var m in items)
            {
                var catName = catDict.GetValueOrDefault(m.CategoryId, "");
                sb.AppendLine(EscapeCsv(catName) + "," + EscapeCsv(m.Name ?? "") + "," + m.SellPrice.ToString(CultureInfo.InvariantCulture) + "," + m.Quantity + "," + m.ReorderLevel);
            }
            await System.IO.File.WriteAllTextAsync(path, sb.ToString(), default).ConfigureAwait(true);
            ExportImportMessage = "Đã xuất CSV.";
        }
        catch (Exception ex)
        {
            ExportImportMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ImportProductsAsync()
    {
        ExportImportMessage = "";
        await using var stream = await _storagePicker.PickOpenFileAsync(".csv", default).ConfigureAwait(true);
        if (stream is null) return;
        IsLoading = true;
        try
        {
            using var reader = new System.IO.StreamReader(stream);
            var content = await reader.ReadToEndAsync(default).ConfigureAwait(true);
            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var cats = await _categoryRepo.GetAllAsync(default).ConfigureAwait(true);
            var catByName = cats.ToDictionary(c => c.Name ?? "", c => c, StringComparer.OrdinalIgnoreCase);
            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = ParseCsvLine(line);
                if (parts.Count < 3) continue;
                var catName = parts[0];
                var prodName = parts[1];
                if (!decimal.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var price)) continue;
                var qty = parts.Count > 3 && int.TryParse(parts[3], out var q) ? q : 0;
                var reorder = parts.Count > 4 && int.TryParse(parts[4], out var r) ? r : 0;
                if (!catByName.TryGetValue(catName, out var existingCat))
                {
                    var newCat = new Category { Name = catName };
                    await _categoryRepo.AddAsync(newCat, default).ConfigureAwait(true);
                    catByName[catName] = newCat;
                }

                var m = new MenuItem
                {
                    Name = prodName,
                    SellPrice = price,
                    Quantity = qty,
                    ReorderLevel = reorder,
                    CategoryId = catByName[catName].Id,
                    CapitalPrice = 0
                };
                await _menuItemRepo.AddAsync(m, default).ConfigureAwait(true);
            }

            ExportImportMessage = "Đã nhập CSV.";
            await LoadAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            ExportImportMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private static string EscapeCsv(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        return value;
    }

    private static List<string> ParseCsvLine(string line)
    {
        var result = new List<string>();
        var cur = new StringBuilder();
        var inQuotes = false;
        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (c == '"') { inQuotes = !inQuotes; continue; }
            if (c == ',' && !inQuotes) { result.Add(cur.ToString()); cur.Clear(); continue; }
            cur.Append(c);
        }
        result.Add(cur.ToString());
        return result;
    }
}
