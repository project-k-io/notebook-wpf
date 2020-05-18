using System;
using System.Windows;
using System.Windows.Threading;

namespace ProjectK.View.Helpers.Misc
{
    public class ViewLib
    {
        public static Action<Action> GetAddDelegate(FrameworkElement lv, DispatcherPriority priority = DispatcherPriority.Background)
        {
            return a => lv.Dispatcher.BeginInvoke(priority, a);
        }
    }
}