using System;
using Avalonia;

namespace POS.Avalonia.Services;

/// <summary>Fixed component sizes per preset (480, 720, 1080, 2K). No dynamic scaling.</summary>
public static class UiSizePresetValues
{
    public static double GetDouble(string key, string sizeKey)
    {
        var i = Index(sizeKey);
        return key switch
        {
            "CaptionFontSize" => new[] { 10.0, 11.0, 12.0, 14.0 }[i],
            "BodyFontSize" => new[] { 12.0, 13.0, 14.0, 16.0 }[i],
            "SubtitleFontSize" => new[] { 14.0, 15.0, 16.0, 18.0 }[i],
            "TitleFontSize" => new[] { 16.0, 18.0, 20.0, 22.0 }[i],
            "LargeTitleFontSize" => new[] { 20.0, 22.0, 24.0, 28.0 }[i],
            "Spacing4" => new[] { 2.0, 3.0, 4.0, 6.0 }[i],
            "Spacing8" => new[] { 4.0, 6.0, 8.0, 12.0 }[i],
            "Spacing16" => new[] { 8.0, 10.0, 16.0, 20.0 }[i],
            "Spacing24" => new[] { 12.0, 14.0, 24.0, 28.0 }[i],
            "Spacing32" => new[] { 16.0, 20.0, 32.0, 40.0 }[i],
            _ => 0
        };
    }

    public static Thickness GetThickness(string key, string sizeKey)
    {
        var i = Index(sizeKey);
        return key switch
        {
            "Margin4" => new[] { new Thickness(2), new Thickness(3), new Thickness(4), new Thickness(6) }[i],
            "Margin8" => new[] { new Thickness(4), new Thickness(6), new Thickness(8), new Thickness(12) }[i],
            "Margin16" => new[] { new Thickness(8), new Thickness(10), new Thickness(16), new Thickness(20) }[i],
            "Margin24" => new[] { new Thickness(12), new Thickness(14), new Thickness(24), new Thickness(28) }[i],
            "Margin32" => new[] { new Thickness(16), new Thickness(20), new Thickness(32), new Thickness(40) }[i],
            _ => new Thickness(0)
        };
    }

    public static CornerRadius GetCornerRadius(string key, string sizeKey)
    {
        var i = Index(sizeKey);
        return key switch
        {
            "CornerRadius4" => new[] { new CornerRadius(2), new CornerRadius(3), new CornerRadius(4), new CornerRadius(6) }[i],
            "CornerRadius8" => new[] { new CornerRadius(4), new CornerRadius(6), new CornerRadius(8), new CornerRadius(10) }[i],
            "CornerRadius12" => new[] { new CornerRadius(6), new CornerRadius(8), new CornerRadius(12), new CornerRadius(14) }[i],
            "CornerRadius16" => new[] { new CornerRadius(8), new CornerRadius(10), new CornerRadius(16), new CornerRadius(18) }[i],
            _ => new CornerRadius(0)
        };
    }

    private static int Index(string sizeKey)
    {
        return sizeKey switch
        {
            UiSizePresets.Key480 => 0,
            UiSizePresets.Key720 => 1,
            UiSizePresets.Key1080 => 2,
            UiSizePresets.Key2K => 3,
            _ => 2
        };
    }

    /// <summary>Apply all size-dependent resources for the given preset to the app. Call on startup and when user changes size.</summary>
    public static void ApplyToApplication(string sizeKey)
    {
        var app = Application.Current;
        if (app?.Resources == null) return;

        var r = app.Resources;
        r["CaptionFontSize"] = GetDouble("CaptionFontSize", sizeKey);
        r["BodyFontSize"] = GetDouble("BodyFontSize", sizeKey);
        r["SubtitleFontSize"] = GetDouble("SubtitleFontSize", sizeKey);
        r["TitleFontSize"] = GetDouble("TitleFontSize", sizeKey);
        r["LargeTitleFontSize"] = GetDouble("LargeTitleFontSize", sizeKey);
        r["Spacing4"] = GetDouble("Spacing4", sizeKey);
        r["Spacing8"] = GetDouble("Spacing8", sizeKey);
        r["Spacing16"] = GetDouble("Spacing16", sizeKey);
        r["Spacing24"] = GetDouble("Spacing24", sizeKey);
        r["Spacing32"] = GetDouble("Spacing32", sizeKey);
        r["Margin4"] = GetThickness("Margin4", sizeKey);
        r["Margin8"] = GetThickness("Margin8", sizeKey);
        r["Margin16"] = GetThickness("Margin16", sizeKey);
        r["Margin24"] = GetThickness("Margin24", sizeKey);
        r["Margin32"] = GetThickness("Margin32", sizeKey);
        r["CornerRadius4"] = GetCornerRadius("CornerRadius4", sizeKey);
        r["CornerRadius8"] = GetCornerRadius("CornerRadius8", sizeKey);
        r["CornerRadius12"] = GetCornerRadius("CornerRadius12", sizeKey);
        r["CornerRadius16"] = GetCornerRadius("CornerRadius16", sizeKey);
    }
}
