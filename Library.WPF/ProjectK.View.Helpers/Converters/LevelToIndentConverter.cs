using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ProjectK.Toolkit.Wpf.Helpers.Converters;

public class LevelToIndentConverter : IValueConverter
{
    private const double IndentSize = 12.0;

    public object Convert(object o, Type type, object parameter, CultureInfo culture)
    {
        var level = 1;
        if (o is int i)
            level = i;

        var left = level * IndentSize;
        return new Thickness((int)left, 0.0, 0.0, 0.0);
    }

    public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}