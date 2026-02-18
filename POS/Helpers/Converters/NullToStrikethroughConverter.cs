using Microsoft.UI.Xaml.Data;
using Windows.UI.Text;

namespace POS.Helpers.Converters
{
    public class NullToStrikethroughConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value == null ? TextDecorations.None : TextDecorations.Strikethrough;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }

}
