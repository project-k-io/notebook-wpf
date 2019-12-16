using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
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
        private static readonly ILogger Logger = LogManager.GetLogger<App>();
        private readonly MainViewModel _mainModel = new MainViewModel();
        private readonly MainWindow _mainWindow = new MainWindow();
        private bool _canSave;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AddLogging();
            await _mainModel.LoadDataAsync();
            await _mainModel.UpdateTypeListAsync();
            LoadSettings();
            _mainWindow.DataContext = _mainModel;
            _mainWindow.Show();
            await StartSavingAsync();
        }

        void AddLogging()
        {
            var serviceProvider = new ServiceCollection().
                AddLogging(cfg => cfg.AddConsole()).
                BuildServiceProvider();

            LogManager.Provider = serviceProvider;
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
                _mainWindow.WindowState = settings.MainWindowState;
                _mainWindow.Top = settings.MainWindowTop;
                _mainWindow.Left = settings.MainWindowLeft;
                _mainWindow.Width = settings.MainWindowWidth;
                _mainWindow.Height = settings.MainWindowHeight;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

            _mainModel.Layout.NavigatorWidth = settings.LayoutNavigatorWidth;
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
                }

                settings.MainWindowState = _mainWindow.WindowState;
                settings.Save();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
    }
}