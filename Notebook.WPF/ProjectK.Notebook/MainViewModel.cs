using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using ProjectK.Logging;
using ProjectK.Notebook.Extensions;
using ProjectK.Notebook.ViewModels;
using ProjectK.Utils;
using ProjectK.Utils.Extensions;

namespace ProjectK.Notebook
{
    internal class MainViewModel : DomainViewModel
    {
        private bool _canSave;
        private ILogger _logger;
        private Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public string Title => Assembly.GetAssemblyTitle() + " " + Assembly.GetAssemblyVersion() + " - " + DataFile;

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

            await OpenFileAsync(r.fileName); // User clicked open file
        }

        private async Task UserSaveFileAsync()
        {
            if (File.Exists(DataFile))
                await SaveFileAsync(); // User Save
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
            if (!string.IsNullOrEmpty(directoryName) && Directory.Exists(directoryName)) dialog.InitialDirectory = directoryName;

            var fileName = Path.GetFileNameWithoutExtension(path);
            if (!string.IsNullOrEmpty(fileName)) dialog.FileName = fileName;

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

        public void InitOutput()
        {
            // MainModel
            Output.UpdateFilter = () => CollectionViewSource.GetDefaultView(Output.Records).Filter = o => Output.Filter(o);
        }

        public void LoadSettings(Window window)
        {
            // ISSUE: variable of a compiler-generated type
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                _logger.LogDebug("LoadSettings");
                // window settings
                window.WindowState = appSettings.GetEnumValue("MainWindowState", WindowState.Normal);
                window.Top = appSettings.GetDouble("MainWindowTop", 100);
                window.Left = appSettings.GetDouble("MainWindowLeft", 100);
                window.Width = appSettings.GetDouble("MainWindowWidth", 800);
                window.Height = appSettings.GetDouble("MainWindowHeight", 400d);
                // model settings
                Layout.NavigatorWidth = appSettings.GetInt("LayoutNavigatorWidth", 200);
                LastListTaskId = appSettings.GetGuid("LastListTaskId", Guid.Empty);
                LastTreeTaskId = appSettings.GetGuid("LastTreeTaskId", Guid.Empty);
                DataFile = appSettings.GetString("RecentFile", "New Data");
                // Output
                Output.OutputButtonErrors.IsChecked = appSettings.GetBool("OutputError", false);
                Output.OutputButtonDebug.IsChecked = appSettings.GetBool("OutputDebug", false);
                Output.OutputButtonMessages.IsChecked = appSettings.GetBool("OutputInfo", false);
                Output.OutputButtonWarnings.IsChecked = appSettings.GetBool("OutputWarning", false);

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


                PrepareSettings();

                // ISSUE: variable of a compiler-generated type
                if (window.WindowState != WindowState.Minimized)
                {
                    settings.SetValue("MainWindowTop", window.Top.ToString(CultureInfo.InvariantCulture));
                    settings.SetValue("MainWindowLeft", window.Left.ToString(CultureInfo.InvariantCulture));
                    settings.SetValue("MainWindowWidth", window.Width.ToString(CultureInfo.InvariantCulture));
                    settings.SetValue("MainWindowHeight", window.Height.ToString(CultureInfo.InvariantCulture));
                    settings.SetValue("LayoutNavigatorWidth", Layout.NavigatorWidth.ToString(CultureInfo.InvariantCulture));
                }

                settings.SetValue("RecentFile", DataFile);
                settings.SetValue("LastListTaskId", LastListTaskId.ToString());
                settings.SetValue("LastTreeTaskId", LastTreeTaskId.ToString());
                settings.SetValue("MainWindowState", window.WindowState.ToString());

                settings.SetValue("OutputError", Output.OutputButtonErrors.IsChecked.ToString());
                settings.SetValue("OutputDebug", Output.OutputButtonDebug.IsChecked.ToString());
                settings.SetValue("OutputInfo", Output.OutputButtonMessages.IsChecked.ToString());
                settings.SetValue("OutputWarning", Output.OutputButtonWarnings.IsChecked.ToString());

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