// Decompiled with JetBrains decompiler
// Type: Projects.Views.Helpers.Int32FromGridLengthConverter
// Assembly: Projects.Views, Version=1.1.8.29122, Culture=neutral, PublicKeyToken=null
// MVID: BCB19E50-AB69-4CA9-9CF4-1A9C4DEAF8F2
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.Views.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Projects.Views.Helpers
{
    public class Int32FromGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new GridLength((int) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int) ((GridLength) value).Value;
        }
    }
}