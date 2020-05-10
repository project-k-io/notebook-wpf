using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ProjectK.View.Helpers.Converters
{
    public class GridLengthToDoubleConverter : ConverterMarkupExtension<GridLengthToDoubleConverter>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new GridLength((double) value, GridUnitType.Pixel);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((GridLength) value).Value;
        }
    }
}