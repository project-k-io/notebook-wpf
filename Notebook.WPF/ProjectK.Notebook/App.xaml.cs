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
            _mainWindow.Closing += async (s1,e1) => await MainWindowOnClosing(s1, e1);

            // Show MainWindow
            await _appModel.LoadRecentFiles();
            _appModel.LoadSettings(_mainWindow);
            _mainWindow.Show();

            // Load Data
            await _appModel.OpenRecentFilesAsync();
            await _appModel.UpdateTypeListAsync();
            await _appModel.StartSavingAsync();
        }


        private async Task MainWindowOnClosing(object sender, CancelEventArgs e)
        {
            _logger.LogDebug("MainWindowOnClosing");
            await _appModel.SaveRecentFiles();
            _appModel.SaveSettings(_mainWindow);
            _appModel.StopSaving();
            await _appModel.SaveFileAsync(); // Exit
        }



        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _mainWindow.Close();
            Shutdown();
        }
    }
}