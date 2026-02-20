using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;
using POS.Models;

namespace POS.Helpers.Converters
{
    public class ItemViewModelToBoolConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var res= value is ComboMenuItem ? Visibility.Visible : Visibility.Collapsed;
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
