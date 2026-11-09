using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace POS.Avalonia.Converters;

/// <summary>MultiValue: values[0]=Quantity, values[1]=ReorderLevel. Returns max for progress bar (at least 1).</summary>
public class StockBarMaximumConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        int qty = values != null && values.Count > 0 && values[0] is int q ? q : 0;
        int reorder = values != null && values.Count > 1 && values[1] is int r ? r : 0;
        return (double)Math.Max(1, Math.Max(reorder * 2, qty));
    }
}
