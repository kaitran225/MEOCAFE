using Microsoft.UI.Xaml.Data;

namespace POS.Helpers.Converters
{
    public class ComboNameToBoolConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString() != "Combo";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }

}
