using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using ProjectK.Logging;
using ProjectK.Notebook.ViewModels;
using ProjectK.Notebook.ViewModels.Extensions;

namespace ProjectK.Notebook.Extensions
{
    public static class MainViewModelExtensions
    {
        private static ILogger _logger;


        public static CommandBindingCollection CreateCommandBindings(this MainViewModel model)
        {
            var commandBindings = new CommandBindingCollection
            {
                new CommandBinding(ApplicationCommands.New, async (s, e) => await model.UserNewFileAsync(), (s, e) => e.CanExecute = true),
                new CommandBinding(ApplicationCommands.Open, async (s, e) =>  await model.UserOpenFileAsync(), (s, e) => e.CanExecute = true),
                new CommandBinding(ApplicationCommands.Save, async (s, e) => await model.UserSaveFileAsync(), (s, e) => e.CanExecute = true),
                new CommandBinding(ApplicationCommands.SaveAs, async (s, e) => await model.UserSaveFileAsAsync(), (s, e) => e.CanExecute = true),
                new CommandBinding(ApplicationCommands.Close, async (s, e) => await model.UserNewFileAsync(), (s, e) => e.CanExecute = true)
            };
            return commandBindings;
        }

        private static void UI_FileOpenOldFormat(this MainViewModel model)
        {
            _logger.LogDebug("UserOpenFileAsync()");
            var dialog = new OpenFileDialog();
            var r = dialog.SetFileDialog(model.DataFile);
            if (!r.ok)
                return;

            model.DataFile = r.fileName;
            model.FileOpenOldFormat();
        }


        private static async Task UserOpenFileAsync(this MainViewModel model)
        {
            _logger.LogDebug("UserOpenFileAsync()");
            var dialog = new OpenFileDialog();
            var r = dialog.SetFileDialog(model.DataFile);
            if (!r.ok)
                return;

            await model.OpenFileAsync(r.fileName); // User clicked open file
        }

        private static async Task UserSaveFileAsync(this MainViewModel model)
        {
            if (File.Exists(model.DataFile))
                await model.SaveFileAsync(); // User Save
            else
                await model.UserSaveFileAsAsync();
        }

        private static async Task UserSaveFileAsAsync(this MainViewModel model)
        {
            var dialog = new SaveFileDialog();
            var r = dialog.SetFileDialog(model.DataFile);
            if (!r.ok)
                return;

            model.DataFile = r.fileName;
            await model.SaveFileAsync(); // Save As
        }

        public static async Task OpenFileAsync(this MainViewModel model)
        {
            _logger.LogDebug("OpenFileAsync");
            var path = model.DataFile;
            if (!File.Exists(path))
                await model.UserOpenFileAsync();
            else
                await model.OpenFileAsync(path);
        }



        public static void InitLogging(this MainViewModel model)
        {
            try
            {
                var serviceProvider = new ServiceCollection()
                    .AddLogging(logging => logging.AddConsole())
                    .AddLogging(logging => logging.AddDebug())
                    .AddLogging(logging => logging.AddProvider(new OutputLoggerProvider(model.Output.LogEvent)))
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

        public static void InitOutput(this MainViewModel model)
        {
            // MainModel
            model.Output.UpdateFilter = () => CollectionViewSource.GetDefaultView(model.Output.Records).Filter = o => model.Output.Filter(o);
        }

        public static void LoadSettings(this AppViewModel model, Window window)
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

                // dock
                model.LoadDockLayout(null);

                // model settings
                model.LastListTaskId = appSettings.GetGuid("LastListTaskId", Guid.Empty);
                model.LastTreeTaskId = appSettings.GetGuid("LastTreeTaskId", Guid.Empty);
                model.DataFile = appSettings.GetString("RecentFile", "New Data");

                // Output
                model.Output.OutputButtonErrors.IsChecked = appSettings.GetBool("OutputError", false);
                model.Output.OutputButtonDebug.IsChecked = appSettings.GetBool("OutputDebug", false);
                model.Output.OutputButtonMessages.IsChecked = appSettings.GetBool("OutputInfo", false);
                model.Output.OutputButtonWarnings.IsChecked = appSettings.GetBool("OutputWarning", false);

                model.MostRecentFiles.Clear();
                if (File.Exists(model.DataFile))
                    model.MostRecentFiles.Add(new FileInfo(model.DataFile));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        public static void SaveSettings(this AppViewModel model, Window window)
        {
            _logger.LogDebug("SaveSettings()");
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;


                model.PrepareSettings();

                // ISSUE: variable of a compiler-generated type
                // window settings
                if (window.WindowState != WindowState.Minimized)
                {
                    settings.SetValue("MainWindowTop", window.Top.ToString(CultureInfo.InvariantCulture));
                    settings.SetValue("MainWindowLeft", window.Left.ToString(CultureInfo.InvariantCulture));
                    settings.SetValue("MainWindowWidth", window.Width.ToString(CultureInfo.InvariantCulture));
                    settings.SetValue("MainWindowHeight", window.Height.ToString(CultureInfo.InvariantCulture));
                }

                // dock
                model.SaveDockLayout(null);

                // model settings
                settings.SetValue("RecentFile", model.DataFile);
                settings.SetValue("LastListTaskId", model.LastListTaskId.ToString());
                settings.SetValue("LastTreeTaskId", model.LastTreeTaskId.ToString());
                settings.SetValue("MainWindowState", window.WindowState.ToString());

                // Output
                settings.SetValue("OutputError", model.Output.OutputButtonErrors.IsChecked.ToString());
                settings.SetValue("OutputDebug", model.Output.OutputButtonDebug.IsChecked.ToString());
                settings.SetValue("OutputInfo", model.Output.OutputButtonMessages.IsChecked.ToString());
                settings.SetValue("OutputWarning", model.Output.OutputButtonWarnings.IsChecked.ToString());

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        public static async Task StartSavingAsync(this MainViewModel model)
        {
            model.CanSave = true;
            while (model.CanSave)
            {
                await model.SaveModifiedFileAsync();
                await Task.Run(() => Thread.Sleep(5000));
            }
        }


        public static void StopSaving(this MainViewModel model)
        {
            model.CanSave = false;
        }
    }
}