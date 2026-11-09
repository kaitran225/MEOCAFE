using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace POS.Avalonia.Converters;

public class EqualityToBrushConverter : IValueConverter
{
    public IBrush? SelectedBrush { get; set; }
    public IBrush? UnselectedBrush { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var equal = string.Equals(value?.ToString(), parameter?.ToString(), StringComparison.Ordinal);
        return equal ? SelectedBrush : UnselectedBrush;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
