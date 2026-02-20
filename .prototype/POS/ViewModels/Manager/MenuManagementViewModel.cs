using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using POS.Models;
using POS.Services.Dao;
using POS.ViewModels;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Microsoft.UI.Xaml.Controls;
using POS;

namespace POS.ViewModels.Manager;

public partial class MenuManagementViewModel : ObservableRecipient
{
    public event EventHandler<string>? ErrorOccurred;
    private readonly IDao _dao;  // Injected DAO service
    private string _searchTerm = "";
    private string _currentFilter = "All";
    private bool _isPaneOpen;

    private Category _selectedCategory;
    private List<Discount> _discounts;
    private Discount _selectedDiscount;
    private ItemViewModel _selectedItemViewModel;
    public ObservableCollection<ItemViewModel> SelectedComboItems { get; set; } = new();
    public ObservableCollection<ItemViewModel> EditSelectedComboItems { get; set; } = new();
    public bool Edited { get; set; } = false;

    // Observable collections
    private ObservableCollection<Category> _categories;
    public ObservableCollection<Category> Categories
    {
        get => _categories;
        set => SetProperty(ref _categories, value);
    }

    public ObservableCollection<ComboItem> ComboItems { get; set; } = new();
    public ObservableCollection<ItemViewModel> ItemViewModels { get; set; } = new();
    public ObservableCollection<ItemViewModel> FilteredItemViewModels { get; } = new();

    public List<Discount> Discounts
    {
        get => _discounts;
        set => SetProperty(ref _discounts, value);
    }
    public string SearchTerm
    {
        get => _searchTerm;
        set
        {
            if (SetProperty(ref _searchTerm, value))
            {
                FilterMenuItems();
            }
        }
    }

    public Discount SelectedDiscount
    {
        get => _selectedDiscount;
        set => SetProperty(ref _selectedDiscount, value);
    }

    public Category SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    public ItemViewModel SelectedItem
    {
        get => _selectedItemViewModel;
        set => SetProperty(ref _selectedItemViewModel, value);
    }

    public bool IsPaneOpen
    {
        get => _isPaneOpen;
        set => SetProperty(ref _isPaneOpen, value);
    }

    public MenuManagementViewModel(IDao dao)
    {
        _dao = dao;
        LoadCategories();
        LoadMenuItems();
        LoadComboMenuItems();
        LoadDiscounts();
        AddPlaceholderItem();
        ApplyFilter(_currentFilter);
    }

    private void LoadCategories()
    {
        // Use DAO to get categories
        var tempCategories = _dao.GetCategories();

        foreach (var category in tempCategories)
        {
            // Load associated menu items for each category
            category.MenuItems = _dao.GetMenuItems().Where(m => m.CategoryId == category.Id).ToList();
        }

        _categories = new ObservableCollection<Category>(tempCategories);
    }

    private void LoadMenuItems()
    {
        var menuItems = _dao.GetMenuItems();
        foreach (var item in menuItems)
        {
            item.Discount = LoadDiscountByMenuItemId(item.Id);
            var menuItemViewModel = new ItemViewModel(item);
            ItemViewModels.Add(menuItemViewModel);
        }
    }

    private void LoadComboMenuItems()
    {
        var comboMenuItems = _dao.GetComboMenuItems();
        foreach (var item in comboMenuItems)
        {
            var menuItemViewModel = new ItemViewModel(item);
            ItemViewModels.Add(menuItemViewModel);
        }
    }

    public List<ComboItem> LoadComboItemsByComboId(int id)
    {
        return _dao.GetComboItemsByComboMenuItemsId(id);
    }

    public void LoadDiscounts()
    {
        Discounts = _dao.GetDiscounts().Where(discount => discount.IsActive).ToList();
    }

    public Discount? LoadDiscountByMenuItemId(int id)
    {
        return _dao.GetDiscount(id);
    }
    public async Task AddCategoryAsync(string categoryName)
    {
        // Use DAO to add category
        var newCategory = new Category { Name = categoryName };
        await _dao.AddCategoryAsync(newCategory);

        _categories.Add(newCategory);
    }

    public async Task AddMenuItemDiscount(int menuItemId, int discountId)
    {
        await _dao.AddMenuItemDiscountAsync(discountId, menuItemId);
    }

