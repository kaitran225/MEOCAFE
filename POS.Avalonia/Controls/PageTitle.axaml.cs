using Avalonia;
using Avalonia.Controls;
using Avalonia.Metadata;

namespace POS.Avalonia.Controls;

public partial class PageTitle : UserControl
{
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<PageTitle, string?>(nameof(Title));

    public static readonly StyledProperty<object?> ActionContentProperty =
        AvaloniaProperty.Register<PageTitle, object?>(nameof(ActionContent));

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    [Content]
    public object? ActionContent
    {
        get => GetValue(ActionContentProperty);
        set => SetValue(ActionContentProperty, value);
    }
}
