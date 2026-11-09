using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace POS.Avalonia.Converters;

public class TableStatusToBrushConverter : IValueConverter
{
    public IBrush? AvailableBrush { get; set; }
    public IBrush? OccupiedBrush { get; set; }
    public IBrush? ReservedBrush { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var s = value?.ToString()?.ToLowerInvariant() ?? "";
        if (s == "reserved") return ReservedBrush;
        if (s == "occupied" || s == "busy") return OccupiedBrush;
        return AvailableBrush; // available, empty, etc.
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
