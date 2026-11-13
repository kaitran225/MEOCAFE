using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace POS.Avalonia.Controls;

public partial class KpiCard : UserControl
{
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<KpiCard, string?>(nameof(Title));

    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<KpiCard, string?>(nameof(Value));

    public static readonly StyledProperty<string?> SubtitleProperty =
        AvaloniaProperty.Register<KpiCard, string?>(nameof(Subtitle));

    public static readonly StyledProperty<IBrush?> AccentBrushProperty =
        AvaloniaProperty.Register<KpiCard, IBrush?>(nameof(AccentBrush));

    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<KpiCard, object?>(nameof(Icon));

    public static readonly StyledProperty<string?> TrendTextProperty =
        AvaloniaProperty.Register<KpiCard, string?>(nameof(TrendText));

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public string? Subtitle
    {
        get => GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public IBrush? AccentBrush
    {
        get => GetValue(AccentBrushProperty);
        set => SetValue(AccentBrushProperty, value);
    }

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string? TrendText
    {
        get => GetValue(TrendTextProperty);
        set => SetValue(TrendTextProperty, value);
    }

    public KpiCard()
    {
        InitializeComponent();
        AccentBrush = new SolidColorBrush(Color.Parse("#BAD344"));
    }
}
