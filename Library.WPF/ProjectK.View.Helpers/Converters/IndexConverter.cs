using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace ProjectK.Toolkit.Wpf.Helpers.Converters;

public class IndexConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var index = 0;
        if (value is ListViewItem item)
            if (ItemsControl.ItemsControlFromItemContainer(item) is ListView listView)
                index = listView.ItemContainerGenerator.IndexFromContainer(item);
        return index.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}