using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using ProjectK.Logging;
using ProjectK.Notebook.ViewModels;
using ProjectK.Utils;
using Microsoft.Windows.Themes;
namespace ProjectK.Notebook
{
    public partial class App : Application
    {
        private static ILogger _logger;
        private readonly MainViewModel _mainModel = new MainViewModel();
        private MainWindowOne _mainWindowOne;

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
            _mainModel.InitLogging();
            _logger = LogManager.GetLogger<App>();
            _mainModel.SetOutput();

            // MainWindowOne
            _mainWindowOne = new MainWindowOne {DataContext = _mainModel};

            // Show MainWindowOne
            _mainModel.LoadSettings(_mainWindowOne);
            _mainWindowOne.Show();

            // Load Data
            await _mainModel.OpenFileAsync();
            await _mainModel.UpdateTypeListAsync();
            await _mainModel.StartSavingAsync();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _mainModel.SaveSettings(_mainWindowOne);
            _mainModel.StopSaving();
            await _mainModel.SaveFileAsync(); // Exit
            _mainWindowOne.Close();
            Shutdown();
        }
    }
}