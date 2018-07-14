// Decompiled with JetBrains decompiler
// Type: Projects.Views.Helpers.LevelToIndentConverter
// Assembly: Projects.Views, Version=1.1.8.29122, Culture=neutral, PublicKeyToken=null
// MVID: BCB19E50-AB69-4CA9-9CF4-1A9C4DEAF8F2
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.Views.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Projects.Views.Helpers
{
  public class LevelToIndentConverter : IValueConverter
  {
    private const double c_IndentSize = 19.0;

    public object Convert(object o, Type type, object parameter, CultureInfo culture)
    {
      return (object) new Thickness((double) (int) o * 19.0, 0.0, 0.0, 0.0);
    }

    public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}
