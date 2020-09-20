using System;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Extensions;
using ProjectK.Notebook.ViewModels;
using Syncfusion.Licensing;

namespace ProjectK.Notebook
{
    public partial class App : Application
    {
        private static ILogger _logger;
        private AppViewModel _appModel;
        private MainWindow _mainWindow;

        public App()
        {
            SyncfusionLicenseProvider.RegisterLicense("MzEwNjY0QDMxMzgyZTMyMmUzMEdzZjVJbzlJbzdSTkNLWWJuNFlPRWlZOXBOWkZNc0N0cnVTRm9PcXVBNEE9");
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.LogError(e.ToString());
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _appModel = new AppViewModel();
            _logger = LogManager.GetLogger<App>();

            // MainWindow
            _mainWindow = new MainWindow {DataContext = _appModel};

            // Show MainWindow
            _appModel.LoadSettings(_mainWindow);
            _mainWindow.Show();

            // Open Database
#if !AK // db open
            _appModel.OpenDatabase();

            // ModelToViewModel Data
            await _appModel.UpdateTypeListAsync();
#endif
        }


        protected override void OnExit(ExitEventArgs e)
        {
            _logger?.LogDebug("OnExit");
            _appModel.SaveSettings(_mainWindow);

            // Close Database
#if AK    // db close
            _appModel.CloseDatabase();
#endif
            _mainWindow.Close();
            Shutdown();
            base.OnExit(e);
        }
    }
}