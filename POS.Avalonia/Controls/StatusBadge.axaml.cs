using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace POS.Avalonia.Controls;

public partial class StatusBadge : UserControl
{
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<StatusBadge, string?>(nameof(Text));

    public static readonly StyledProperty<IBrush?> BadgeBrushProperty =
        AvaloniaProperty.Register<StatusBadge, IBrush?>(nameof(BadgeBrush));

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public IBrush? BadgeBrush
    {
        get => GetValue(BadgeBrushProperty);
        set => SetValue(BadgeBrushProperty, value);
    }

    public StatusBadge()
    {
        InitializeComponent();
        BadgeBrush = new SolidColorBrush(Color.Parse("#6B8E6B"));
    }
}
