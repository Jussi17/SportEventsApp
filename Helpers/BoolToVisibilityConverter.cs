using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace SportEventsApp.Helpers
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b; // true = näkyvä, false = piilossa
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b;
            return false;
        }
    }
}
