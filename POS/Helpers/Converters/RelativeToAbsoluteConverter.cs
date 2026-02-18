using Microsoft.UI.Xaml.Data;

namespace POS.Helpers.Converters
{
    public class RelativeToAbsoluteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string imageName)
            {
                if (imageName.StartsWith("MockImage/"))
                {
                    return $"ms-appx:///Assets/{imageName}.png";
                }

                return imageName;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
