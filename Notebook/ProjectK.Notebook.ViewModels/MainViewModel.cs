﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Castle.Core.Internal;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectK.Notebook.Data;
using ProjectK.Notebook.Domain;
using ProjectK.Notebook.Domain.Interfaces;
using ProjectK.Notebook.Domain.Reports;
using ProjectK.Notebook.ViewModels.Enums;
using ProjectK.Notebook.ViewModels.Extensions;
using ProjectK.Notebook.ViewModels.Helpers;
using ProjectK.Notebook.ViewModels.Interfaces;
using ProjectK.Notebook.ViewModels.Reports;
using ProjectK.Notebook.ViewModels.Services;
using ProjectK.Utils;
using ProjectK.Utils.Extensions;
using ProjectK.ViewModels;

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
            "Time Tracker",
            "Year",
            "Month",
            "Day",
            "Week"
        };

        public static Guid RootGuid = new Guid("98601237-050a-4915-860c-5a820b361910");

        #endregion

        #region Fields

        private NotebookContext _db;
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

            // var connectionString = @"Data Source=D:\\db\\alan_notebooks.db";
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
            CopyTaskCommand = new RelayCommand(async () => await CopyTask());
            ContinueTaskCommand = new RelayCommand(async () => await ContinueTask());
            ShowReportCommand = new RelayCommand<ReportTypes>(this.UserAction_ShowReport);
            ExportSelectedAllAsTextCommand =
                new RelayCommand(async () => await this.UserAction_ExportSelectedAllAsText());
            ExportSelectedAllAsJsonCommand =
                new RelayCommand(async () => await this.UserAction_ExportSelectedAllAsJson());
            
            // OpenDatabaseCommand = new RelayCommand(OpenDatabase);
            SyncDatabaseCommand = new RelayCommand(async () => await SyncDatabaseAsync());
            AddNotebookCommand = new RelayCommand(async () => await AddNotebookAsync());

            // Add Context 
            ContextList.AddRange(GlobalContextList);

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

                // Selected Notebook
                if(Notebooks.Count == 0)
                {
                    SelectedNotebook = null;
                }
                else
                {
                    if (notebook.Model.Id == SelectedNotebook.Model.Id)
                    {
                        SelectedNotebook = Notebooks.FirstOrDefault();
                    }
                }
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

        public void OpenDatabase(string connectionString)
        {
            _db = new NotebookContext(connectionString);

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
            var notebookNames = Notebooks.Select(notebook => notebook.Title).ToList();
            var notebookName = StringHelper.GetUniqueName("Notebook", notebookNames);

            // Create Notebook
            var model = new NotebookModel
            {
                Name = notebookName,
                Id = Guid.NewGuid(),
                Context = "Notebook",
                Created = DateTime.Now,
            };

            // var names = Notebooks.ToList().Where(n => n.Title).ToList();

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

            var notebook = new NotebookViewModel(model);
            Notebooks.Add(notebook);
            notebook.RootTask.BuildTree(items);
            RootTask.Add(notebook.RootTask);
            return (notebook, items);
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
            var types = new SortedSet<string>();
            var contexts = new SortedSet<string>();
            var titles = new SortedSet<string>();

            foreach (var node in nodes)
            {
                if (node is TaskModel task)
                {
                    var type = task.Type;
                    if (!types.Contains(type)) types.Add(type);
                }

                var context = node.Context;
                if (!contexts.Contains(context)) contexts.Add(context);

                var title = node.Name;
                if (!titles.Contains(title)) titles.Add(title);
            }

            TypeList.Clear();
            foreach (var str in types) TypeList.Add(str);

            // Add only news ones
            foreach (var item in GlobalContextList)
            {
                if (!contexts.Contains(item))
                    contexts.Add(item);
            }

            ContextList.Clear();
            ContextList.AddRange(contexts);


            TaskTitleList.Clear();
            foreach (var str in titles) TaskTitleList.Add(str);
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
            var list = new List<ExcelCsvRecord>();
            string line;
            while ((line = stringReader.ReadLine()) != null)
                if (!string.IsNullOrEmpty(line))
                {
                    var excelCsvRecord = new ExcelCsvRecord();
                    if (excelCsvRecord.TryParse(line)) list.Add(excelCsvRecord);
                }

            var selectedTreeTask = SelectedNotebook.SelectedTreeNode;
            if (selectedTreeTask.Context != "Week") return;

            foreach (var record in list)
            {
                var day = record.Day;
                var name = day.DayOfWeek.ToString();
                var dayNode = selectedTreeTask.Nodes.FirstOrDefault(t => t.Name == name);
                if (dayNode == null)
                {
                    var model = new TaskModel
                    {
                        Name = name,
                        DateStarted = day,
                        DateEnded = day
                    };

                    dayNode = new NodeViewModel()
                    {
                        Model = model
                    };
                    selectedTreeTask.Nodes.Add(dayNode);
                }

                var model2 = new TaskModel
                {
                    Name = record.Task,
                    Context = "TaskModel"
                };

                var taskViewModel2 = new NodeViewModel()
                {
                    Model = model2
                };

                var taskViewModel3 = taskViewModel2;
                day = record.Day;
                var year1 = day.Year;
                day = record.Day;
                var month1 = day.Month;
                day = record.Day;
                var day1 = day.Day;
                day = record.Start;
                var hour1 = day.Hour;
                day = record.Start;
                var minute1 = day.Minute;
                day = record.Start;
                var second1 = day.Second;
                var dateTime2 = new DateTime(year1, month1, day1, hour1, minute1, second1);

                taskViewModel3.Model.DateStarted = dateTime2;

                var taskViewModel4 = taskViewModel2;
                day = record.Day;
                var year2 = day.Year;
                day = record.Day;
                var month2 = day.Month;
                day = record.Day;
                var day2 = day.Day;
                day = record.End;
                var hour2 = day.Hour;
                day = record.End;
                var minute2 = day.Minute;
                day = record.End;
                var second2 = day.Second;
                var dateTime3 = new DateTime(year2, month2, day2, hour2, minute2, second2);
                taskViewModel4.Model.DateEnded = dateTime3;
                taskViewModel2.Description = $"{record.Type1}:{record.Type2}:{record.SubTask}";

                dayNode.Nodes.Add(taskViewModel2);
            }
        }

        private async Task OnTreeViewKeyDown(KeyboardKeys keyboardKeys, KeyboardStates keyboardState)
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
            await KeyboardAction(SelectedNotebook.SelectedNode, keyboardKeys, service);
        }

        private async Task CopyTask()
        {
            await OnTreeViewKeyDown(KeyboardKeys.Insert, KeyboardStates.IsControlPressed);
        }

        private async Task ContinueTask()
        {
            await OnTreeViewKeyDown(KeyboardKeys.Insert, KeyboardStates.IsShiftPressed);
        }


        #endregion


        #region Keyboard Actions

        public async Task KeyboardAction(NodeViewModel node, KeyboardKeys keyboardKeys, IActionService service)
        {
            var state = service.GetState();

            // don't show logging ctl, alt, shift or arrow keys
            if (keyboardKeys != KeyboardKeys.None && state != KeyboardStates.None)
                Logger.LogDebug($"KeyboardAction: {keyboardKeys}, {state}");

            switch (keyboardKeys)
            {
                case KeyboardKeys.Insert:
                    await AddNode(node, state, service);
                    break;
                case KeyboardKeys.Delete:
                    node.DeleteNode(service);
                    break;

                case KeyboardKeys.Left:
                    if (state == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = node.Parent;
                        if (parent1 == null)
                            break;
                        var parent2 = parent1.Parent;
                        if (parent2 == null)
                            break;

                        parent1.Remove(node);

                        var num2 = parent2.Nodes.IndexOf(parent1);
                        parent2.Insert(num2 + 1, node);
                        service.SelectItem(node);
                        service.Handled();
                    }

                    break;
                case KeyboardKeys.Right:
                    if (state == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = node.Parent;
                        if (parent1 == null)
                            break;
                        var num2 = parent1.Nodes.IndexOf(node);
                        if (num2 <= 0)
                            break;

                        var parentNode2 = parent1.Nodes[num2 - 1];
                        if (parentNode2 == null)
                            break;

                        parent1.Remove(node);
                        parentNode2.Add(node);
                        service.SelectItem(node);
                        parent1.IsExpanded = true;
                        node.IsSelected = true;
                        service.Handled();
                    }

                    break;
                case KeyboardKeys.Up:
                    if (state == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = node.Parent;
                        if (parent1 == null)
                            break;
                        var num2 = parent1.Nodes.IndexOf(node);
                        if (num2 <= 0)
                            break;
                        parent1.Remove(node);
                        parent1.Insert(num2 - 1, node);
                        service.SelectItem(node);
                        parent1.IsExpanded = true;
                        node.IsSelected = true;
                        service.Handled();
                    }

                    break;
                case KeyboardKeys.Down:
                    if (state == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = node.Parent;
                        if (parent1 == null)
                            break;
                        var num2 = parent1.Nodes.IndexOf(node);
                        if (num2 >= parent1.Nodes.Count - 1)
                            break;
                        parent1.Remove(node);
                        parent1.Insert(num2 + 1, node);
                        service.SelectItem(node);
                        parent1.IsExpanded = true;
                        node.IsSelected = true;
                        service.Handled();
                    }

                    break;
            }
        }


        public async Task AddNode(NodeViewModel node, KeyboardStates state, IActionService service)
        {
            // var node = this;
            NodeViewModel newNode;
            switch (state)
            {
                case KeyboardStates.IsShiftPressed:
                    newNode = await AddNew(node.Parent);
                    break;
                case KeyboardStates.IsControlPressed:
                    var lastSubNode = node.Parent.LastSubNode;
                    newNode = await AddNew(node.Parent);

                    if (lastSubNode != null)
                    {
                        newNode.Name = node.Name;
                    }

                    break;
                default:
                    newNode = await AddNew(node);
                    break;
            }

            node.IsSelected = true;
            service.SelectItem(node);
            service.ExpandItem(node);
            service.Handled();
            if (newNode != null)
            {
                MessengerInstance.Send(new NotificationMessage<NodeViewModel>(newNode, "Add"));
                Logger.LogDebug($"Added [{newNode.Name}] to [{node.Name}]");
            }
        }


        public async Task<NodeViewModel> AddNew(NodeViewModel node)
        {
            var context = RulesHelper.GetSubNodeContext(node.Context);
            if (context.IsNullOrEmpty())
                context = "Node";

            NodeViewModel subNode;
            INode model;

            // Create Model
            if (context == "Task")
            {
                model = new TaskModel
                {
                    DateStarted = DateTime.Now,
                    DateEnded = DateTime.Now
                };
            }
            else
            {
                model = new NodeModel
                {
                    Id = Guid.NewGuid(),
                    Created = DateTime.Now
                };
            }

            model.Id =  Guid.NewGuid();
            model.Context = context;
            var title = RulesHelper.GetSubNodeTitle(node, model);
            if (!string.IsNullOrEmpty(title))
                model.Name = title;
            else
                model.Name = context;


            if (model is TaskModel taskModel)
            {
                SelectedNotebook.Model.Tasks.Add(taskModel);
            }
            else if (model is NodeModel nodeModel)
            {
                SelectedNotebook.Model.Nodes.Add(nodeModel);
            }

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            subNode = new NodeViewModel(model);
            node.Add(subNode);
            return subNode;
        }



        #endregion

    }

}