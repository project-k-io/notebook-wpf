﻿using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Data;
using ProjectK.Notebook.Domain;
using ProjectK.Notebook.Extensions;
// using ProjectK.NotebookModel.Models.Versions.Version2;
using ProjectK.Notebook.ViewModels;
using ProjectK.Notebook.ViewModels.Extensions;
using ProjectK.Utils;
using ProjectK.Utils.Extensions;
using SQLitePCL;
using Syncfusion.Windows.Tools.Controls;
using Task = System.Threading.Tasks.Task;

namespace ProjectK.Notebook
{
    public class AppViewModel: MainViewModel
    {
        #region Consts

        private const string DockFileName = "DockStates.xml";
        private const string FileNameRecentFiles = "RecentFiles.json";

        #endregion

        #region Fields

        private readonly NotebookContext _db = new NotebookContext();

        #endregion

        #region Commands
        public ICommand LoadDockLayoutCommand { get; }
        public ICommand SaveDockLayoutCommand { get; }
        public ICommand AddCommand { get; }

        #endregion

        #region Constructors
        public AppViewModel()
        {
            LoadDockLayoutCommand = new RelayCommand(LoadDockLayout);
            SaveDockLayoutCommand = new RelayCommand(SaveDockLayout);
            AddCommand = new RelayCommand(Add);

            Assembly = Assembly.GetExecutingAssembly();
            InitLogging();
            InitOutput();
            Logger = LogManager.GetLogger<MainViewModel>();
            Logger.LogDebug("Init Logging()");
            MessengerInstance.Register<NotificationMessage<NodeModel>>(this, NotifyMe);
        }

        private void NotifyMe(NotificationMessage<NodeModel> notificationMessage)
        {
            var notification = notificationMessage.Notification;
            var model = notificationMessage.Content;
            if (notification == "Modified")
            {
                Logger.LogDebug($"Model={model.Name} {notification}");
                // _db.SaveChanges();
            }
        }


        public void NotifyMe(NotificationMessageAction<NodeModel> notificationMessageAction)
        {
            string notification = notificationMessageAction.Notification;
            //do your work
            notificationMessageAction.Execute("callback parameter"); //Execute the callback
        }

        #endregion

        #region DockingManager



        public void SaveDockLayout()
        {
            if (!(Application.Current.MainWindow is MainWindow window))
                return;

            SaveDockLayout(window);
        }


        /// <summary>
        /// Helps to perform save and load operation of Docking Manager.
        /// </summary>
        /// <param name="window"></param>
        public void SaveDockLayout(MainWindow window)
        {
            try
            {
                var writer = XmlWriter.Create(DockFileName);
                window.DockingManager.SaveDockState(writer);
                writer.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void LoadDockLayout()
        {
            if (!(Application.Current.MainWindow is MainWindow window))
                return;

            LoadDockLayout(window);
        }

        /// <summary>
            /// Helps to perform save and load operation of Docking Manager.
            /// </summary>
            /// <param name="window"></param>
        public void LoadDockLayout(MainWindow window)
        {
            if(!File.Exists(DockFileName))
                return;

            try
            {
                var reader = XmlReader.Create(DockFileName);
                window.DockingManager.LoadDockState(reader);
                reader.Close();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                throw;
            }
        }
        #endregion

        #region Override

        public override void OpenDatabase()
        {
            // this is for demo purposes only, to make it easier
            // to get up and running
            _db.Database.EnsureCreated();

            // load the entities into EF Core
            _db.Notebooks.Load();

            // bind to the source
            NotebookModels = _db.Notebooks.Local.ToObservableCollection();

            foreach (var model in NotebookModels)
            {
                var notebook = AddNotebook(model);
                SelectedNotebook = notebook;
            }
        }
        public override void SyncDatabase()
        {
            _db.SaveChanges();
            RootTask.ResetParentChildModified();
            RootTask.ResetModified();
        }

        public override void ImportNotebook(NotebookModel notebookModel, Domain.Versions.Version2.DataModel dataModel)
        {
            Logger.LogDebug($"Import NotebookModel: {notebookModel.Name}");

            // Add NotebookModel
            NotebookModels.Add(notebookModel);
            _db.SaveChanges();

            // Add Tasks
            notebookModel.Init(dataModel);
            _db.SaveChanges();

            AddNotebook(notebookModel);
        }

        #endregion

        #region Private Functions

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

        /// <summary>
        /// Helps to perform save and load operation of Docking Manager.
        /// </summary>
        /// <param name="obj"></param>
        private void Add()
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


        public void LoadSettings(MainWindow window)
        {
            Logger.LogDebug("LoadSettings");
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
                LoadDockLayout(window);

                // model settings
                LastListTaskId = appSettings.GetGuid("LastListTaskId", Guid.Empty);
                LastTreeTaskId = appSettings.GetGuid("LastTreeTaskId", Guid.Empty);
                if(SelectedNotebook != null)
                    SelectedNotebook.Title = appSettings.GetString("RecentFile", "New Data");

                // Output
                Output.OutputButtonErrors.IsChecked = appSettings.GetBool("OutputError", false);
                Output.OutputButtonDebug.IsChecked = appSettings.GetBool("OutputDebug", false);
                Output.OutputButtonMessages.IsChecked = appSettings.GetBool("OutputInfo", false);
                Output.OutputButtonWarnings.IsChecked = appSettings.GetBool("OutputWarning", false);

                MostRecentFiles.Clear();
                if (File.Exists(SelectedNotebook?.Title))
                    MostRecentFiles.Add(new FileInfo(SelectedNotebook?.Title));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        public void SaveSettings(MainWindow window)
        {
            Logger.LogDebug("SaveSettings()");
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
                SaveDockLayout(window);

                // model settings
                settings.SetValue("RecentFile", SelectedNotebook?.Title);
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
                Logger.LogError(ex);
            }
        }
        public CommandBindingCollection CreateCommandBindings()
        {
            var commandBindings = new CommandBindingCollection
            {
                new CommandBinding(ApplicationCommands.New, async (s, e) => await UserNewFileAsync(), (s, e) => e.CanExecute = true),
                new CommandBinding(ApplicationCommands.Open, async (s, e) =>  await this.UserAction_OpenFileAsync(), (s, e) => e.CanExecute = true),
                new CommandBinding(ApplicationCommands.Close, async (s, e) => await UserNewFileAsync(), (s, e) => e.CanExecute = true)
            };
            return commandBindings;
        }

        private NotebookViewModel AddNotebook(NotebookModel model)
        {
            Logger.LogDebug($"AddNotebook: {model.Name}");
            
            var notebook = new NotebookViewModel(model);
            notebook.ModelToViewModel();

            SelectedNotebook = notebook;
            Notebooks.Add(notebook);
            RootTask.Add(notebook.RootTask);
            // add notebookModel task to root task
            return notebook;
        }

        private async Task UserNewFileAsync()
        {
            Logger.LogDebug("UserNewFileAsync");
            CanSave = false;

            var notebook = new NotebookViewModel();
            var path = FileHelper.MakeUnique(notebook.Title);


            Notebooks.Add(notebook);
            RootTask.Add(notebook.RootTask);
            SelectedNotebook = notebook;
            CanSave = true;
        }

    }
}