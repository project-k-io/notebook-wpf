// Decompiled with JetBrains decompiler
// Type: Vibor.View.Helpers.Converters.BackgroundConvertor
// Assembly: Vibor.View.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 66A7410A-B003-4703-9434-CC2ABBB24103
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.View.Helpers.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Vibor.View.Helpers.Converters
{
  public sealed class BackgroundConvertor : ConverterMarkupExtension<BackgroundConvertor>, IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      ListViewItem listViewItem = (ListViewItem) value;
      if ((ItemsControl.ItemsControlFromItemContainer((DependencyObject) listViewItem) as ListView).ItemContainerGenerator.IndexFromContainer((DependencyObject) listViewItem) % 2 == 0)
        return (object) Brushes.LightBlue;
      return (object) Brushes.White;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}