    public async Task DeleteCategoryAsync(int categoryId)
    {
        await _dao.DeleteCategoryAsync(categoryId);

        // Remove the category from the in-memory collection
        var categoryToDelete = _categories.FirstOrDefault(category => category.Id == categoryId);
        if (categoryToDelete != null)
        {
            _categories.Remove(categoryToDelete);
        }

        // Remove associated menu items
        var itemsToRemove = ItemViewModels.Where(item => item.CategoryId == categoryId).ToList();
        foreach (var item in itemsToRemove)
        {
            ItemViewModels.Remove(item);
        }

        ApplyCategoryFilter(_currentFilter);
        ApplyFilter(_currentFilter);
    }

    public async Task DeleteItemAsync(ItemViewModel item)
    {
        if (item.Item is MenuItem)
        {
            await _dao.DeleteMenuItemAsync(item.ItemId);
        }
        else
        {
            await _dao.DeleteComboMenuItemAsync(item.ItemId);
        }

        // Remove the item from the collection
        ItemViewModels.Remove(item);
        ApplyFilter(_currentFilter);
    }

    public async Task DeleteMenuItemDiscount(int menuItemId)
    {
        await _dao.DeleteMenuItemDiscountAsync(menuItemId);
    }

    public async Task AddItemAsync(ItemViewModel item, Image? image = null)
    {

        // Prepare image data if available
        var imageBytes = await ToBase64(image);

        if (item.Item is MenuItem)
        {
            MenuItem newMenuItem = new MenuItem
            {
                Name = item.Name,
                SellPrice = item.SellPrice,
                CapitalPrice = item.CapitalPrice,
                Quantity = item.Quantity,
                Image = imageBytes,
                CategoryId = item.CategoryId
            };

            await _dao.AddMenuItemAsync(newMenuItem);
            ItemViewModels.Add(new ItemViewModel(newMenuItem));
        }
        else
        {
            ComboMenuItem newComboMenuItem = new ComboMenuItem
            {
                Name = item.Name,
                Description = item.Description,
                Quantity = item.Quantity,
                SellPrice = item.SellPrice,
                Image = imageBytes
            };

            await _dao.AddComboMenuItemAsync(newComboMenuItem);
            foreach (var selectedComboItem in SelectedComboItems)
            {
                ComboItem newComboItem = new ComboItem
                {
                    ComboMenuItemId = newComboMenuItem.Id,
                    MenuItemId = selectedComboItem.ItemId
                };
                await _dao.AddComboItemAsync(newComboItem);
            }
            ItemViewModels.Add(new ItemViewModel(newComboMenuItem));
        }

        // Reapply filter if necessary
        ApplyFilter(_currentFilter);
        
    }

    public async Task EditMenuItemAsync(ItemViewModel item, string newName, decimal newSellPrice, decimal newCapitalPrice, int newQuantity, Image? newImage)
    {
        byte[]? imageBytes = null;
        if (newImage != null)
        {
            imageBytes = await ToBase64(newImage);
        }

        var updatedMenuItem = new MenuItem
        {
            Id = item.ItemId,
            Name = newName,
            SellPrice = newSellPrice,
            CapitalPrice = newCapitalPrice,
            Quantity = newQuantity,
            Image = imageBytes,
            CategoryId = item.CategoryId
        };

        // Use DAO to update the menu item
        await _dao.EditMenuItemAsync(updatedMenuItem);

        if (SelectedDiscount != null)
        {
            if (SelectedDiscount.Name != "No discount")
            {
                await AddMenuItemDiscount(item.ItemId, SelectedDiscount.Id);
            }
            else
            {
                await DeleteMenuItemDiscount(item.ItemId);
            }
            item.Discount = LoadDiscountByMenuItemId(item.ItemId);
        }

        // Update the in-memory model
        item.Name = newName;
        item.SellPrice = newSellPrice;
        item.CapitalPrice = newCapitalPrice;
        item.Quantity = newQuantity;
        item.SaveImage(imageBytes);

        ApplyFilter(_currentFilter);
    }

