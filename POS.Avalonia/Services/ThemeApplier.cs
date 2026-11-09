using Avalonia;
using Avalonia.Styling;

namespace POS.Avalonia.Services;

public static class ThemeApplier
{
    public const string KeyDark = "Dark";
    public const string KeyLight = "Light";

    public static void Apply(string? themeKey)
    {
        if (Application.Current is null) return;
        Application.Current.RequestedThemeVariant =
            themeKey == KeyLight ? ThemeVariant.Light : ThemeVariant.Dark;
    }
}
