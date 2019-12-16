using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Projects.Models.Versions.Version2;
using Projects.ViewModels;
using Microsoft.Extensions.Logging;
using Vibor.Helpers;

namespace ProjectApp
{
    public partial class App : Application
    {
        private static ILogger _logger;
        private MainViewModel _mainModel;
        private MainWindow _mainWindow;
        private bool _canSave;

        public App()
        {
            // AddLogging();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AddLogging();
            // MainModel
            _mainModel = new MainViewModel();
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

        void AddLogging()
        {
            try
            {
                var serviceProvider = new ServiceCollection()
                    .AddLogging(cfg => cfg.AddConsole())
                    .AddLogging(cfg => cfg.AddDebug())
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
                SaveSettings();
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
                _mainModel.Folder = settings.RecentFolder;
                _mainModel.RecentFile = settings.RecentFile;
                _mainModel.MostRecentFiles.Clear();

                if (Directory.Exists(_mainModel.Folder))
                    _mainModel.MostRecentFiles.Add(new FileInfo(_mainModel.Folder));
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
                    settings.RecentFolder = _mainModel.Folder;
                    settings.RecentFile = _mainModel.RecentFile;
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