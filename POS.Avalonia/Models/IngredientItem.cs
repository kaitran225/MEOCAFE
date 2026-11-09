using CommunityToolkit.Mvvm.ComponentModel;

namespace POS.Avalonia.Models;

/// <summary>A single ingredient shown in the recipe detail, with a highlight state
/// driven by clicking a [[mention]] chip in a step.</summary>
public partial class IngredientItem : ObservableObject
{
    [ObservableProperty] private string _text = "";

    [ObservableProperty] private bool _isHighlighted;
}
