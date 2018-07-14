// Decompiled with JetBrains decompiler
// Type: Vibor.View.Helpers.Converters.Int32ToTextConverter
// Assembly: Vibor.View.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 66A7410A-B003-4703-9434-CC2ABBB24103
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.View.Helpers.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace Vibor.View.Helpers.Converters
{
  public class Int32ToTextConverter : ConverterMarkupExtension<Int32ToTextConverter>, IValueConverter
  {
    private readonly Int32Converter _converter;

    public Int32ToTextConverter()
    {
      this._converter = new Int32Converter();
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) this._converter.ConvertToString(value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return this._converter.ConvertFromString((string) value);
    }
  }
}
