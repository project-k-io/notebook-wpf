using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.ViewModels;
using ProjectK.Utils.Extensions;
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
            _appModel.OpenDatabase();
        }


        protected override void OnExit(ExitEventArgs e)
        {
            _logger?.LogDebug("OnExit");
            _appModel.SaveSettings(_mainWindow);

            // Close Database
            _appModel.CloseDatabase();
            _mainWindow.Close();
            Shutdown();
            base.OnExit(e);
        }
    }
}