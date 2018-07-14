using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using Vibor.Logging;

namespace Vibor.View.Helpers.Converters
{
    [ValueConversion(typeof(double), typeof(string))]
    public class DoubleToTextConverter : ConverterMarkupExtension<DoubleToTextConverter>, IValueConverter
    {
        private static readonly ILog Log = LogManager.GetLogger();
        private readonly DoubleConverter _converter;

        public DoubleToTextConverter()
        {
            _converter = new DoubleConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _converter.ConvertToString(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!_converter.IsValid(value))
                return null;
            return _converter.ConvertFromString((string) value);
        }
    }
}