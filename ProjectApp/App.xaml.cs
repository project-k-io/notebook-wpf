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
        private bool _canSave;
        private readonly MainViewModel _mainModel = new MainViewModel();
        private MainWindow _mainWindow;

        public App()
        {
            this.DispatcherUnhandledException += OnDispatcherUnhandledException; 
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.LogError(e.ToString());
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AddLogging();

            // MainModel
            var registryPath  = XApp.AppName + "\\Output";
            var subKey = Registry.CurrentUser.CreateSubKey(registryPath);
            _mainModel.Output.SetValue = (key, value) => subKey.SetValue(key, value);
            _mainModel.Output.GetValue = (key, value) => subKey.GetValue(key, value);
            _mainModel.Output.UpdateFilter = () =>  CollectionViewSource.GetDefaultView(_mainModel.Output.Records).Filter = o => _mainModel.Output.Filter(o);
            _mainModel.Output.ReadSettings();

            // MainWindow
            _mainWindow = new MainWindow();
            _mainWindow.DataContext = _mainModel;

            // Show MainWindow
            LoadSettings();
            _mainWindow.Show();
            // Load Data
            await _mainModel.LoadDataAsync();
            await _mainModel.UpdateTypeListAsync();
            await StartSavingAsync();
        }


        private void AddLogging()
        {
            try
            {
                var serviceProvider = new ServiceCollection()
                    .AddLogging(logging => logging.AddConsole())
                    .AddLogging(logging => logging.AddDebug())
                    .AddLogging(logging => logging.AddProvider(new OutputLoggerProvider(_mainModel.Output.LogEvent)))
                    .Configure<LoggerFilterOptions>(o => o.MinLevel = LogLevel.Debug)
                    .BuildServiceProvider();

                LogManager.Provider = serviceProvider;
                _logger = LogManager.GetLogger<App>();
                _logger.LogDebug("Logger Installed");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        protected override async void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            SaveSettings();
            StopSaving();
            await _mainModel.SaveDataAsync();
            _mainWindow.Close();
            Shutdown();
        }

        public async Task StartSavingAsync()
        {
            _canSave = true;
            while (_canSave)
            {
                await _mainModel.SaveDataAsync();
                await Task.Run(() => Thread.Sleep(5000));
            }
        }

        public void StopSaving()
        {
            _canSave = false;
        }

        private void LoadSettings()
        {
            // ISSUE: variable of a compiler-generated type
            var settings = Settings.Default;
            try
            {
                _logger.LogDebug("LoadSettings");

                // window settings
                _mainWindow.WindowState = settings.MainWindowState;
                _mainWindow.Top = settings.MainWindowTop;
                _mainWindow.Left = settings.MainWindowLeft;
                _mainWindow.Width = settings.MainWindowWidth;
                _mainWindow.Height = settings.MainWindowHeight;
                // model settings
                _mainModel.Layout.NavigatorWidth = settings.LayoutNavigatorWidth;
                _mainModel.LastListTaskId = settings.LastListTaskId;
                _mainModel.LastTreeTaskId = settings.LastTreeTaskId;
                _mainModel.DataFile = settings.RecentFile;
                _mainModel.MostRecentFiles.Clear();

                if(File.Exists(_mainModel.DataFile))
                    _mainModel.MostRecentFiles.Add(new FileInfo(_mainModel.DataFile));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        private void SaveSettings()
        {
            try
            {
                _logger.LogDebug("SaveSettings()");

                _mainModel.PrepareSettings();
                var settings = Settings.Default;

                // ISSUE: variable of a compiler-generated type
                if (_mainWindow.WindowState != WindowState.Minimized)
                {
                    settings.MainWindowTop = _mainWindow.Top;
                    settings.MainWindowLeft = _mainWindow.Left;
                    settings.MainWindowWidth = _mainWindow.Width;
                    settings.MainWindowHeight = _mainWindow.Height;
                    settings.LayoutNavigatorWidth = _mainModel.Layout.NavigatorWidth;
                    settings.RecentFile = _mainModel.DataFile;
                    settings.LastListTaskId = _mainModel.LastListTaskId;
                    settings.LastTreeTaskId = _mainModel.LastTreeTaskId;
                }

                settings.MainWindowState = _mainWindow.WindowState;
                settings.Save();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }
    }
}