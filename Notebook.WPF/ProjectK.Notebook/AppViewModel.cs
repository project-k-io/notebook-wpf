using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using ProjectK.Logging;
using ProjectK.Notebook.Settings;
using ProjectK.Notebook.ViewModels;
using ProjectK.Utils;
using ProjectK.View.Helpers.Extensions;
using ProjectK.ViewModels;

namespace ProjectK.Notebook
{
    public class AppViewModel : MainViewModel
    {
        #region Constructors

        public AppViewModel(IOptions<AppSettings> settings)
        {
            _settings = settings.Value;
            AddCommand = new RelayCommand(Add);

            Assembly = Assembly.GetExecutingAssembly();
            Logger = LogManager.GetLogger<MainViewModel>();
            Logger.LogDebug("Import Logging()");
        }

        #endregion


        public void LoadSettings()
        {
            Logger.LogDebug("LoadSettings");
            // ISSUE: variable of a compiler-generated type
            try
            {
                // model settings
                LastListTaskId = _settings.LastListTaskId;
                LastTreeTaskId = _settings.LastTreeTaskId;
                if (SelectedNotebook != null)
                    SelectedNotebook.Title = _settings.RecentFile;

                // Output
                Output.ButtonErrors.IsChecked = _settings.Layout.Output.Error;
                Output.ButtonDebug.IsChecked = _settings.Layout.Output.Debug;
                Output.ButtonMessages.IsChecked = _settings.Layout.Output.Info;
                Output.ButtonWarnings.IsChecked = _settings.Layout.Output.Warning;

                MostRecentFiles.Clear();
                if (File.Exists(SelectedNotebook?.Title))
                    MostRecentFiles.Add(new FileInfo(SelectedNotebook?.Title));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }


        public void SaveSettings()
        {
            Logger.LogDebug("SaveSettings()");
            try
            {
                PrepareSettings();

                // model settings
                _settings.LastListTaskId = LastListTaskId;
                _settings.LastTreeTaskId = LastTreeTaskId;
                if (SelectedNotebook != null)
                    _settings.RecentFile = SelectedNotebook.Title;

                //// Output
                _settings.Layout.Output.Error = Output.ButtonErrors.IsChecked;
                _settings.Layout.Output.Debug = Output.ButtonDebug.IsChecked;
                _settings.Layout.Output.Info = Output.ButtonMessages.IsChecked;
                _settings.Layout.Output.Warning = Output.ButtonWarnings.IsChecked;

                MostRecentFiles.Clear();
                if (File.Exists(SelectedNotebook?.Title))
                    MostRecentFiles.Add(new FileInfo(SelectedNotebook?.Title));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        public CommandBindingCollection CreateCommandBindings()
        {
            var commandBindings = new CommandBindingCollection
            {
                new CommandBinding(ApplicationCommands.Open, async (s, e) => await OpenFileAsync(),
                    (s, e) => e.CanExecute = true)
            };
            return commandBindings;
        }

        public async Task SaveAppSettings(string directory)
        {
            var path = Path.Combine(directory, "appsettings.json");
            var root = new
            {
                AppSettings = _settings
            };
            await FileHelper.SaveToFileAsync(path, root);
        }

        public void OpenDatabase()
        {
            var key = "AlanDatabase";
            // var key = "TestDatabase";
            var connectionString = _settings.Connections[key];
            OpenDatabase(connectionString);
        }

        public async Task OpenFileAsync()
        {
            try
            {
                Logger.LogDebug("OpenFileAsync()");
                var dialog = new OpenFileDialog();
                var r = dialog.SetFileDialog(SelectedNotebook?.Title);
                if (!r.ok)
                    return;

                var path = r.fileName;
                await OpenFileAsync(path);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }

        #region Commands

        public ICommand LoadDockLayoutCommand { get; set; }
        public ICommand SaveDockLayoutCommand { get; set; }
        public ICommand AddCommand { get; }
        public AppSettings _settings;

        #endregion


        #region Private Functions

        public void InitOutput(OutputViewModel output)
        {
            Output = output;
            Output.UpdateFilter = () =>
                CollectionViewSource.GetDefaultView(Output.Records).Filter = o => Output.Filter(o);
        }

        /// <summary>
        ///     Helps to perform save and load operation of Docking Manager.
        /// </summary>
        /// <param name="obj"></param>
        private void Add()
        {
            var count = 1;
            var contentControl = new ContentControl();
            contentControl.Name = "newChild" + count;
            if (!(Application.Current.MainWindow is MainWindow mainWindow))
                return;
        }

        #endregion
    }
}