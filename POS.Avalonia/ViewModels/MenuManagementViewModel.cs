using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Core.Contracts;
using POS.Core.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class MenuManagementViewModel : ViewModelBase
{
    private readonly ICategoryRepository _categoryRepo;
    private readonly IMenuItemRepository _menuItemRepo;

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

    public string Title => "Menu management";

    public MenuManagementViewModel(ICategoryRepository categoryRepo, IMenuItemRepository menuItemRepo)
    {
        _categoryRepo = categoryRepo;
        _menuItemRepo = menuItemRepo;
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
        // Stub: Phase 10.14 - wire to CSV/Excel export
        await Task.CompletedTask.ConfigureAwait(true);
    }

    [RelayCommand]
    private async Task ImportProductsAsync()
    {
        // Stub: Phase 10.13 - wire to CSV/Excel import
        await Task.CompletedTask.ConfigureAwait(true);
    }
}
