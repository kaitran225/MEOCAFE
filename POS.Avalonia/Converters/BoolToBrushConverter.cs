using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace POS.Avalonia.Converters;

public class BoolToBrushConverter : IValueConverter
{
    public IBrush? WhenTrue { get; set; }
    public IBrush? WhenFalse { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? WhenTrue : WhenFalse;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
