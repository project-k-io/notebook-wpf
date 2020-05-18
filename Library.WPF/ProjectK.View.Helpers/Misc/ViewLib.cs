using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Utils;

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