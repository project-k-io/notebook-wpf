// Decompiled with JetBrains decompiler
// Type: Vibor.View.Helpers.Converters.DoubleToTextConverter
// Assembly: Vibor.View.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 66A7410A-B003-4703-9434-CC2ABBB24103
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.View.Helpers.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Data.Common;
using System.Windows.Data;
using Vibor.Helpers;
using Vibor.Logging;

namespace Vibor.View.Helpers.Converters
{
  [ValueConversion(typeof (double), typeof (string))]
  public class DoubleToTextConverter : ConverterMarkupExtension<DoubleToTextConverter>, IValueConverter
  {
    private static readonly ILog Log = LogManager.GetLogger();
    private readonly DoubleConverter _converter;

    public DoubleToTextConverter()
    {
      this._converter = new DoubleConverter();
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) this._converter.ConvertToString(value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!this._converter.IsValid(value))
        return (object) null;
      return this._converter.ConvertFromString((string) value);
    }
  }
}
