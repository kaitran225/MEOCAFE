using Microsoft.UI.Xaml.Data;

namespace POS.Helpers.Converters
{
    public class PriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is decimal price)
            {
                return $"{price:N0} VNĐ";  // Format the price with comma separation and "VNĐ"
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}