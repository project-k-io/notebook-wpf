﻿// Decompiled with JetBrains decompiler
// Type: Vibor.View.Helpers.Converters.TextToDoubleConverter
// Assembly: Vibor.View.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 66A7410A-B003-4703-9434-CC2ABBB24103
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.View.Helpers.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using Vibor.Helpers;

namespace Vibor.View.Helpers.Converters
{
  [ValueConversion(typeof (string), typeof (double))]
  public class TextToDoubleConverter : ConverterMarkupExtension<TextToDoubleConverter>, IValueConverter
  {
    private static readonly ILog Log = XLogger.GetLogger();
    private readonly DoubleConverter _converter;

    public TextToDoubleConverter()
    {
      this._converter = new DoubleConverter();
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      double result = 0.0;
      if (!double.TryParse((string) value, out result))
        return (object) result;
      return (object) result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) this._converter.ConvertToString(value);
    }
  }
}
