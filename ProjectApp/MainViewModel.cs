using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using ProjectK.Logging;
using ProjectK.Notebook.ViewModels;
using ProjectK.Utils;

namespace ProjectK.Notebook
{
    class MainViewModel: DomainViewModel
    {
        private ILogger _logger;
        private bool _canSave;

        public CommandBindingCollection CreateCommandBindings()
        {
            var commandBindings = new CommandBindingCollection
            {
                new CommandBinding(ApplicationCommands.New, async (s, e) => await UserNewFileAsync(), (s, e) => e.CanExecute = true),
                new CommandBinding(ApplicationCommands.Open, async (s, e) => await UserOpenFileAsync(), (s, e) => e.CanExecute = true),
                new CommandBinding(ApplicationCommands.Save, async (s, e) => await UserSaveFileAsync(), (s, e) => e.CanExecute = true),
                new CommandBinding(ApplicationCommands.SaveAs, async (s, e) => await UserSaveFileAsAsync(), (s, e) => e.CanExecute = true),
                new CommandBinding(ApplicationCommands.Close, async (s, e) => await UserNewFileAsync(), (s, e) => e.CanExecute = true)
            };
            return commandBindings;
        }

        private void UI_FileOpenOldFormat()
        {
            _logger.LogDebug("UserOpenFileAsync()");
            var dialog = new OpenFileDialog();
            var r = SetFileDialog(dialog, DataFile);
            if (!r.ok)
                return;

            DataFile = r.fileName;
            FileOpenOldFormat();
        }


        private async Task UserOpenFileAsync()
        {
            _logger.LogDebug("UserOpenFileAsync()");
            var dialog = new OpenFileDialog();
            var r = SetFileDialog(dialog, DataFile);
            if (!r.ok)
                return;

            await OpenFileAsync(r.fileName);  // User clicked open file
        }

        private async Task UserSaveFileAsync()
        {
            if (File.Exists(DataFile))
                await SaveFileAsync();  // User Save
            else
                await UserSaveFileAsAsync();
        }

        private async Task UserSaveFileAsAsync()
        {
            var dialog = new SaveFileDialog();
            var r = SetFileDialog(dialog, DataFile);
            if (!r.ok)
                return;

            DataFile = r.fileName;
            await SaveFileAsync(); // Save As
        }

        public async Task OpenFileAsync()
        {
            _logger.LogDebug("OpenFileAsync");
            var path = DataFile;
            if (!File.Exists(path))
                await UserOpenFileAsync();
            else
                await OpenFileAsync(path);
        }


        public (string fileName, bool ok) SetFileDialog(FileDialog dialog, string path)
        {
            var directoryName = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directoryName) && Directory.Exists(directoryName))
            {
                dialog.InitialDirectory = directoryName;
            }

            var fileName = Path.GetFileNameWithoutExtension(path);
            if (!string.IsNullOrEmpty(fileName))
            {
                dialog.FileName = fileName;
            }

            dialog.DefaultExt = ".json";
            dialog.Filter = "Json documents (.json)|*.json" +
                            "|XML documents(.xml) | *.xml";

            var result = dialog.ShowDialog();
            if (result != true)
                return ("", false);

            return (dialog.FileName, true);
        }

        public void InitLogging()
        {
            try
            {
                var serviceProvider = new ServiceCollection()
                    .AddLogging(logging => logging.AddConsole())
                    .AddLogging(logging => logging.AddDebug())
                    .AddLogging(logging => logging.AddProvider(new OutputLoggerProvider(Output.LogEvent)))
                    .Configure<LoggerFilterOptions>(o => o.MinLevel = LogLevel.Debug)
                    .BuildServiceProvider();

                LogManager.Provider = serviceProvider;
                _logger = LogManager.GetLogger<MainViewModel>();
                _logger.LogDebug("Logger Installed");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public void SetOutput()
        {
            // MainModel
            var registryPath = XApp.AppName + "\\Output";
            var subKey = Registry.CurrentUser.CreateSubKey(registryPath);
            Output.SetValue = (key, value) => subKey.SetValue(key, value);
            Output.GetValue = (key, value) => subKey.GetValue(key, value);
            Output.UpdateFilter = () => CollectionViewSource.GetDefaultView(Output.Records).Filter = o => Output.Filter(o);
            Output.ReadSettings();
        }

        public void LoadSettings(Window window)
        {
            // ISSUE: variable of a compiler-generated type
            var settings = Settings.Default;
            try
            {
                _logger.LogDebug("LoadSettings");

                // window settings
                window.WindowState = settings.MainWindowState;
                window.Top = settings.MainWindowTop;
                window.Left = settings.MainWindowLeft;
                window.Width = settings.MainWindowWidth;
                window.Height = settings.MainWindowHeight;

                // model settings
                Layout.NavigatorWidth = settings.LayoutNavigatorWidth;
                LastListTaskId = settings.LastListTaskId;
                LastTreeTaskId = settings.LastTreeTaskId;
                DataFile = settings.RecentFile;
                MostRecentFiles.Clear();

                if (File.Exists(DataFile))
                    MostRecentFiles.Add(new FileInfo(DataFile));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        public void SaveSettings(Window window)
        {
            try
            {
                _logger.LogDebug("SaveSettings()");

                PrepareSettings();
                var settings = Settings.Default;

                // ISSUE: variable of a compiler-generated type
                if (window.WindowState != WindowState.Minimized)
                {
                    settings.MainWindowTop = window.Top;
                    settings.MainWindowLeft = window.Left;
                    settings.MainWindowWidth = window.Width;
                    settings.MainWindowHeight = window.Height;
                    settings.LayoutNavigatorWidth = Layout.NavigatorWidth;
                    settings.RecentFile = DataFile;
                    settings.LastListTaskId = LastListTaskId;
                    settings.LastTreeTaskId = LastTreeTaskId;
                }

                settings.MainWindowState = window.WindowState;
                settings.Save();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        public async Task StartSavingAsync()
        {
            _canSave = true;
            while (_canSave)
            {
                await SaveModifiedFileAsync();
                await Task.Run(() => Thread.Sleep(5000));
            }
        }


        public void StopSaving()
        {
            _canSave = false;
        }

    }
}
