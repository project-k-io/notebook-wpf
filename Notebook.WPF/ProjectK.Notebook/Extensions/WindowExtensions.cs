using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace ProjectK.Notebook.Extensions
{
    public static class WindowExtensions
    {
        public static void LoadSettings(this Window window, NameValueCollection appSettings)
        {
            // window settings
            window.WindowState = appSettings.GetEnumValue("MainWindowState", WindowState.Normal);
            window.Top = appSettings.GetDouble("MainWindowTop", 100);
            window.Left = appSettings.GetDouble("MainWindowLeft", 100);
            window.Width = appSettings.GetDouble("MainWindowWidth", 800);
            window.Height = appSettings.GetDouble("MainWindowHeight", 400d);
        }

        public static void SaveSettings(this Window window, KeyValueConfigurationCollection settings)
        {
            // ISSUE: variable of a compiler-generated type
            // window settings
            if (window.WindowState != WindowState.Minimized)
            {
                settings.SetValue("MainWindowTop"   , window.Top.ToString(CultureInfo.InvariantCulture));
                settings.SetValue("MainWindowLeft"  , window.Left.ToString(CultureInfo.InvariantCulture));
                settings.SetValue("MainWindowWidth" , window.Width.ToString(CultureInfo.InvariantCulture));
                settings.SetValue("MainWindowHeight", window.Height.ToString(CultureInfo.InvariantCulture));
            }
        }
    }

}
