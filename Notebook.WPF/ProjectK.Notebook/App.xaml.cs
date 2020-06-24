using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Extensions;
using ProjectK.Notebook.ViewModels;

namespace ProjectK.Notebook
{
    public partial class App : Application
    {
        private static ILogger _logger;
        private readonly MainViewModel _mainModel = new MainViewModel();
        private MainWindow _mainWindow;

        public App()
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.LogError(e.ToString());
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _mainModel.Assembly = Assembly.GetExecutingAssembly();
            _mainModel.InitLogging();
            _logger = LogManager.GetLogger<App>();

            _mainModel.InitOutput();

            // MainWindow
            _mainWindow = new MainWindow {DataContext = _mainModel};

            // Show MainWindow
            _mainModel.LoadSettings(_mainWindow);
            _mainWindow.Show();

            // Load Data
            await _mainModel.OpenFileAsync();
            await _mainModel.UpdateTypeListAsync();
            await _mainModel.StartSavingAsync();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _mainModel.SaveSettings(_mainWindow);
            _mainModel.StopSaving();
            await _mainModel.SaveFileAsync(); // Exit
            _mainWindow.Close();
            Shutdown();
        }
    }
}