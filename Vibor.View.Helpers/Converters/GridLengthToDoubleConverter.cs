// Decompiled with JetBrains decompiler
// Type: Vibor.View.Helpers.Converters.GridLengthToDoubleConverter
// Assembly: Vibor.View.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 66A7410A-B003-4703-9434-CC2ABBB24103
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.View.Helpers.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Vibor.View.Helpers.Converters
{
  public class GridLengthToDoubleConverter : ConverterMarkupExtension<GridLengthToDoubleConverter>, IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) new GridLength((double) value, GridUnitType.Pixel);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) ((GridLength) value).Value;
    }
  }
}
