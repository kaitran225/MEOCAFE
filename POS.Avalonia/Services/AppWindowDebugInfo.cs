using System.ComponentModel;

namespace POS.Avalonia.Services;

/// <summary>Exposes current main window dimensions for debug display. Updated by MainWindow.</summary>
public sealed class AppWindowDebugInfo : INotifyPropertyChanged
{
    private int _width;
    private int _height;
    private bool _isFullScreen;

    public int Width
    {
        get => _width;
        private set
        {
            if (_width == value) return;
            _width = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Width)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DimensionText)));
        }
    }

    public int Height
    {
        get => _height;
        private set
        {
            if (_height == value) return;
            _height = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Height)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DimensionText)));
        }
    }

    public bool IsFullScreen
    {
        get => _isFullScreen;
        private set
        {
            if (_isFullScreen == value) return;
            _isFullScreen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFullScreen)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DimensionText)));
        }
    }

    public string DimensionText => IsFullScreen ? "Full screen" : $"{Width} × {Height}";

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Update(int width, int height, bool isFullScreen = false)
    {
        IsFullScreen = isFullScreen;
        Width = width;
        Height = height;
    }
}
