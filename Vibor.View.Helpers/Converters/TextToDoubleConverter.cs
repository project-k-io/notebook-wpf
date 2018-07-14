﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using Vibor.Logging;

namespace Vibor.View.Helpers.Converters
{
    [ValueConversion(typeof(string), typeof(double))]
    public class TextToDoubleConverter : ConverterMarkupExtension<TextToDoubleConverter>, IValueConverter
    {
        private static readonly ILog Log = LogManager.GetLogger();
        private readonly DoubleConverter _converter;

        public TextToDoubleConverter()
        {
            _converter = new DoubleConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = 0.0;
            if (!double.TryParse((string) value, out result))
                return result;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _converter.ConvertToString(value);
        }
    }
}