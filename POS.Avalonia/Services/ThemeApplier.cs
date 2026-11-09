using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;
using POS.Avalonia.Converters;

namespace POS.Avalonia.Services;

public static class ThemeApplier
{
    public const string KeyDark = "Dark";
    public const string KeyLight = "Light";

    public static void Apply(string? themeKey)
    {
        if (Application.Current is null) return;

        var variant = themeKey == KeyLight ? ThemeVariant.Light : ThemeVariant.Dark;
        Application.Current.RequestedThemeVariant = variant;
        SyncConverterBrushes(variant);
    }

    /// <summary>
    /// Converter instances are not AvaloniaObjects, so they cannot hold a DynamicResource.
    /// Anything that carries a theme color inside a converter is refreshed here; new code
    /// should use Classes + Style selectors instead.
    /// </summary>
    private static void SyncConverterBrushes(ThemeVariant variant)
    {
        var resources = Application.Current?.Resources;
        if (resources is null) return;

        if (!resources.TryGetResource("EqualityToBrushConverter", variant, out var value) ||
            value is not EqualityToBrushConverter converter)
            return;

        if (resources.TryGetResource("PrimaryBrush", variant, out var selected) && selected is IBrush selectedBrush)
            converter.SelectedBrush = selectedBrush;
        if (resources.TryGetResource("SurfaceMutedBrush", variant, out var unselected) && unselected is IBrush unselectedBrush)
            converter.UnselectedBrush = unselectedBrush;
    }
}
