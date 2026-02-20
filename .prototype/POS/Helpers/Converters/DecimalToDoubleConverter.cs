using System;
using System.Globalization;
using Microsoft.UI.Xaml.Data;
namespace POS.Helpers.Converters
{

    public class DecimalToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is decimal decimalValue)
            {
                return (double)decimalValue;
            }
            return 0.0; // Default value for non-decimal input
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is double doubleValue)
            {
                return (decimal)doubleValue;
            }
            return 0m; // Default value for non-double input
        }
    }

}
