using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace POS.Avalonia.Controls;

public partial class PaginationControl : UserControl
{
    public static readonly StyledProperty<string?> SummaryProperty =
        AvaloniaProperty.Register<PaginationControl, string?>(nameof(Summary));

    public static readonly StyledProperty<ICommand?> PrevCommandProperty =
        AvaloniaProperty.Register<PaginationControl, ICommand?>(nameof(PrevCommand));

    public static readonly StyledProperty<ICommand?> NextCommandProperty =
        AvaloniaProperty.Register<PaginationControl, ICommand?>(nameof(NextCommand));

    public string? Summary
    {
        get => GetValue(SummaryProperty);
        set => SetValue(SummaryProperty, value);
    }

    public ICommand? PrevCommand
    {
        get => GetValue(PrevCommandProperty);
        set => SetValue(PrevCommandProperty, value);
    }

    public ICommand? NextCommand
    {
        get => GetValue(NextCommandProperty);
        set => SetValue(NextCommandProperty, value);
    }
}
