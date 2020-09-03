using System.ComponentModel;
using System.Reflection;
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
        private AppViewModel _mainModel = null;
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
            _mainModel = new AppViewModel();
            _mainModel.Assembly = Assembly.GetExecutingAssembly();
            _mainModel.LogEvent = _mainModel.Output.LogEvent;
            _mainModel.InitLogging(_mainModel.LogEvent);

            _logger = LogManager.GetLogger<App>();

            _mainModel.InitOutput();

            // MainWindow
            _mainWindow = new MainWindow {DataContext = _mainModel};
            _mainWindow.Closing += async (s1,e1) => await MainWindowOnClosing(s1, e1);



            // Show MainWindow
            await _mainModel.LoadRecentFiles();
            _mainModel.LoadSettings(_mainWindow);
            _mainWindow.Show();

            // Load Data
            await _mainModel.OpenRecentFilesAsync();
            await _mainModel.UpdateTypeListAsync();
            await _mainModel.StartSavingAsync();
        }

        private async Task MainWindowOnClosing(object sender, CancelEventArgs e)
        {
            await _mainModel.SaveRecentFiles();
            _mainModel.SaveSettings(_mainWindow);
            _mainModel.StopSaving();
            await _mainModel.SaveFileAsync(); // Exit
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _mainWindow.Close();
            Shutdown();
        }
    }
}