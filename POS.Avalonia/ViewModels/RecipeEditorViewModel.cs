using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS.Avalonia.Models;
using POS.Core.Contracts;
using POS.Core.Models;

namespace POS.Avalonia.ViewModels;

/// <summary>Editable ingredient row in the recipe editor.</summary>
public partial class IngredientEditorItem : ObservableObject
{
    [ObservableProperty] private string _text = "";

    public IngredientEditorItem(string text = "") => Text = text;
}

/// <summary>Editable rich-text step in the recipe editor.</summary>
public partial class StepEditorItem : ObservableObject
{
    [ObservableProperty] private string _html = "";

    public StepEditorItem(string html = "") => Html = html;
}

/// <summary>
/// Modal editor for a drink recipe. Steps are rich text (safe HTML) and can reference the
/// recipe's ingredients via [[name]] mentions. Saving upserts the recipe through the repository.
/// </summary>
public partial class RecipeEditorViewModel : ViewModelBase
{
    private readonly IRecipeRepository _recipeRepo;
    private readonly IMenuItemRepository _menuItemRepo;
    private int _editRecipeId;

    /// <summary>Raised after a recipe is successfully saved so the parent can reload its list.</summary>
    public event Action? Saved;

    [ObservableProperty] private ObservableCollection<MenuItem> _menuItems = new();
    [ObservableProperty] private MenuItem? _selectedMenuItem;
    [ObservableProperty] private string _variantNote = "";
    [ObservableProperty] private ObservableCollection<IngredientEditorItem> _ingredients = new();
    [ObservableProperty] private ObservableCollection<string> _ingredientNames = new();
    [ObservableProperty] private ObservableCollection<StepEditorItem> _steps = new();
    [ObservableProperty] private bool _isOpen;
    [ObservableProperty] private bool _isSaving;
    [ObservableProperty] private string _title = "New Recipe";
    [ObservableProperty] private string _error = "";

    public RecipeEditorViewModel(IRecipeRepository recipeRepo, IMenuItemRepository menuItemRepo)
    {
        _recipeRepo = recipeRepo;
        _menuItemRepo = menuItemRepo;
        _ = LoadMenuItemsAsync();
    }

    private async Task LoadMenuItemsAsync()
    {
        var items = await _menuItemRepo.GetAllAsync(default).ConfigureAwait(true);
        MenuItems = new ObservableCollection<MenuItem>(items.OrderBy(m => m.Name));
        if (SelectedMenuItem is null && !string.IsNullOrEmpty(Error))
            SelectedMenuItem = MenuItems.FirstOrDefault();
        else if (SelectedMenuItem is null)
            SelectedMenuItem = MenuItems.FirstOrDefault();
    }

    /// <summary>Open the editor for a brand-new recipe (user picks the menu item).</summary>
    public void OpenNew()
    {
        _editRecipeId = 0;
        Title = "New Recipe";
        VariantNote = "";
        Ingredients = new ObservableCollection<IngredientEditorItem>();
        Steps = new ObservableCollection<StepEditorItem>();
        SyncIngredientNames();
        if (MenuItems.Count == 0)
            _ = LoadMenuItemsAsync();
        SelectedMenuItem = MenuItems.FirstOrDefault();
        Error = "";
        IsOpen = true;
    }

    /// <summary>Open the editor pre-filled from an existing recipe.</summary>
    public void OpenFor(RecipeListItem item)
    {
        if (item?.Recipe is null) return;
        _editRecipeId = item.Recipe.Id;
        Title = $"Edit — {item.MenuItemName}";
        SelectedMenuItem = MenuItems.FirstOrDefault(m => m.Id == item.Recipe.MenuItemId);
        VariantNote = item.Recipe.VariantNote ?? "";
        Ingredients = new ObservableCollection<IngredientEditorItem>(
            (item.Recipe.Ingredients ?? new List<string>()).Select(i => new IngredientEditorItem(i ?? "")));
        Steps = new ObservableCollection<StepEditorItem>(
            (item.Recipe.Steps ?? new List<string>()).Select(s => new StepEditorItem(s ?? "")));
        SyncIngredientNames();
        Error = "";
        IsOpen = true;
    }

    [RelayCommand]
    private void Close() => IsOpen = false;

    [RelayCommand]
    private void AddIngredient() => AddIngredientRow("");

    [RelayCommand]
    private void RemoveIngredient(IngredientEditorItem? item)
    {
        if (item is null) return;
        Ingredients.Remove(item);
        SyncIngredientNames();
    }

    [RelayCommand]
    private void MoveIngredientUp(IngredientEditorItem? item) => MoveIngredient(item, -1);

    [RelayCommand]
    private void MoveIngredientDown(IngredientEditorItem? item) => MoveIngredient(item, +1);

    private void MoveIngredient(IngredientEditorItem? item, int delta)
    {
        if (item is null) return;
        var idx = Ingredients.IndexOf(item);
        var target = idx + delta;
        if (idx < 0 || target < 0 || target >= Ingredients.Count) return;
        Ingredients.Move(idx, target);
        SyncIngredientNames();
    }

    private void AddIngredientRow(string text)
    {
        var row = new IngredientEditorItem(text);
        row.PropertyChanged += (_, e) => { if (e.PropertyName == nameof(IngredientEditorItem.Text)) SyncIngredientNames(); };
        Ingredients.Add(row);
        SyncIngredientNames();
    }

    private void SyncIngredientNames()
    {
        var names = Ingredients
            .Select(i => i.Text?.Trim())
            .OfType<string>()
            .Where(n => n.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        IngredientNames = new ObservableCollection<string>(names);
    }

    [RelayCommand]
    private void AddStep() => Steps.Add(new StepEditorItem(""));

    [RelayCommand]
    private void RemoveStep(StepEditorItem? item)
    {
        if (item is not null) Steps.Remove(item);
    }

    [RelayCommand]
    private void MoveStepUp(StepEditorItem? item) => MoveStep(item, -1);

    [RelayCommand]
    private void MoveStepDown(StepEditorItem? item) => MoveStep(item, +1);

    private void MoveStep(StepEditorItem? item, int delta)
    {
        if (item is null) return;
        var idx = Steps.IndexOf(item);
        var target = idx + delta;
        if (idx < 0 || target < 0 || target >= Steps.Count) return;
        Steps.Move(idx, target);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (SelectedMenuItem is null)
        {
            Error = "Choose a menu item for this recipe.";
            return;
        }

        var ingredients = Ingredients
            .Select(i => i.Text?.Trim() ?? "")
            .Where(s => s.Length > 0)
            .ToList();
        var steps = Steps
            .Select(s => s.Html?.Trim() ?? "")
            .Where(s => s.Length > 0)
            .ToList();
        if (ingredients.Count == 0 && steps.Count == 0)
        {
            Error = "Add at least one ingredient or one step.";
            return;
        }

        IsSaving = true;
        Error = "";
        try
        {
            var recipe = new DrinkRecipe
            {
                Id = _editRecipeId,
                MenuItemId = SelectedMenuItem.Id,
                VariantNote = string.IsNullOrWhiteSpace(VariantNote) ? null : VariantNote.Trim(),
                Ingredients = ingredients,
                Steps = steps
            };
            await _recipeRepo.AddOrUpdateAsync(recipe, default).ConfigureAwait(true);
            IsOpen = false;
            Saved?.Invoke();
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            IsSaving = false;
        }
    }
}
