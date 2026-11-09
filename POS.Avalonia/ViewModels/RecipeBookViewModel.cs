using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Avalonia.Models;
using POS.Core.Contracts;
using POS.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace POS.Avalonia.ViewModels;

public partial class RecipeBookViewModel : ViewModelBase
{
    private readonly IRecipeRepository _recipeRepo;
    private readonly IMenuItemRepository _menuItemRepo;
    private readonly ICategoryRepository _categoryRepo;

    [ObservableProperty] private ObservableCollection<RecipeListItem> _defaultRecipes = new();
    [ObservableProperty] private ObservableCollection<RecipeListItem> _filteredRecipes = new();
    [ObservableProperty] private ObservableCollection<string> _categories = new();
    [ObservableProperty] private string _selectedCategory = "All";
    [ObservableProperty] private string _searchText = "";
    [ObservableProperty] private RecipeListItem? _selectedRecipeItem;
    [ObservableProperty] private ObservableCollection<IngredientItem> _selectedIngredients = new();
    [ObservableProperty] private bool _isLoading;

    /// <summary>Recipe editor modal.</summary>
    public RecipeEditorViewModel Editor { get; }

    public RecipeBookViewModel(IRecipeRepository recipeRepo, IMenuItemRepository menuItemRepo, ICategoryRepository categoryRepo)
    {
        _recipeRepo = recipeRepo;
        _menuItemRepo = menuItemRepo;
        _categoryRepo = categoryRepo;
        Editor = new RecipeEditorViewModel(recipeRepo, menuItemRepo);
        Editor.Saved += async () => await LoadAsync().ConfigureAwait(true);
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var selectedId = SelectedRecipeItem?.Recipe.Id;
            var recipes = await _recipeRepo.GetAllDefaultsAsync(default).ConfigureAwait(true);
            var items = await _menuItemRepo.GetAllAsync(default).ConfigureAwait(true);
            var categories = await _categoryRepo.GetAllAsync(default).ConfigureAwait(true);
            var list = recipes.Select(r =>
            {
                var menuItem = items.FirstOrDefault(m => m.Id == r.MenuItemId);
                var cat = menuItem != null ? categories.FirstOrDefault(c => c.Id == menuItem.CategoryId) : null;
                return new RecipeListItem
                {
                    Recipe = r,
                    MenuItemName = menuItem?.Name ?? $"Item #{r.MenuItemId}",
                    CategoryName = cat?.Name,
                    CategoryId = menuItem?.CategoryId ?? 0,
                    ImageBytes = menuItem?.Image
                };
            }).ToList();
            DefaultRecipes = new ObservableCollection<RecipeListItem>(list);
            var catNames = list.Select(x => x.CategoryName ?? "—").Where(n => !string.IsNullOrEmpty(n)).Distinct().OrderBy(n => n).ToList();
            Categories = new ObservableCollection<string>(new[] { "All" }.Concat(catNames));
            ApplyFilter();
            if (selectedId.HasValue && selectedId.Value > 0)
                OpenRecipe(FilteredRecipes.FirstOrDefault(r => r.Recipe.Id == selectedId.Value)
                           ?? DefaultRecipes.FirstOrDefault(r => r.Recipe.Id == selectedId.Value));
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSelectedCategoryChanged(string value) => ApplyFilter();
    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        var q = DefaultRecipes.AsEnumerable();
        if (!string.IsNullOrEmpty(SelectedCategory) && SelectedCategory != "All")
            q = q.Where(r => string.Equals(r.CategoryName ?? "—", SelectedCategory, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var term = SearchText.Trim();
            q = q.Where(r => (r.MenuItemName?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false));
        }
        FilteredRecipes = new ObservableCollection<RecipeListItem>(q.ToList());
    }

    [RelayCommand]
    private void OpenRecipe(RecipeListItem? item)
    {
        SelectedRecipeItem = item;
        RebuildIngredients();
    }

    private void RebuildIngredients()
    {
        SelectedIngredients = new ObservableCollection<IngredientItem>(
            (SelectedRecipeItem?.Recipe.Ingredients ?? Enumerable.Empty<string>())
                .Select(i => new IngredientItem { Text = i ?? "" }));
    }

    /// <summary>Highlights the ingredient matching a [[mention]] click in a step.</summary>
    [RelayCommand]
    private void HighlightIngredient(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;
        var target = name.Trim();
        foreach (var ingredient in SelectedIngredients)
            ingredient.IsHighlighted = string.Equals(ingredient.Text?.Trim(), target, StringComparison.OrdinalIgnoreCase);
    }

    [RelayCommand]
    private void EditRecipe(RecipeListItem? item)
    {
        if (item?.Recipe is null) return;
        Editor.OpenFor(item);
    }

    [RelayCommand]
    private void NewRecipe() => Editor.OpenNew();

    [RelayCommand]
    private async Task DeleteRecipeAsync(RecipeListItem? item)
    {
        if (item?.Recipe is null || item.Recipe.Id <= 0) return;
        var id = item.Recipe.Id;
        var ok = await RunCrudAsync(() => _recipeRepo.DeleteByIdAsync(id, default)).ConfigureAwait(true);
        if (!ok) return;
        if (SelectedRecipeItem?.Recipe.Id == id)
            SelectedRecipeItem = null;
        await LoadAsync().ConfigureAwait(true);
    }
}
