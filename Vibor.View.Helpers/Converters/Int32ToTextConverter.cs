using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace ProjectK.View.Helpers.Converters
{
    public class Int32ToTextConverter : ConverterMarkupExtension<Int32ToTextConverter>, IValueConverter
    {
        private readonly Int32Converter _converter;

        public Int32ToTextConverter()
        {
            _converter = new Int32Converter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _converter.ConvertToString(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _converter.ConvertFromString((string) value);
        }
    }
}