using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Extensions;
using ProjectK.Notebook.ViewModels;
using ProjectK.Utils.Extensions;
using Syncfusion.Licensing;

namespace ProjectK.Notebook
{
    public partial class App : Application
    {
        private static ILogger _logger;

        public App()
        {
            SyncfusionLicenseProvider.RegisterLicense("MzEwNjY0QDMxMzgyZTMyMmUzMEdzZjVJbzlJbzdSTkNLWWJuNFlPRWlZOXBOWkZNc0N0cnVTRm9PcXVBNEE9");
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.LogError(e.ToString());
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _logger = LogManager.GetLogger<App>();

            var appSettings = ConfigurationManager.AppSettings;

            // MainWindow
            var window = new MainWindow();
            window.LoadSettings(appSettings);
            window.LoadDockLayout();

            // Show MainWindow
            var model = new AppViewModel();
            window.DataContext = model;
            model.LoadSettings(appSettings);
            window.Show();

            // Open Database
            // var key = "AlanDatabase";
            var key = "TestDatabase";
            var connectionString = ConfigurationManager.ConnectionStrings[key].ConnectionString;
            model.OpenDatabase(connectionString);

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            _logger?.LogDebug("OnExit");
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            if (App.Current.MainWindow is MainWindow window)
            {
                window.SaveSettings(settings);
                window.SaveDockLayout();
                window.Close();

                if (window.DataContext is AppViewModel appModel)
                {
                    appModel.SaveSettings(settings);
                    await appModel.CloseDatabaseAsync();
                }

            }
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

            Shutdown();
            base.OnExit(e);
        }
    }
}