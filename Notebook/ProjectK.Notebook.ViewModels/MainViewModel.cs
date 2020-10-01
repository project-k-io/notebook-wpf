﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using System.Xml;
using Castle.Core.Internal;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectK.Notebook.Data;
using ProjectK.Notebook.Domain;
using ProjectK.Notebook.Domain.Reports;
using ProjectK.Notebook.ViewModels.Enums;
using ProjectK.Notebook.ViewModels.Extensions;
using ProjectK.Notebook.ViewModels.Reports;
using ProjectK.Notebook.ViewModels.Services;
using ProjectK.Utils;
using ProjectK.Utils.Extensions;
using ProjectK.ViewModels;
using Task = System.Threading.Tasks.Task;

namespace ProjectK.Notebook.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Static

        protected ILogger Logger;

        static readonly List<string> GlobalContextList = new List<string>
        {
            "Notebook",
            "Company",
            "Contact",
            "Node",
            "Task",
            "Note",
            "Task Manager",
            "Year",
            "Month",
            "Day",
            "Week"
        };

        public static Guid RootGuid = new Guid("98601237-050a-4915-860c-5a820b361910");

        #endregion

        #region Fields

        private readonly NotebookContext _db = new NotebookContext();
        private NotebookViewModel _selectedNotebook;
        private ReportTypes _reportType = ReportTypes.Notes;
        private string _excelCsvText;
        private string _textReport;
        private string _title;
        private bool _useTimeOptimization;

        #endregion

        #region Properties

        public ObservableCollection<NotebookViewModel> Notebooks { get; set; } = new ObservableCollection<NotebookViewModel>();
        public ObservableCollection<FileInfo> MostRecentFiles { get; } = new ObservableCollection<FileInfo>();
        public ObservableCollection<string> TypeList { get; set; }
        public ObservableCollection<string> ContextList { get; set; }
        public ObservableCollection<string> TaskTitleList { get; set; }
        // public ObservableCollection<NotebookModel> NotebookModels { get; set; }

        public ReportTypes ReportType
        {
            get => _reportType;
            set
            {
                if (!Set(ref _reportType, value)) return;
            }
        }

        public Assembly Assembly { get; set; }

        public string Title
        {
            get => _title;
            set
            {
                if (_title == null)
                    return;

                _title = value;
                RaisePropertyChanged();
            }
        }

        public string TextReport
        {
            get => _textReport;
            set => Set(ref _textReport, value);
        }

        public Guid LastListTaskId { get; set; }
        public Guid LastTreeTaskId { get; set; }


        public NodeViewModel RootTask { get; set; }

        public NotebookViewModel SelectedNotebook
        {
            get => _selectedNotebook;
            set => Set(ref _selectedNotebook, value);
        }

        public string ExcelCsvText
        {
            get => _excelCsvText;
            set => Set(ref _excelCsvText, value);
        }

        public bool UseTimeOptimization
        {
            get => _useTimeOptimization;
            set => Set(ref _useTimeOptimization, value);
        }

        public bool CanSave { get; set; }
        public Action<Action> OnDispatcher { get; set; }
        public OutputViewModel Output { get; set; } = new OutputViewModel();


        #endregion

        #region Commands

        public ICommand ClearCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand FixTimeCommand { get; }
        public ICommand ExtractContextCommand { get; }
        public ICommand FixContextCommand { get; private set; }
        public ICommand FixTitlesCommand { get; }
        public ICommand FixTypesCommand { get; }
        public ICommand CopyTaskCommand { get; }
        public ICommand ContinueTaskCommand { get; }
        public ICommand ShowReportCommand { get; }
        public ICommand ExportSelectedAllAsTextCommand { get; }
        public ICommand ExportSelectedAllAsJsonCommand { get; }
        public ICommand OpenDatabaseCommand { get; }
        public ICommand SyncDatabaseCommand { get; }
        public ICommand AddNotebookCommand { get; }

        #endregion

        #region Consuctors

        public MainViewModel()
        {
            var rootModel = new 
            {
                Id = RootGuid,
                ParentId = Guid.Empty,
                Context = "Root",
                Created = DateTime.Now,
                Name = "Root",
            };

            RootTask = new NodeViewModel(rootModel);


            CanSave = true;
            TypeList = new ObservableCollection<string>();
            ContextList = new ObservableCollection<string>();
            TaskTitleList = new ObservableCollection<string>();
            ClearCommand = new RelayCommand(this.UserAction_Clear);
            EditCommand = new RelayCommand(this.UserAction_Edit);
            FixTimeCommand = new RelayCommand(() => SelectedNotebook?.FixTime());
            ExtractContextCommand = new RelayCommand(() => SelectedNotebook?.FixContext());
            FixTitlesCommand = new RelayCommand(() => SelectedNotebook.FixTitles());
            FixTypesCommand = new RelayCommand(() => SelectedNotebook.FixTypes());
            CopyTaskCommand = new RelayCommand(CopyTask);
            ContinueTaskCommand = new RelayCommand(ContinueTask);
            ShowReportCommand = new RelayCommand<ReportTypes>(this.UserAction_ShowReport);
            ExportSelectedAllAsTextCommand =
                new RelayCommand(async () => await this.UserAction_ExportSelectedAllAsText());
            ExportSelectedAllAsJsonCommand =
                new RelayCommand(async () => await this.UserAction_ExportSelectedAllAsJson());
            OpenDatabaseCommand = new RelayCommand(OpenDatabase);
            SyncDatabaseCommand = new RelayCommand(async () => await SyncDatabaseAsync());
            AddNotebookCommand = new RelayCommand(async () => await AddNotebookAsync());

            // Add Context 
            foreach (var context in GlobalContextList)
            {
                ContextList.Add(context);
            }

            MessengerInstance.Register<NotificationMessage<NodeViewModel>>(this, NotifyMe);
        }

        private void NotifyMe(NotificationMessage<NodeViewModel> notificationMessage)
        {
            var notification = notificationMessage.Notification;
            var node = notificationMessage.Content;
            Logger.LogDebug($"Model={node.Name} {notification}");
            switch (notification)
            {
                case "Modified":
                    break;
                case "Delete":
                    DeleteNode(node);
                    break;
                case "Add":
                    AddNode(node);
                    break;
            }
        }

        private void AddNode(NodeViewModel vm)
        {
            var model = vm.Model;
            var notebook = SelectedNotebook.Model;
            notebook.AddModel(model);
        }

        private void DeleteNode(NodeViewModel node)
        {
            if (node.Context == "Notebook")
            {
                var notebook = Notebooks.First(n => n.RootTask.Id == node.Id);
                if (notebook == null)
                    return;

                Notebooks.Remove(notebook);
                _db.Notebooks.Remove(notebook.Model);
            }
            else
            {
                var models = node.GetModels();
                _db.RemoveRange(models);
            }
        }

        public async Task SyncDatabaseAsync()
        {
            SaveRootNodes();
            SaveNonRootNodes();
            await _db.SaveChangesAsync();
            RootTask.ResetParentChildModified();
            RootTask.ResetModified();
        }

        private void SaveRootNodes()
        {
            // find root nodes not in notebooks
            foreach (var notebook in Notebooks)
            {
                // Get Notebook nodes
                var models = notebook.RootTask.Nodes.GetModels();
                notebook.Model.AddModels(models);
            }
        }

        private void SaveNonRootNodes()
        {
            // find root nodes not in notebooks
            var nodes = RootTask.Nodes.Where(n => n.ParentId == RootTask.Id && n.Context != "Notebook").ToList();
            if(nodes.IsNullOrEmpty())
                return;

            // find non root notebook
            var notebooks = _db.Notebooks.Where(n => n.NonRoot).ToArray();
            
            NotebookModel notebook;
            if (!notebooks.IsNullOrEmpty())
                notebook = notebooks[0];
            else
            {
                notebook = new NotebookModel
                {
                    NonRoot = true,
                    Created = DateTime.Now,
                    Context = "Notebook",
                    Description = "Notebook for nodes outside of notebooks",
                    Name = "Non Root Notebook",
                    Id = Guid.NewGuid()
                };
                _db.Notebooks.Add(notebook);
            }

            // Add top nodes to notebook
            // Get all nodes
            var models = nodes.GetModels();

            // Add modes to notebook
            notebook.AddModels(models);
        }

        public void OpenDatabase()
        {
            // this is for demo purposes only, to make it easier
            // to get up and running
            _db.Database.EnsureCreated();

            // load the entities into EF Core
            _db.Notebooks.Load();

            // bind to the source
            var notebookModels = _db.Notebooks.Local.ToObservableCollection();

            var models = new List<ItemModel>();
            foreach (var model in notebookModels)
            {
                var (notebook, nodes) = AddNotebook(model);
                if(notebook != null)
                    SelectedNotebook = notebook;

                models.AddRange(nodes);
            }

            // ModelToViewModel Data
            UpdateTypeListAsync(models);
        }

        public async Task CloseDatabaseAsync()
        {
            await SyncDatabaseAsync();
            await _db.SaveChangesAsync();
            await _db.Database.CloseConnectionAsync();
        }

        private async Task AddNotebookAsync()
        {
            Logger.LogDebug("AddNotebook");
            // Create Notebook
            var model = new NotebookModel
            {
                Name = "Notebook",
                Id = Guid.NewGuid(),
                Context = "Notebook",
                Created = DateTime.Now,
            };

            ImportNotebook(model);
            await SyncDatabaseAsync();
        }
        public void ImportNotebook(NotebookModel notebookModel)
        {
            Logger.LogDebug($"Import NotebookModel: {notebookModel.Name}");

            // Add NotebookModel
            var a = _db.Notebooks.Add(notebookModel);
            // this will create Primary Key for notebook
            var (notebook, nodes) = AddNotebook(notebookModel);
            if (notebook != null)
                SelectedNotebook = notebook;

            UpdateTypeListAsync(nodes);
        }

        private (NotebookViewModel, List<ItemModel>) AddNotebook(NotebookModel model)
        {
            Logger.LogDebug($"AddNotebook: {model.Name}");

            var items = model.GetItems();
            if (model.NonRoot)
            {
                RootTask.BuildTree(items);
                return (null, items);
            }
            else
            {
                var notebook = new NotebookViewModel(model);
                Notebooks.Add(notebook);
                notebook.RootTask.BuildTree(items);
                RootTask.Add(notebook.RootTask);
                return (notebook, items);
            }
        }
        private void OnCurrentNotebookChanged()
        {
            var noteBookName = SelectedNotebook != null ? SelectedNotebook.RootTask.Name : "";
            Title = Assembly.GetAssemblyTitle() + " " + Assembly.GetAssemblyVersion() + " - " + noteBookName;
        }

        #endregion

        #region Delegates

        public event EventHandler<TaskEventArgs> SelectedTaskChanged;
        public Action CurrentNotebookChanged { get; set; }

        #endregion

        #region Public functions

        public void FileOpenOldFormat()
        {
            //AK SelectedNotebook.LoadFrom(Models.Versions.Version1.NotebookModel.ReadFromFile(Title));
        }


        public void UpdateTypeListAsync(List<ItemModel> nodes)
        {
            var sortedSet1 = new SortedSet<string>();
            var sortedSet2 = new SortedSet<string>();
            var sortedSet3 = new SortedSet<string>();
            foreach (var node in nodes)
            {
                if (node is TaskModel task)
                {
                    var type = task.Type;
                    if (!sortedSet1.Contains(type)) sortedSet1.Add(type);
                }

                var context = node.Context;
                if (!sortedSet2.Contains(context)) sortedSet2.Add(context);

                var title = node.Name;
                if (!sortedSet3.Contains(title)) sortedSet3.Add(title);
            }

            TypeList.Clear();
            foreach (var str in sortedSet1) TypeList.Add(str);

            ContextList.Clear();
            foreach (var str in sortedSet2) ContextList.Add(str);

            TaskTitleList.Clear();
            foreach (var str in sortedSet3) TaskTitleList.Add(str);
        }




        public void OnSelectedTaskChanged(NodeViewModel task)
        {
            SelectedTaskChanged?.Invoke(this, new TaskEventArgs
            {
                Task = task
            });
        }

        private readonly WorksheetReport _worksheetReport = new WorksheetReport();
        private readonly NotesReport _notesReport = new NotesReport();

        public void OnGenerateReportChanged()
        {
            switch (ReportType)
            {
                case ReportTypes.Worksheet:
                    _worksheetReport.GenerateReport(this);
                    break;
                case ReportTypes.Notes:
                    TextReport = _notesReport.GenerateReport(SelectedNotebook?.SelectedNode);
                    break;
            }
        }

        public void PrepareSettings()
        {
            if (SelectedNotebook?.SelectedNode != null)
                LastListTaskId = SelectedNotebook.SelectedNode.Id;

            if (SelectedNotebook?.SelectedTreeNode == null) return;

            LastTreeTaskId = SelectedNotebook.SelectedTreeNode.Id;
        }

        public void SelectTreeTask(NodeViewModel task)
        {
            task.TypeList = TypeList;
            task.ContextList = ContextList;
            task.TitleList = TaskTitleList;
            var notebook = FindNotebook(task);
            if (notebook != null)
            {
                SelectedNotebook = notebook;
                SelectedNotebook.SelectedNode = task;
                Logger.LogDebug($"SelectedNotebook selected | {notebook.Title}");
            }

            SelectedNotebook?.SelectTreeTask(task);
            OnGenerateReportChanged();
        }


        public NotebookViewModel FindNotebook(NodeViewModel task)
        {
            var (ok, notebookNode) = task.FindNode(t => t.Context == "Notebook");
            if (!ok)
                return null;

            var path = notebookNode.Name;

            var notebook = Notebooks.FirstOrDefault(n => n.Title == path);
            return notebook;
        }


        #endregion

        #region Private functions

        private void UseSettings()
        {
            if (SelectedNotebook == null)
                return;

            SelectedNotebook.SelectTreeTask(LastListTaskId);
            SelectedNotebook.SelectTask(LastTreeTaskId);
        }

        private void CopyExcelCsvText()
        {
            if (SelectedNotebook == null)
                return;

            var stringReader = new StringReader(ExcelCsvText);
            var excelCsvRecordList = new List<ExcelCsvRecord>();
            string line;
            while ((line = stringReader.ReadLine()) != null)
                if (!string.IsNullOrEmpty(line))
                {
                    var excelCsvRecord = new ExcelCsvRecord();
                    if (excelCsvRecord.TryParse(line)) excelCsvRecordList.Add(excelCsvRecord);
                }

            var selectedTreeTask = SelectedNotebook.SelectedTreeNode;
            if (selectedTreeTask.Context != "Week") return;

            foreach (var excelCsvRecord in excelCsvRecordList)
            {
                var dateTime1 = excelCsvRecord.Day;
                var dayOfTheWeek = dateTime1.DayOfWeek.ToString();
                var taskViewModel1 = selectedTreeTask.Nodes.FirstOrDefault(t => t.Name == dayOfTheWeek);
                if (taskViewModel1 == null)
                {
                    taskViewModel1 = new TaskViewModel()
                    {
                        Name = dayOfTheWeek,
                        DateStarted = excelCsvRecord.Day,
                        DateEnded = excelCsvRecord.Day
                    };
                    selectedTreeTask.Nodes.Add(taskViewModel1);
                }

                var taskViewModel2 = new TaskViewModel()
                {
                    Name = excelCsvRecord.Task,
                    Context = "TaskModel"
                };

                var taskViewModel3 = taskViewModel2;
                dateTime1 = excelCsvRecord.Day;
                var year1 = dateTime1.Year;
                dateTime1 = excelCsvRecord.Day;
                var month1 = dateTime1.Month;
                dateTime1 = excelCsvRecord.Day;
                var day1 = dateTime1.Day;
                dateTime1 = excelCsvRecord.Start;
                var hour1 = dateTime1.Hour;
                dateTime1 = excelCsvRecord.Start;
                var minute1 = dateTime1.Minute;
                dateTime1 = excelCsvRecord.Start;
                var second1 = dateTime1.Second;
                var dateTime2 = new DateTime(year1, month1, day1, hour1, minute1, second1);

                taskViewModel3.DateStarted = dateTime2;
                var taskViewModel4 = taskViewModel2;
                dateTime1 = excelCsvRecord.Day;
                var year2 = dateTime1.Year;
                dateTime1 = excelCsvRecord.Day;
                var month2 = dateTime1.Month;
                dateTime1 = excelCsvRecord.Day;
                var day2 = dateTime1.Day;
                dateTime1 = excelCsvRecord.End;
                var hour2 = dateTime1.Hour;
                dateTime1 = excelCsvRecord.End;
                var minute2 = dateTime1.Minute;
                dateTime1 = excelCsvRecord.End;
                var second2 = dateTime1.Second;
                var dateTime3 = new DateTime(year2, month2, day2, hour2, minute2, second2);
                taskViewModel4.DateEnded = dateTime3;
                taskViewModel2.Description = $"{excelCsvRecord.Type1}:{excelCsvRecord.Type2}:{excelCsvRecord.SubTask}";
                taskViewModel1.Nodes.Add(taskViewModel2);
            }
        }

        private void OnTreeViewKeyDown(KeyboardKeys keyboardKeys, KeyboardStates keyboardState)
        {
            var service = new ActionService
            {
                GetState = () => keyboardState,
                Handled = () => { },
                SelectItem = t => t.IsSelected = true,
                ExpandItem = t => t.IsExpanded = true,
                DeleteMessageBox = () => true,
                Dispatcher = OnDispatcher
            };
            SelectedNotebook.SelectedNode?.KeyboardAction(keyboardKeys, service);
        }

        private void CopyTask()
        {
            OnTreeViewKeyDown(KeyboardKeys.Insert, KeyboardStates.IsControlPressed);
        }

        private void ContinueTask()
        {
            OnTreeViewKeyDown(KeyboardKeys.Insert, KeyboardStates.IsShiftPressed);
        }


        #endregion

    }

}