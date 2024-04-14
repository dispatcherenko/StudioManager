using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StudioManager.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                bool result;
                bool.TryParse(stringValue, out result);
                return result;
            }

            return false;
        }
    }
}
