using Avalonia;
using Avalonia.Controls;
using Avalonia.Metadata;

namespace POS.Avalonia.Controls;

public partial class ModalOverlay : UserControl
{
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<ModalOverlay, bool>(nameof(IsOpen));

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<ModalOverlay, string?>(nameof(Title));

    public static readonly StyledProperty<object?> DialogContentProperty =
        AvaloniaProperty.Register<ModalOverlay, object?>(nameof(DialogContent));

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    [Content]
    public object? DialogContent
    {
        get => GetValue(DialogContentProperty);
        set => SetValue(DialogContentProperty, value);
    }
}
