﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using ProjectK.Notebook.Data;
using ProjectK.Notebook.Models;
using ProjectK.Notebook.Models.Interfaces;
using ProjectK.Notebook.Models.Reports;
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

        private static readonly Guid RootGuid = new Guid("98601237-050a-4915-860c-5a820b361910");

        #endregion

        #region Fields

        private readonly Storage _db = new Storage();
        private ReportTypes _reportType = ReportTypes.Notes;
        private string _excelCsvText;
        private string _textReport;
        private string _title;
        private bool _useTimeOptimization;
        private NodeViewModel _selectedNode;
        private NodeViewModel _selectedTreeNode;

        #endregion

        #region Properties

        public ObservableCollection<FileInfo> MostRecentFiles { get; } = new ObservableCollection<FileInfo>();
        public ObservableCollection<string> TypeList { get; set; }
        public ObservableCollection<string> ContextList { get; set; }
        public ObservableCollection<string> TaskTitleList { get; set; }
        public ObservableCollection<NodeViewModel> SelectedNodeList { get; } = new ObservableCollection<NodeViewModel>();
        public ReportTypes ReportType
        {
            get => _reportType;
            set => Set(ref _reportType, value);
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
        public NodeViewModel RootNode { get; set; }
        public NodeViewModel SelectedTreeNode
        {
            get => _selectedTreeNode;
            set => Set(ref _selectedTreeNode, value);
        }
        public NodeViewModel SelectedNode
        {
            get => _selectedNode;
            set => Set(ref _selectedNode, value);
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
        public OutputViewModel Output { get; set; }

        #endregion

        #region Commands

        public ICommand ClearCommand { get; }
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
            var rootModel = new NodeModel
            {
                Id = RootGuid,
                ParentId = Guid.Empty,
                Context = "Root",
                Created = DateTime.Now,
                Name = "Root"
            };

            RootNode = new NodeViewModel(rootModel);
            CanSave = true;
            TypeList = new ObservableCollection<string>();
            ContextList = new ObservableCollection<string>();
            TaskTitleList = new ObservableCollection<string>();
            ClearCommand = new RelayCommand(this.UserAction_Clear);
            FixTimeCommand = new RelayCommand(FixTime);
            ExtractContextCommand = new RelayCommand(FixContext);
            FixTitlesCommand = new RelayCommand(FixTitles);
            FixTypesCommand = new RelayCommand(FixTypes);
            CopyTaskCommand = new RelayCommand(async () => await CopyTask());
            ContinueTaskCommand = new RelayCommand(async () => await ContinueTask());
            ShowReportCommand = new RelayCommand<ReportTypes>(this.UserAction_ShowReport);
            ExportSelectedAllAsTextCommand = new RelayCommand(async () => await this.UserAction_ExportSelectedAllAsText());
            ExportSelectedAllAsJsonCommand = new RelayCommand(async () => await this.UserAction_ExportSelectedAllAsJson());
            SyncDatabaseCommand = new RelayCommand(async () => await SyncDatabaseAsync());
            AddNotebookCommand = new RelayCommand(async () => await AddNotebookAsync());

            // Add Context 
            ContextList.AddRange(ModelRules.GlobalContextList);

            MessengerInstance.Register<NotificationMessage<NodeViewModel>>(this, NotifyMe);
        }


        private void NotifyMe(NotificationMessage<NodeViewModel> notificationMessage)
        {
            var notification = notificationMessage.Notification;
            var node = notificationMessage.Content;
            Logger.LogDebug($"Main={node.Name} {notification}");
            switch (notification)
            {
                case "Modified":
                    break;
            }
        }


        public async Task SyncDatabaseAsync()
        {
            await _db.SaveChangesAsync();
            RootNode.ResetParentChildModified();
            RootNode.ResetModified();
        }

        public void OpenDatabase(string connectionString)
        {
            _db.OpenDatabase(connectionString);

            // bind to the source
            var notebookModels = _db.GetNotebooks();

            var models = new List<INode>();
            foreach (var model in notebookModels)
            {
                var (notebook, nodes) = AddNotebook(model);
                if (notebook != null)
                    SelectedNode = notebook;

                models.AddRange(nodes);
            }

            // ModelToViewModel Data
            UpdateTypeListAsync(models);
        }

        public async Task CloseDatabaseAsync()
        {
            await SyncDatabaseAsync();
            await _db.SaveChangesAsync();
            await _db.CloseConnection();
        }

        private async Task AddNotebookAsync()
        {
            Logger.LogDebug("AddNotebook");
            var notebookNames =  _db.GetNotebooks().Select(notebook => notebook.Name).ToList();
            var notebookName = StringHelper.GetUniqueName("Notebook", notebookNames);

            // Create Notebook
            var model = new NotebookModel
            {
                Name = notebookName,
                Id = Guid.NewGuid(),
                Context = "Notebook",
                Created = DateTime.Now
            };
            await ImportNotebook(model);
            await SyncDatabaseAsync();
        }


        public async Task OpenFileAsync(string path)
        {
            try
            {
                Logger.LogDebug($"OpenFileAsync | {Path.GetDirectoryName(path)} | {Path.GetFileName(path)} ");
                var notebook = new NotebookModel {Name = path};
                var tasks = await ImportHelper.ReadFromFileVersionTwo(path);
                await _db.ImportData(notebook, tasks);
                await ImportNotebook(notebook);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }
        }


        public async Task ImportNotebook(NotebookModel notebookModel)
        {
            Logger.LogDebug($"Import NotebookModel: {notebookModel.Name}");

            // Add NotebookModel
            await _db.Add(notebookModel);
            // this will create Primary Key for notebook
            var (notebook, nodes) = AddNotebook(notebookModel);
            if (notebook != null)
                SelectedTreeNode = notebook;

            UpdateTypeListAsync(nodes);
        }


        private (NodeViewModel, List<INode>) AddNotebook(NotebookModel model)
        {
            Logger.LogDebug($"AddNotebook: {model.Name}");

            var items = model.GetItems();
            if (model.NonRoot)
            {
                RootNode.BuildTree(items);
                return (null, items);
            }

            var notebook = new NodeViewModel(model);
            notebook.BuildTree(items);
            RootNode.Add(notebook);
            return (notebook, items);
        }
        public void SetTitle(string recentFile)
        {
            var product = Assembly.GetAssemblyTitle();
            var version = Assembly.GetAssemblyVersion();
            var file = recentFile;
            Title = $"{product} {version} - {file}";
        }

        #endregion

        #region Delegates

        public event EventHandler<TaskEventArgs> SelectedTaskChanged;
        public Action CurrentNotebookChanged { get; set; }

        #endregion

        #region Public functions

        public void UpdateTypeListAsync(List<INode> nodes)
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

            ContextList.Clear();
            ContextList.AddRange(ModelRules.GlobalContextList);


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
                    WorksheetReport.GenerateReport(this);
                    break;
                case ReportTypes.Notes:
                    TextReport = _notesReport.GenerateReport(SelectedNode);
                    break;
            }
        }

        public void PrepareSettings()
        {
            if (SelectedNode != null)
                LastListTaskId = SelectedNode.Id;

            if (SelectedTreeNode == null) return;

            LastTreeTaskId = SelectedTreeNode.Id;
        }



        public NodeViewModel FindNotebook(NodeViewModel task)
        {
            var (ok, notebook) = task.FindNode(t => t.Context == "Notebook");
            if (!ok)
                return null;

            return notebook;
        }

        #endregion

        #region Private functions

        private void CopyExcelCsvText()
        {
            var stringReader = new StringReader(ExcelCsvText);
            var list = new List<ExcelCsvRecord>();

            string line;
            while ((line = stringReader.ReadLine()) != null)
                if (!string.IsNullOrEmpty(line))
                {
                    var excelCsvRecord = new ExcelCsvRecord();
                    if (excelCsvRecord.TryParse(line)) 
                        list.Add(excelCsvRecord);
                }

            var selectedTreeTask = SelectedTreeNode;
            if (selectedTreeTask.Context != "Week") return;

            foreach (var record in list)
            {
                var day = record.Day;
                var dayOfWeek = day.DayOfWeek.ToString();
                var dayNode = selectedTreeTask.Nodes.FirstOrDefault(t => t.Name == dayOfWeek);
                if (dayNode == null)
                {
                    var model = new TaskModel // CopyExcelCsvText
                    {
                        Name = dayOfWeek,
                        DateStarted = day,
                        DateEnded = day
                    };

                    dayNode = new NodeViewModel
                    {
                        Model = model
                    };
                    selectedTreeTask.Nodes.Add(dayNode);
                }

                var taskModel = new TaskModel // CopyExcelCsvText
                {
                    Name = record.Task,
                    Context = "TaskModel"
                };

                var taskViewModel = new NodeViewModel()
                {
                    Model = taskModel
                };

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

                taskModel.DateStarted = dateTime2;

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


                taskModel.DateEnded = dateTime3;
                taskViewModel.Description = $"{record.Type1}:{record.Type2}:{record.SubTask}";
                 
                dayNode.Nodes.Add(taskViewModel);
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
            await KeyboardAction(SelectedNode, keyboardKeys, service);
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
                    DeleteNode(node, service);
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

        private void DeleteNode(NodeViewModel node, IActionService service)
        {
            node.DeleteNode(service);

            if (node.Context == "Notebook")
            {
#if AK
                var notebook = Notebooks.First(n => n.Id == node.Id);
                if (notebook == null)
                    return;

                Notebooks.Remove(notebook);

                // Selected Notebook
                if (Notebooks.Count == 0)
                {
                    SelectedNotebook = null;
                }
                else
                {
                    if (notebook.Main.Id == SelectedNotebook.Main.Id) SelectedNotebook = Notebooks.FirstOrDefault();
                }

                _db.Remove(notebook.Main as NotebookModel);
#endif
            }
            else
            {
                var models = node.GetModels();
                _db.RemoveRange(models);
            }
        }

        public async Task<NodeViewModel> AddNode(NodeViewModel node, KeyboardStates state, IActionService service)
        {
            NodeViewModel newNode;
            switch (state)
            {
                case KeyboardStates.IsShiftPressed:
                    newNode = await AddNew(node.Parent);
                    break;
                case KeyboardStates.IsControlPressed:
                    var lastSubNode = node.Parent.LastSubNode;
                    newNode = await AddNew(node.Parent);

                    if (lastSubNode != null) newNode.Name = node.Name;

                    break;
                default:
                    newNode = await AddNew(node);
                    break;
            }

            node.IsSelected = true;
            service.SelectItem(node);
            service.ExpandItem(node);
            service.Handled();
            return newNode;
        }

        public async Task<NodeViewModel> AddNew(NodeViewModel parentNode)
        {
#if AK
            if (!(SelectedNotebook.Main is NotebookModel notebook))
                return null;
            var notebookId = notebook.Id;
            var model = parentNode.CreateModel(notebookId);
            var node = parentNode.CreateNode(model);
            _db.AddModel(model);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return node;
#else
            return null;
#endif
        }

        #endregion

        public List<DateTime> GetSelectedDays()
        {
            var dateTimeList = new List<DateTime>();
            foreach (var selectedNode in SelectedNodeList)
                if (selectedNode.Context == "Day")
                    if (selectedNode.Model is TaskModel selectedTask)
                        dateTimeList.Add(selectedTask.DateStarted);

            return dateTimeList;
        }
        public void SelectTreeTask2(NodeViewModel node)
        {
            node.TypeList = TypeList;
            node.ContextList = ContextList;
            node.TitleList = TaskTitleList;
            var notebook = FindNotebook(node);
            if (notebook != null)
            {
                SelectedNode = node;
                Logger.LogDebug($"SelectedNotebook selected | {notebook.Title}");
            }

            SelectTreeTask(node);
            OnGenerateReportChanged();
        }
        public void SelectTreeTask(NodeViewModel task)
        {
            if (task == null)
                return;

            SelectedTreeNode = task;
            SelectedNodeList.Clear();
            SelectedNodeList.AddToList(task);
            OnSelectedDaysChanged();
            SelectedNode = !SelectedNodeList.IsNullOrEmpty() ? SelectedNodeList[0] : task;
            RaisePropertyChanged(nameof(SelectedNodeList));
        }
        public void FixTime()
        {
            if (SelectedTreeNode.Model is TaskModel task)
                SelectedTreeNode.FixTime();
        }
        public void ExtractContext()
        {
            ContextList.Clear();
            SelectedNode.ExtractContext(ContextList);
            RaisePropertyChanged("ContextList");
        }
        private void FixContext()
        {
            SelectedTreeNode.FixContext();
        }
        public void FixTitles()
        {
            SelectedTreeNode.FixTitles();
        }
        public void FixTypes()
        {
            SelectedTreeNode.FixTypes();
        }
        public static void AddToList22(ICollection<NodeViewModel> list, NodeViewModel node, IList dates)
        {
            if (node.Model is TaskModel task)
                if (ModelRules.ContainDate(dates, task.DateStarted))
                    list.Add(node);

            foreach (var subTask in node.Nodes)
                AddToList22(list, subTask, dates);
        }
        public void UpdateSelectDayTasks(IList dates)
        {
            SelectedNodeList.Clear();
#if AK
            AddToList2(SelectedNodeList, RootNode, dates);
#endif
        }
        public async Task ExportSelectedAllAsText(string text)
        {
            var path = Title;
            var name = SelectedNode.Name;
            var (exportPath, ok) = FileHelper.GetNewFileName(path, "Export", name, ".txt");
            if (!ok)
                return;

            await File.WriteAllTextAsync(exportPath, text);
        }
        public async Task ExportSelectedAllAsJson()
        {
            var path = Title;
            var (exportPath, ok) = FileHelper.GetNewFileName(path, "Export", SelectedNode.Name);
            if (!ok)
                return;

            await SelectedNode.ExportToFileAsync(exportPath);
        }
        public NodeViewModel FindTask(Guid id)
        {
            return SelectedNode.FindNode(id);
        }
        public event EventHandler SelectedDaysChanged;
        public void OnSelectedDaysChanged()
        {
            SelectedDaysChanged?.Invoke(this, EventArgs.Empty);
        }

    }
}