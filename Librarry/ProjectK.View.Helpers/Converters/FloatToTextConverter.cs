using System;
using System.Globalization;
using System.Windows.Data;

namespace ProjectK.View.Helpers.Converters
{
    [ValueConversion(typeof(double), typeof(string))]
    public class FloatToTextConverter : ConverterMarkupExtension<FloatToTextConverter>, IValueConverter
    {
        public const double Thousand = 1000.0;
        public const double Million = 1000000.0;
        public const double Billion = 1000000000.0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var num = System.Convert.ToDouble(value, culture);
                var format = parameter as string;
                if (string.IsNullOrEmpty(format))
                    format = "N0";
                if (format == "MC")
                    return string.Format("{0:N2}", num / 1000000000.0);
                return num.ToString(format, culture);
            }
            catch
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = 0.0f;
            if (!float.TryParse((string) value, out result))
                return null;
            return result;
        }
    }
}