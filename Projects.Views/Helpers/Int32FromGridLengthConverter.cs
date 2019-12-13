using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Projects.Views.Helpers
{
    public class Int32FromGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new GridLength((int) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int) ((GridLength) value).Value;
        }
    }
}