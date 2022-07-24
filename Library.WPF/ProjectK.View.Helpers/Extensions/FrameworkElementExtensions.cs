using System;
using System.Windows;
using System.Windows.Threading;

namespace ProjectK.Toolkit.Wpf.Helpers.Extensions;

public static class FrameworkElementExtensions
{
    public static Action<Action> GetAddDelegate(this FrameworkElement element,
        DispatcherPriority priority = DispatcherPriority.Background)
    {
        return a => element.Dispatcher.BeginInvoke(priority, a);
    }
}