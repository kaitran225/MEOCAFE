using System.Collections.Generic;

namespace POS.Avalonia.Services;

/// <summary>Fixed UI size presets: 480p, 720p, 1080p, 2K. Used by Settings and MainWindow.</summary>
public static class UiSizePresets
{
    public const string Key480 = "480";
    public const string Key720 = "720";
    public const string Key1080 = "1080";
    public const string Key2K = "2K";
    public const string KeyFullScreen = "Full screen";

    public static readonly IReadOnlyList<string> AllKeys = new[] { Key480, Key720, Key1080, Key2K, KeyFullScreen };

    /// <summary>Returns (width, height) for the preset. Full screen returns (0,0) and is handled separately.</summary>
    public static (int Width, int Height) GetDimensions(string key)
    {
        return key switch
        {
            Key480 => (854, 480),
            Key720 => (1280, 720),
            Key1080 => (1920, 1080),
            Key2K => (2560, 1440),
            KeyFullScreen => (0, 0),
            _ => (1920, 1080)
        };
    }

    public static bool IsFullScreen(string key) => key == KeyFullScreen;

    public static int MinWidth => 854;
    public static int MinHeight => 480;
}
