﻿using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using ProjectK.Logging;
using ProjectK.Notebook.ViewModels;
using ProjectK.Notebook.WinApp.Settings;
using ProjectK.Utils;
using ProjectK.View.Helpers.Extensions;
using ProjectK.ViewModels;

namespace ProjectK.Notebook.WinApp
{
    public class AppViewModel : MainViewModel
    {
        #region Commands

        public AppSettings _settings;

        #endregion

        #region Properties

        public LayoutViewModel Layout { get; } = new LayoutViewModel();


        #endregion

        #region Constructors

        public AppViewModel(IOptions<AppSettings> settings)
        {
            if(settings != null)
                _settings = settings.Value;

            Assembly = Assembly.GetExecutingAssembly();
            Logger = LogManager.GetLogger<MainViewModel>();
            Logger.LogDebug("Import Logging()");
        }

        #endregion

        #region Private Functions

        public void InitOutput(OutputViewModel output)
        {
            Output = output;
            Output.UpdateFilter = () =>
                CollectionViewSource.GetDefaultView(Output.Records).Filter = o => Output.Filter(o);
        }

        #endregion

        #region Public Functions

        public void LoadSettings()
        {
            Logger.LogDebug("LoadSettings");
            // ISSUE: variable of a compiler-generated type
            try
            {
                // Layout
                Layout.OutputHeight = _settings.Layout.OutputHeight;
                Layout.NavigatorWidth = _settings.Layout.NavigatorWidth;
                Layout.PropertiesWidth = _settings.Layout.PropertiesWidth;

                // model settings
                LastListTaskId = _settings.LastListTaskId;
                LastTreeTaskId = _settings.LastTreeTaskId;

                // Output
                Output.ButtonErrors.IsChecked = _settings.Layout.Output.Error;
                Output.ButtonDebug.IsChecked = _settings.Layout.Output.Debug;
                Output.ButtonMessages.IsChecked = _settings.Layout.Output.Info;
                Output.ButtonWarnings.IsChecked = _settings.Layout.Output.Warning;

                MostRecentFiles.Clear();
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
                // Layout
                _settings.Layout.OutputHeight = Layout.OutputHeight;
                _settings.Layout.NavigatorWidth  = Layout.NavigatorWidth;
                _settings.Layout.PropertiesWidth = Layout.PropertiesWidth;

                // model settings
                _settings.LastListTaskId = LastListTaskId;
                _settings.LastTreeTaskId = LastTreeTaskId;

                //// Output
                _settings.Layout.Output.Error = Output.ButtonErrors.IsChecked;
                _settings.Layout.Output.Debug = Output.ButtonDebug.IsChecked;
                _settings.Layout.Output.Info = Output.ButtonMessages.IsChecked;
                _settings.Layout.Output.Warning = Output.ButtonWarnings.IsChecked;

                MostRecentFiles.Clear();
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
            SetTitle(key);
        }
        public async Task OpenFileAsync()
        {
            try
            {
                Logger.LogDebug("OpenFileAsync()");
                var dialog = new OpenFileDialog();
                var (path, ok) = dialog.SetFileDialog("");
                if (!ok)
                    return;

                await OpenFileAsync(path);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }



        #endregion
    }
}