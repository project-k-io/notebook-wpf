using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Extensions;
using ProjectK.Notebook.ViewModels;
using ProjectK.Notebook.ViewModels.Extensions;
using ProjectK.Utils;
using ProjectK.Utils.Extensions;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;

namespace ProjectK.Notebook
{
    public class AppViewModel: MainViewModel
    {
        #region SaveDockLayoutCommand
        private ICommand _saveDockLayoutCommand;
        private readonly ILogger _logger;
        private const string DockFileName = "DockStates.xml";
        private const string FileNameRecentFiles = "RecentFiles.json";

        public AppViewModel()
        {
            Assembly = Assembly.GetExecutingAssembly();
            InitLogging();
            InitOutput();
            _logger = LogManager.GetLogger<ViewModels.MainViewModel>();
            _logger.LogDebug("Init Logging()");
        }


        private void InitLogging()
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void InitOutput()
        {
            Output.UpdateFilter = () => CollectionViewSource.GetDefaultView(Output.Records).Filter = o => Output.Filter(o);
        }


        public ICommand SaveDockLayoutCommand
        {
            get
            {
                _saveDockLayoutCommand = new DelegateCommand(SaveDockLayout, CanSelect);
                return _saveDockLayoutCommand;
            }
        }

        private bool CanSelect(object arg)
        {
            return true;
        }

