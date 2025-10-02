using Microsoft.Maui.Controls;
using System;
using System.Globalization;

namespace SportEventsApp.Converters
{
    public class BellImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool notify = value is bool b && b;
            return notify ? "bell_filled.png" : "bell.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
