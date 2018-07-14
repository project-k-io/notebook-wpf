using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ProjectApp.Properties
{
    [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    [CompilerGenerated]
    internal sealed class Settings : ApplicationSettingsBase
    {
        private static readonly Settings defaultInstance = (Settings) Synchronized(new Settings());

        public static Settings Default
        {
            get
            {
                var defaultInstance = Settings.defaultInstance;
                return defaultInstance;
            }
        }

        [DebuggerNonUserCode]
        [DefaultSettingValue("Normal")]
        [UserScopedSetting]
        public WindowState MainWindowState
        {
            get => (WindowState) this[nameof(MainWindowState)];
            set => this[nameof(MainWindowState)] = value;
        }

        private void SettingChangingEventHandler(object sender, SettingChangingEventArgs e)
        {
        }

        private void SettingsSavingEventHandler(object sender, CancelEventArgs e)
        {
        }
    }
}