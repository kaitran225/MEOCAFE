using System;
using Avalonia;
using Avalonia.Controls;

namespace POS.Avalonia.Controls;

public partial class DateRangePicker : UserControl
{
    public static readonly StyledProperty<DateTimeOffset?> StartDateProperty =
        AvaloniaProperty.Register<DateRangePicker, DateTimeOffset?>(nameof(StartDate));

    public static readonly StyledProperty<DateTimeOffset?> EndDateProperty =
        AvaloniaProperty.Register<DateRangePicker, DateTimeOffset?>(nameof(EndDate));

    public DateTimeOffset? StartDate
    {
        get => GetValue(StartDateProperty);
        set => SetValue(StartDateProperty, value);
    }

    public DateTimeOffset? EndDate
    {
        get => GetValue(EndDateProperty);
        set => SetValue(EndDateProperty, value);
    }
}