        /// <summary>
        /// Helps to perform save and load operation of Docking Manager.
        /// </summary>
        /// <param name="obj"></param>
        public void SaveDockLayout(object obj)
        {
            if (!(Application.Current.MainWindow is MainWindow mainWindow))
                return;

            try
            {
                var writer = XmlWriter.Create(DockFileName);
                mainWindow.DockingManager.SaveDockState(writer);
                writer.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        #endregion

        #region LoadDockLayoutCommand
        private ICommand _loadDockLayoutCommand;

        public ICommand LoadDockLayoutCommand
        {
            get
            {
                _loadDockLayoutCommand = new DelegateCommand(LoadDockLayout, CanLoad);
                return _loadDockLayoutCommand;
            }
        }

        private bool CanLoad(object arg)
        {
            return true;
        }

        /// <summary>
        /// Helps to perform save and load operation of Docking Manager.
        /// </summary>
        /// <param name="obj"></param>
        public void LoadDockLayout(object obj)
        {
            if (!(Application.Current.MainWindow is MainWindow mainWindow))
                return;
            
            if(!File.Exists(DockFileName))
                return;

            try
            {
                var reader = XmlReader.Create(DockFileName);
                mainWindow.DockingManager.LoadDockState(reader);
                reader.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        #endregion

        #region AddCommand
        private ICommand _addCmd;

        public ICommand AddCommand
        {
            get
            {
                _addCmd = new DelegateCommand(Add, CanAdd);
                return _addCmd;
            }
        }

        private bool CanAdd(object arg)
        {
            return true;
        }

        /// <summary>
        /// Helps to perform save and load operation of Docking Manager.
        /// </summary>
        /// <param name="obj"></param>
        private void Add(object obj)
        {
            var count = 1;
            var contentControl = new ContentControl();
            contentControl.Name = "newChild" + count;
            DockingManager.SetHeader(contentControl, "New Child " + count);
            DockingManager.SetDesiredWidthInDockedMode(contentControl, 200);
            if (!(Application.Current.MainWindow is MainWindow mainWindow))
                return;

            mainWindow.DockingManager.Children.Add(contentControl);
            count++;
        }

        #endregion

        public async Task LoadRecentFiles()
        {
            _logger.LogDebug($"LoadRecentFiles | {FileNameRecentFiles} | {Path.GetDirectoryName(Path.GetFullPath(FileNameRecentFiles))}");

            var recentFiles = await FileHelper.ReadFromFileAsync<List<string>>(FileNameRecentFiles);
            if (recentFiles.IsNullOrEmpty())
                return;

            foreach (var recentFile in recentFiles)
            {
                _logger.LogDebug($"{recentFile}");
                var notebook = new NotebookViewModel
                {
                    DataFile = recentFile, 
                    RootTask = {Context = "Notebook"}
                };
                Notebooks.Add(notebook);
            }
        }

        public async Task SaveRecentFiles()
        {
            _logger.LogDebug("SaveRecentFiles");
            var recentFiles = new List<string>();
            foreach (var notebook in Notebooks)
            {
                var recentFile = notebook.DataFile;
                _logger.LogDebug($"{recentFile}");
                recentFiles.Add(recentFile);
            }

            await FileHelper.SaveToFileAsync(FileNameRecentFiles, recentFiles);
        }

        public void LoadSettings(Window window)
        {
            _logger.LogDebug("LoadSettings");
            // ISSUE: variable of a compiler-generated type
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                // window settings
                window.WindowState = appSettings.GetEnumValue("MainWindowState", WindowState.Normal);
                window.Top = appSettings.GetDouble("MainWindowTop", 100);
                window.Left = appSettings.GetDouble("MainWindowLeft", 100);
                window.Width = appSettings.GetDouble("MainWindowWidth", 800);
                window.Height = appSettings.GetDouble("MainWindowHeight", 400d);

                // dock
                LoadDockLayout(null);

                // model settings
                LastListTaskId = appSettings.GetGuid("LastListTaskId", Guid.Empty);
                LastTreeTaskId = appSettings.GetGuid("LastTreeTaskId", Guid.Empty);
#if AK
                Notebook.DataFile = appSettings.GetString("RecentFile", "New Data");
#endif
                // Output
                Output.OutputButtonErrors.IsChecked = appSettings.GetBool("OutputError", false);
                Output.OutputButtonDebug.IsChecked = appSettings.GetBool("OutputDebug", false);
                Output.OutputButtonMessages.IsChecked = appSettings.GetBool("OutputInfo", false);
                Output.OutputButtonWarnings.IsChecked = appSettings.GetBool("OutputWarning", false);

                MostRecentFiles.Clear();
#if AK
                if (File.Exists(Notebook.DataFile))
                    MostRecentFiles.Add(new FileInfo(Notebook.DataFile));
#endif
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
                // window settings
                if (window.WindowState != WindowState.Minimized)
                {
                    settings.SetValue("MainWindowTop", window.Top.ToString(CultureInfo.InvariantCulture));
                    settings.SetValue("MainWindowLeft", window.Left.ToString(CultureInfo.InvariantCulture));
                    settings.SetValue("MainWindowWidth", window.Width.ToString(CultureInfo.InvariantCulture));
                    settings.SetValue("MainWindowHeight", window.Height.ToString(CultureInfo.InvariantCulture));
                }

                // dock
                SaveDockLayout(null);

                // model settings
                settings.SetValue("RecentFile", SelectedNotebook?.DataFile);
                settings.SetValue("LastListTaskId", LastListTaskId.ToString());
                settings.SetValue("LastTreeTaskId", LastTreeTaskId.ToString());
                settings.SetValue("MainWindowState", window.WindowState.ToString());

                // Output
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
            CanSave = true;
            while (CanSave)
            {
                await SaveModifiedFileAsync();
                await Task.Run(() => Thread.Sleep(5000));
            }
        }


        public void StopSaving()
        {
            CanSave = false;
        }


        public CommandBindingCollection CreateCommandBindings()
        {
            var commandBindings = new CommandBindingCollection
            {
                new CommandBinding(ApplicationCommands.New, async (s, e) => await UserNewFileAsync(), (s, e) => e.CanExecute = true),
                new CommandBinding(ApplicationCommands.Open, async (s, e) =>  await this.UserAction_OpenFileAsync(), (s, e) => e.CanExecute = true),
                new CommandBinding(ApplicationCommands.Save, async (s, e) => await this.UserAction_SaveFileAsync(), (s, e) => e.CanExecute = true),
                new CommandBinding(ApplicationCommands.SaveAs, async (s, e) => await this.UserAction_SaveFileAsAsync(), (s, e) => e.CanExecute = true),
                new CommandBinding(ApplicationCommands.Close, async (s, e) => await UserNewFileAsync(), (s, e) => e.CanExecute = true)
            };
            return commandBindings;
        }

        public async Task OpenRecentFilesAsync()
        {
            _logger.LogDebug("Open Recent Files");
            foreach (var notebook in Notebooks)
            {
                var path = notebook.DataFile;
                if (!File.Exists(path))
                    continue;

                await notebook.OpenFileAsync(path);

                // add notebook node to root node
                RootTask.Add(notebook.RootTask);
                SelectedNotebook = notebook;
            }
        }


    }
}
