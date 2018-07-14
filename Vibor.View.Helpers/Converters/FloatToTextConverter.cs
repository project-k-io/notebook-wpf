// Decompiled with JetBrains decompiler
// Type: Vibor.View.Helpers.Converters.FloatToTextConverter
// Assembly: Vibor.View.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 66A7410A-B003-4703-9434-CC2ABBB24103
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.View.Helpers.dll

using System;
using System.Globalization;
using System.Windows.Data;

namespace Vibor.View.Helpers.Converters
{
  [ValueConversion(typeof (double), typeof (string))]
  public class FloatToTextConverter : ConverterMarkupExtension<FloatToTextConverter>, IValueConverter
  {
    public const double Thousand = 1000.0;
    public const double Million = 1000000.0;
    public const double Billion = 1000000000.0;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        double num = Convert.ToDouble(value, (IFormatProvider) culture);
        string format = parameter as string;
        if (string.IsNullOrEmpty(format))
          format = "N0";
        if (format == "MC")
          return (object) string.Format("{0:N2}", (object) (num / 1000000000.0));
        return (object) num.ToString(format, (IFormatProvider) culture);
      }
      catch
      {
        return value;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      float result = 0.0f;
      if (!float.TryParse((string) value, out result))
        return (object) null;
      return (object) result;
    }
  }
}
