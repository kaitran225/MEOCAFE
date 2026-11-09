using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace POS.Avalonia.Converters;

/// <summary>
/// True when the bound value equals the parameter (case-insensitive).
/// Lets a choice button own its selected state: Classes.selected="{Binding Mode, Converter={StaticResource StringEqualsConverter}, ConverterParameter=Cash}".
/// </summary>
public sealed class StringEqualsConverter : IValueConverter
{
    public static readonly StringEqualsConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => string.Equals(value as string, parameter as string, StringComparison.OrdinalIgnoreCase);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
