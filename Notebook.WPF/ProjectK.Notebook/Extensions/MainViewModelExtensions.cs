using System;
using System.Collections.Generic;
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
using ProjectK.Utils;
using ProjectK.Utils.Extensions;

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
            var r = dialog.SetFileDialog(model.SelectedNotebook.DataFile);
            if (!r.ok)
                return;

            model.SelectedNotebook.DataFile = r.fileName;
            model.FileOpenOldFormat();
        }


        private static async Task UserOpenFileAsync(this MainViewModel model)
        {
            _logger.LogDebug("UserOpenFileAsync()");
            var dialog = new OpenFileDialog();
            var r = dialog.SetFileDialog("model.Notebook?.DataFile");
            if (!r.ok)
                return;

            var notebook = new NotebookViewModel();

            await notebook.OpenFileAsync(r.fileName); // User clicked open file
            model.SelectedNotebook = notebook; 
            model.Notebooks.Add(notebook);
            model.RootTask.Add(notebook.RootTask);
        }


        public static async Task OpenRecentFilesAsync(this MainViewModel model)
        {

            foreach (var notebook in model.Notebooks)
            {
                _logger.LogDebug("OpenRecentFilesAsync");
                var path = notebook.DataFile;
                if (!File.Exists(path))
                    continue;

                await notebook.OpenFileAsync(path);

                // add notebook node to root node
                model.RootTask.Add(notebook.RootTask);
                model.SelectedNotebook = notebook;
            }
        }


        private static async Task UserSaveFileAsync(this MainViewModel model)
        {
            var notebook = model.SelectedNotebook;
            if (notebook == null)
                return;

            if (File.Exists(notebook.DataFile))
                await model.SaveFileAsync(); // User Save
            else
                await model.UserSaveFileAsAsync();
        }

        private static async Task UserSaveFileAsAsync(this MainViewModel model)
        {
            var notebook = model.SelectedNotebook;
            if(notebook == null)
                return;

            var dialog = new SaveFileDialog();
            var r = dialog.SetFileDialog(notebook.DataFile);
            if (!r.ok)
                return;

            notebook.DataFile = r.fileName;
            await model.SaveFileAsync(); // Save As
        }


        public static void OpenFileAsync1(string path)
        {
            // Notebooks.Add(notebook);
            // UseSettings();
            // var notebook = new NotebookViewModel();
        }



        public static void InitLogging(this MainViewModel model, Action<LogLevel, EventId, string> logEvent )
        {
            try
            {
                var serviceProvider = new ServiceCollection()
                    .AddLogging(logging => logging.AddConsole())
                    .AddLogging(logging => logging.AddDebug())
                    .AddLogging(logging => logging.AddProvider(new OutputLoggerProvider(logEvent)))
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
#if AK
                model.Notebook.DataFile = appSettings.GetString("RecentFile", "New Data");
#endif
                // Output
                model.Output.OutputButtonErrors.IsChecked = appSettings.GetBool("OutputError", false);
                model.Output.OutputButtonDebug.IsChecked = appSettings.GetBool("OutputDebug", false);
                model.Output.OutputButtonMessages.IsChecked = appSettings.GetBool("OutputInfo", false);
                model.Output.OutputButtonWarnings.IsChecked = appSettings.GetBool("OutputWarning", false);

                model.MostRecentFiles.Clear();
#if AK
                if (File.Exists(model.Notebook.DataFile))
                    model.MostRecentFiles.Add(new FileInfo(model.Notebook.DataFile));
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        private const string FileNameRecentFiles = "RecentFiles.json";

        public static async Task SaveRecentFiles(this AppViewModel model)
        {
            var recentFiles = new List<string>();
            foreach (var notebook in model.Notebooks)
            {
                var recentFile = notebook.DataFile;
                recentFiles.Add(recentFile);
            }

            await FileHelper.SaveToFileAsync(recentFiles, FileNameRecentFiles);
        }

        public static async Task LoadRecentFiles(this AppViewModel model)
        {
            var recentFiles = await FileHelper.ReadFromFileAsync<List<string>>(FileNameRecentFiles);
            if (recentFiles.IsNullOrEmpty())
                return;

            foreach (var recentFile in recentFiles)
            {
                var notebook = new NotebookViewModel();
                notebook.DataFile = recentFile;
                notebook.RootTask.Context = "Notebook";
                model.Notebooks.Add(notebook);
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
                settings.SetValue("RecentFile", model.SelectedNotebook?.DataFile);
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