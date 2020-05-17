using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
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
    class MainViewModel : DomainViewModel
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
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                _logger.LogDebug("LoadSettings");

                T GetEnumValue<T>(string key, T defaultValue) where T : struct => Enum.TryParse(appSettings[key], out T value) ? value : defaultValue;
                double GetDoubleValue(string key, double defaultValue) => double.TryParse(appSettings[key], out var value) ? value : defaultValue;
                int GetIntValue(string key, int defaultValue) => int.TryParse(appSettings[key], out var value) ? value : defaultValue;
                Guid GetGuidValue(string key, Guid defaultValue) => Guid.TryParse(appSettings[key], out var value) ? value : defaultValue;
                string GetStringValue(string key, string defaultValue) =>appSettings[key] ?? defaultValue;

                // window settings
                window.WindowState = GetEnumValue<WindowState>("MainWindowState", WindowState.Normal);

                window.Top = GetDoubleValue("MainWindowTop", 100);
                window.Left = GetDoubleValue("MainWindowLeft", 100);
                window.Width = GetDoubleValue("MainWindowWidth", 800);
                window.Height = GetDoubleValue("MainWindowHeight", 400d);

                // model settings
                ;
                Layout.NavigatorWidth = GetIntValue("LayoutNavigatorWidth", 200);
                LastListTaskId = GetGuidValue("LastListTaskId", Guid.Empty);
                LastTreeTaskId = GetGuidValue("LastTreeTaskId", Guid.Empty);
                DataFile = GetStringValue("RecentFile", "New Data");

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
            _logger.LogDebug("SaveSettings()");
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;

                void SetValue(string key, string value)
                {
                    if (settings[key] == null)
                    {
                        settings.Add(key, value);
                    }
                    else
                    {
                        settings[key].Value = value;
                    }
                }

                PrepareSettings();

                // ISSUE: variable of a compiler-generated type
                if (window.WindowState != WindowState.Minimized)
                {
                    SetValue("MainWindowTop", window.Top.ToString(CultureInfo.InvariantCulture));
                    SetValue("MainWindowLeft", window.Left.ToString(CultureInfo.InvariantCulture));
                    SetValue("MainWindowWidth", window.Width.ToString(CultureInfo.InvariantCulture));
                    SetValue("MainWindowHeight", window.Height.ToString(CultureInfo.InvariantCulture));
                    SetValue("LayoutNavigatorWidth", Layout.NavigatorWidth.ToString(CultureInfo.InvariantCulture));
                }

                SetValue("RecentFile", DataFile);
                SetValue("LastListTaskId", LastListTaskId.ToString());
                SetValue("LastTreeTaskId",  LastTreeTaskId.ToString());
                SetValue("MainWindowState", window.WindowState.ToString());

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
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
