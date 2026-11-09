using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

namespace POS.Avalonia.Converters;

/// <summary>MultiValue: values[0]=id (int), values[1]=list (IEnumerable). Returns true if list contains id.</summary>
public class IdInListConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values == null || values.Count < 2) return false;
        var id = values[0] is int i ? i : (values[0] is long l ? (int)l : 0);
        if (values[1] is IEnumerable list)
            return list.Cast<object>().Any(x => x is int k && k == id);
        return false;
    }
}
