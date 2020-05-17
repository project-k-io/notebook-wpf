using System;
using System.Globalization;
using System.Windows.Data;

namespace ProjectK.View.Helpers.Converters
{
    public class DebuggingConverter : ConverterMarkupExtension<DebuggingConverter>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This method should never be called");
        }
    }
}