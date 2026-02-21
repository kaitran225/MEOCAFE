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
    [ObservableProperty] private Category? _selectedCategory;
    [ObservableProperty] private MenuItem? _selectedProduct;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isProductEditOpen;
    [ObservableProperty] private bool _isCategoryEditOpen;
    [ObservableProperty] private string _productName = "";
    [ObservableProperty] private decimal _productPrice;
    [ObservableProperty] private int _productQuantity;
    [ObservableProperty] private int _productReorderLevel;
    [ObservableProperty] private Category? _productFormCategory;
    [ObservableProperty] private string _categoryName = "";
    [ObservableProperty] private string _exportImportMessage = "";

    public string Title => "Menu management";

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
            Products = new ObservableCollection<MenuItem>(items);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void OpenAddProduct()
    {
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
        if (string.IsNullOrWhiteSpace(ProductName)) return;
        var categoryId = ProductFormCategory?.Id ?? 0;
        if (SelectedProduct != null)
        {
            SelectedProduct.Name = ProductName;
            SelectedProduct.SellPrice = ProductPrice;
            SelectedProduct.Quantity = ProductQuantity;
            SelectedProduct.ReorderLevel = ProductReorderLevel;
            SelectedProduct.CategoryId = categoryId;
            await _menuItemRepo.UpdateAsync(SelectedProduct, default).ConfigureAwait(true);
        }
        else
        {
            var m = new MenuItem { Name = ProductName, SellPrice = ProductPrice, Quantity = ProductQuantity, ReorderLevel = ProductReorderLevel, CategoryId = categoryId, CapitalPrice = 0 };
            await _menuItemRepo.AddAsync(m, default).ConfigureAwait(true);
            Products.Add(m);
        }
        IsProductEditOpen = false;
        _ = LoadAsync();
    }

    [RelayCommand]
    private void CloseProductEdit() => IsProductEditOpen = false;

    [RelayCommand]
    private void OpenAddCategory()
    {
        CategoryName = "";
        IsCategoryEditOpen = true;
    }

    [RelayCommand]
    private async Task SaveCategoryAsync()
    {
        if (string.IsNullOrWhiteSpace(CategoryName)) return;
        var c = new Category { Name = CategoryName };
        await _categoryRepo.AddAsync(c, default).ConfigureAwait(true);
        Categories.Add(c);
        IsCategoryEditOpen = false;
    }

    [RelayCommand]
    private void CloseCategoryEdit() => IsCategoryEditOpen = false;

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
            await System.IO.File.WriteAllTextAsync(path, sb.ToString(), Encoding.UTF8).ConfigureAwait(true);
            ExportImportMessage = "Exported " + items.Count + " products to " + System.IO.Path.GetFileName(path);
        }
        catch (Exception ex)
        {
            ExportImportMessage = "Export failed: " + ex.Message;
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
        if (stream == null) return;
        IsLoading = true;
        try
        {
            using var reader = new System.IO.StreamReader(stream, Encoding.UTF8);
            var header = await reader.ReadLineAsync().ConfigureAwait(true);
            if (string.IsNullOrEmpty(header) || !header.Contains("ProductName", StringComparison.OrdinalIgnoreCase))
            {
                ExportImportMessage = "Invalid CSV: expected header with CategoryName, ProductName, Price, Quantity, ReorderLevel";
                return;
            }
            var cats = await _categoryRepo.GetAllAsync(default).ConfigureAwait(true);
            var nameToCat = cats.ToDictionary(c => (c.Name ?? "").Trim(), c => c, StringComparer.OrdinalIgnoreCase);
            var items = await _menuItemRepo.GetAllAsync(default).ConfigureAwait(true);
            var existingByKey = items.GroupBy(m => (CategoryId: m.CategoryId, Name: (m.Name ?? "").Trim())).ToDictionary(g => g.Key, g => g.First());
            int added = 0, updated = 0, errors = 0;
            string? line;
            while ((line = await reader.ReadLineAsync().ConfigureAwait(true)) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var cols = ParseCsvLine(line);
                if (cols.Count < 3) { errors++; continue; }
                var catName = cols.Count > 0 ? cols[0].Trim() : "";
                var productName = cols.Count > 1 ? cols[1].Trim() : "";
                if (string.IsNullOrEmpty(productName)) { errors++; continue; }
                if (!decimal.TryParse(cols.Count > 2 ? cols[2] : "", NumberStyles.Any, CultureInfo.InvariantCulture, out var price)) price = 0;
                var qty = int.TryParse(cols.Count > 3 ? cols[3] : "0", out var q) ? q : 0;
                var reorder = int.TryParse(cols.Count > 4 ? cols[4] : "0", out var r) ? r : 0;
                if (!nameToCat.TryGetValue(catName, out var category))
                {
                    var newCat = new Category { Name = catName };
                    await _categoryRepo.AddAsync(newCat, default).ConfigureAwait(true);
                    nameToCat[catName] = newCat;
                    category = newCat;
                }
                var key = (CategoryId: category.Id, Name: productName);
                if (existingByKey.TryGetValue(key, out var existing))
                {
                    existing.SellPrice = price;
                    existing.Quantity = qty;
                    existing.ReorderLevel = reorder;
                    await _menuItemRepo.UpdateAsync(existing, default).ConfigureAwait(true);
                    updated++;
                }
                else
                {
                    var m = new MenuItem { Name = productName, SellPrice = price, Quantity = qty, ReorderLevel = reorder, CategoryId = category.Id, CapitalPrice = 0 };
                    await _menuItemRepo.AddAsync(m, default).ConfigureAwait(true);
                    existingByKey[(CategoryId: m.CategoryId, Name: productName)] = m;
                    added++;
                }
            }
            ExportImportMessage = $"Import done: {added} added, {updated} updated" + (errors > 0 ? $", {errors} rows skipped" : "");
            _ = LoadAsync();
        }
        catch (Exception ex)
        {
            ExportImportMessage = "Import failed: " + ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private static string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        if (value.Contains(',', StringComparison.Ordinal) || value.Contains('"', StringComparison.Ordinal))
            return "\"" + value.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"";
        return value;
    }

    private static List<string> ParseCsvLine(string line)
    {
        var list = new List<string>();
        var i = 0;
        while (i < line.Length)
        {
            if (line[i] == '"')
            {
                var end = i + 1;
                while (end < line.Length && (line[end] != '"' || (end + 1 < line.Length && line[end + 1] == '"'))) end += line[end] == '"' ? 2 : 1;
                list.Add(line[(i + 1)..(end < line.Length ? end : line.Length)].Replace("\"\"", "\"", StringComparison.Ordinal));
                i = end >= line.Length ? line.Length : end + 1;
                if (i < line.Length && line[i] == ',') i++;
            }
            else
            {
                var comma = line.IndexOf(',', i);
                if (comma < 0) { list.Add(line[i..].Trim()); break; }
                list.Add(line[i..comma].Trim());
                i = comma + 1;
            }
        }
        return list;
    }
}
