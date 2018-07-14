﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Vibor.View.Helpers.Converters
{
    public sealed class BackgroundConvertor : ConverterMarkupExtension<BackgroundConvertor>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var listViewItem = (ListViewItem) value;
            if ((ItemsControl.ItemsControlFromItemContainer(listViewItem) as ListView).ItemContainerGenerator
                .IndexFromContainer(listViewItem) % 2 == 0)
                return Brushes.LightBlue;
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}