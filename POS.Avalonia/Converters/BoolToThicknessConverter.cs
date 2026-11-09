using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia;

namespace POS.Avalonia.Converters;

public class BoolToThicknessConverter : IValueConverter
{
    public double WhenTrue { get; set; } = 2;
    public double WhenFalse { get; set; } = 1;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var v = value is true ? WhenTrue : WhenFalse;
        return new Thickness(v);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
