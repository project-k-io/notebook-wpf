using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ProjectApp.Properties;
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
            LoadSettings(_mainModel.Config.Layout);
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
            SaveSettings(_mainModel.Config.Layout);
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
                SaveSettings(_mainModel.Config.Layout);
                await _mainModel.SaveDataAsync();
                await Task.Run(() => Thread.Sleep(5000));
            }
        }

        public void StopSaving()
        {
            _canSave = false;
        }

        private void LoadSettings(ConfigModel.LayoutModel s1)
        {
            // ISSUE: variable of a compiler-generated type
            var settings = Settings.Default;
            try
            {
                _mainWindow.WindowState = settings.MainWindowState;
                _mainWindow.Top = s1.MainWindowTop;
                _mainWindow.Left = s1.MainWindowLeft;
                _mainWindow.Width = s1.MainWindowWidth;
                _mainWindow.Height = s1.MainWindowHeight;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

            _mainModel.Layout.NavigatorWidth = s1.LayoutNavigatorWidth;
        }

        private void SaveSettings(ConfigModel.LayoutModel s1)
        {
            try
            {
                _mainModel.PrepareSettings();
                // ISSUE: variable of a compiler-generated type
                var settings = Settings.Default;
                if (_mainWindow.WindowState != WindowState.Minimized)
                {
                    s1.MainWindowTop = _mainWindow.Top;
                    s1.MainWindowLeft = _mainWindow.Left;
                    s1.MainWindowWidth = _mainWindow.Width;
                    s1.MainWindowHeight = _mainWindow.Height;
                    s1.LayoutNavigatorWidth = _mainModel.Layout.NavigatorWidth;
                }

                settings.MainWindowState = _mainWindow.WindowState;
                settings.Save();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        [DebuggerNonUserCode]
        [STAThread]
        public static void Main()
        {
            new App().Run();
        }
    }
}