    public async Task EditComboItemAsync(ItemViewModel item, string newName, decimal newSellPrice,
        string newDescription, int newQuantity, Image? newImage)
    {
        byte[]? imageBytes = null;
        if (newImage != null)
        {
            imageBytes = await ToBase64(newImage);
        }

        var updatedComboMenuItem = new ComboMenuItem
        {
            Id = item.ItemId,
            Name = newName,
            SellPrice = newSellPrice,
            Description = newDescription,
            Quantity = newQuantity,
            Image = imageBytes,
        };

        // Use DAO to update the menu item
        await _dao.EditComboMenuItemAsync(updatedComboMenuItem);

        item.Name = newName;
        item.SellPrice = newSellPrice;
        item.Description = newDescription;
        item.Quantity = newQuantity;
        item.SaveImage(imageBytes);

        if (!Edited && EditSelectedComboItems.Count == 0) return;
        
        await _dao.DeleteComboItemAsync(SelectedItem.ItemId);
        foreach (var editSelectedComboItem in EditSelectedComboItems)
        {
            ComboItem newComboItem = new ComboItem
            {
                ComboMenuItemId = updatedComboMenuItem.Id,
                MenuItemId = editSelectedComboItem.ItemId
            };
            await _dao.AddComboItemAsync(newComboItem);
        }

        // Update the in-memory model

        ApplyFilter(_currentFilter);
    }

    private async Task<byte[]?> ToBase64(Image? control)
    {
        if (control?.Source == null)
        {
            return null;
        }
        var bitmap = new RenderTargetBitmap();
        await bitmap.RenderAsync(control);
        return await ToBase64(bitmap);
    }

    private async Task<byte[]?> ToBase64(RenderTargetBitmap bitmap)
    {
        var bytes = (await bitmap.GetPixelsAsync()).ToArray();
        return await ToBase64(bytes, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight);
    }

    private async Task<byte[]?> ToBase64(byte[] image, uint height, uint width, double dpiX = 96, double dpiY = 96)
    {
        // encode image
        var encoded = new InMemoryRandomAccessStream();
        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, encoded);
        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, height, width, dpiX, dpiY, image);
        await encoder.FlushAsync();
        encoded.Seek(0);

        // read bytes
        var bytes = new byte[encoded.Size];
        await encoded.AsStream().ReadAsync(bytes, 0, bytes.Length);

        // create base64
        return bytes;
    }

    private void FilterMenuItems()
    {
        // Start with the filtered items from the current category filter
        var filteredItems = ApplyCategoryFilter(_currentFilter);

        // If there is a search term, apply the search filter to the category-filtered items
        if (!string.IsNullOrWhiteSpace(_searchTerm))
        {
            filteredItems = filteredItems
                .Where(item => item.Name.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // Update the filtered items
        UpdateFilteredMenuItems(filteredItems);
    }


    private IEnumerable<ItemViewModel> ApplyCategoryFilter(string filterOption)
    {
        switch (filterOption)
        {
            // Apply the category filter
            case "All":
                return ItemViewModels.Where(item => !item.IsPlaceholder);
            case "Combo":
                return ItemViewModels.Where(item => item.Item is ComboMenuItem);
            default:
            {
                var selectedCategory = _categories.FirstOrDefault(c => c.Name == filterOption);
                return selectedCategory == null ? ItemViewModels.Where(item => !item.IsPlaceholder) : ItemViewModels.Where(item => item.CategoryId == selectedCategory.Id && !item.IsPlaceholder);
            }
        }
    }

    public void ApplyFilter(string filterOption)
    {
        _currentFilter = filterOption;
        FilterMenuItems(); // Apply both category and search filters
    }

    private void UpdateFilteredMenuItems(IEnumerable<ItemViewModel> items)
    {
        FilteredItemViewModels.Clear();

        // Add the filtered items to the collection
        foreach (var item in items)
        {
            FilteredItemViewModels.Add(item);
        }

        // Ensure the placeholder is added if not already in the filtered list
        var placeholderItem = ItemViewModels.FirstOrDefault(i => i.IsPlaceholder);
        if (placeholderItem != null && !FilteredItemViewModels.Contains(placeholderItem))
        {
            FilteredItemViewModels.Add(placeholderItem);
        }
    }
    private void AddPlaceholderItem()
    {
        var placeholder = new MenuItem { Name = "Add New", IsPlaceholder = true };
        ItemViewModels.Add(new ItemViewModel(placeholder));
    }
}
