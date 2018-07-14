// Decompiled with JetBrains decompiler
// Type: Vibor.View.Helpers.Converters.IndexConverter
// Assembly: Vibor.View.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 66A7410A-B003-4703-9434-CC2ABBB24103
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.View.Helpers.dll

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