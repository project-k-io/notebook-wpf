using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Vibor.View.Helpers.Converters
{
    public class IndexConverter : ConverterMarkupExtension<IndexConverter>, IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, CultureInfo culture)
        {
            var listViewItem = (ListViewItem) value;
            var listView = ItemsControl.ItemsControlFromItemContainer(listViewItem) as ListView;
            if (listView == null)
                return -1;
            return listView.ItemContainerGenerator.IndexFromContainer(listViewItem).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}