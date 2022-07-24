using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using ProjectK.Notebook.Data;
using ProjectK.Notebook.Models;
using ProjectK.Notebook.Models.Extensions;
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

namespace ProjectK.Notebook.ViewModels;

public class MainViewModel : ObservableObject
{
    private Dictionary<Guid, TaskViewModel> Tasks { get; } = new();

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
        Logger.LogDebug($"SelectTreeTask2({node.Name})");
        node.TypeList = TypeList;
        node.ContextList = ContextList;
        node.TitleList = TaskTitleList;
        var notebook = FindNotebook(node);
        if (notebook != null) SetSelectedNode(node);

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
        SetSelectedNode(!SelectedNodeList.IsNullOrEmpty() ? SelectedNodeList[0] : task);
        OnPropertyChanged(nameof(SelectedNodeList));
    }

    public void FixTime()
    {
        Logger.LogDebug("FixTime");
        FixTime(SelectedTreeNode);
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

    public async Task ExportSelectedAllAsText()
    {
        if (!(SelectedNode is NodeViewModel node))
            return;

        var (path, ok) = FileHelper.GetNewFileName(Title, "Export", node.Name, ".txt");
        if (!ok)
            return;

        var text = TextReport;
        await File.WriteAllTextAsync(path, text);
        Process.Start("explorer", path);
    }

    public async Task ExportSelectedAllAsJson()
    {
        if (!(SelectedNode is NodeViewModel node))
            return;

        var (path, ok) = FileHelper.GetNewFileName(Title, "Export", node.Name, ".json");
        if (!ok)
            return;

        var notebook = new NotebookModel();
        notebook.ViewModelToModel(node);

        var text = await FileHelper.GetJsonAsync(notebook);

        // await FileHelper.SaveToFileAsync(path, notebook);
        await File.WriteAllTextAsync(path, text);
        Process.Start("explorer", path);
    }


    public NodeViewModel FindTask(Guid id)
    {
        if (!(SelectedNode is NodeViewModel node))
            return null;

        return node.FindNode(id);
    }

    public event EventHandler SelectedDaysChanged;

    public void OnSelectedDaysChanged()
    {
        SelectedDaysChanged?.Invoke(this, EventArgs.Empty);
    }

    private bool TryGetTask2(Guid id, out TaskViewModel task)
    {
        if (Tasks.TryGetValue(id, out task) || task == null)
            return false;

        return true;
    }

    private bool TryGetTask(INode node, out TaskViewModel task)
    {
        task = null;
        if (!(node is TaskModel model))
            return false;

        if (Tasks.TryGetValue(model.Id, out task))
            return true;

        task = new TaskViewModel(model);
        Tasks.Add(task.Id, task);
        return true;
    }

    public void FixTime(NodeViewModel node)
    {
        if (node == null)
            return;

        if (!TryGetTask(node.Model, out var task))
            return;

        if (ModelRules.IsPersonalType(task.Type))
            return;

        if (node.Nodes.IsNullOrEmpty())
        {
            task.Total = task.Duration;
            return;
        }


        for (var index = 0; index < node.Nodes.Count; ++index)
        {
            if (!TryGetTask(node.Nodes[index].Model, out var subTask))
                continue;

            if (subTask.DateEnded == DateTime.MinValue && index < node.Nodes.Count - 1)
            {
                if (!TryGetTask(node.Nodes[index - 1].Model, out var nextTask))
                    continue;

                subTask.DateEnded = nextTask.DateStarted;
            }
        }

        var total = TimeSpan.Zero;
        foreach (var subNode in node.Nodes)
        {
            if (!TryGetTask(subNode.Model, out var subTask))
                continue;

            FixTime(subNode);
            total += subTask.Total;
        }

        task.Total = total;


        if (!TryGetTask(node.Nodes[^1].Model, out var lastTask))
            if (lastTask.DateEnded != DateTime.MinValue)
                lastTask.DateEnded = lastTask.DateEnded;

        if (!TryGetTask(node.Nodes[0].Model, out var firstTask))
            if (firstTask.DateStarted != DateTime.MinValue)
                firstTask.DateStarted = firstTask.DateStarted;
    }

    #region Static

    protected ILogger Logger;

    private static readonly Guid RootGuid = new("98601237-050a-4915-860c-5a820b361910");

    #endregion

    #region Fields

    private readonly Storage _storage = new();
    private ReportTypes _reportType = ReportTypes.Notes;
    private string _excelCsvText;
    private string _textReport;
    private string _title;
    private bool _useTimeOptimization;
    private ItemViewModel _selectedNode;
    private NodeViewModel _selectedTreeNode;

    #endregion

    #region Properties

    public ObservableCollection<FileInfo> MostRecentFiles { get; } = new();
    public ObservableCollection<string> TypeList { get; set; }
    public ObservableCollection<string> ContextList { get; set; }
    public ObservableCollection<string> TaskTitleList { get; set; }
    public ObservableCollection<NodeViewModel> SelectedNodeList { get; } = new();

    public ReportTypes ReportType
    {
        get => _reportType;
        set => SetProperty(ref _reportType, value);
    }

    public Assembly Assembly { get; set; }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string TextReport
    {
        get => _textReport;
        set => SetProperty(ref _textReport, value);
    }

    public Guid LastListTaskId { get; set; }
    public Guid LastTreeTaskId { get; set; }
    public NodeViewModel RootNode { get; set; }

    public NodeViewModel SelectedTreeNode
    {
        get => _selectedTreeNode;
        set => SetProperty(ref _selectedTreeNode, value);
    }

    public ItemViewModel SelectedNode
    {
        get => _selectedNode;
        set => SetProperty(ref _selectedNode, value);
    }

    public string ExcelCsvText
    {
        get => _excelCsvText;
        set => SetProperty(ref _excelCsvText, value);
    }

    public bool UseTimeOptimization
    {
        get => _useTimeOptimization;
        set => SetProperty(ref _useTimeOptimization, value);
    }

    public bool CanSave { get; set; }
    public Action<Action> OnDispatcher { get; set; }
    public OutputViewModel Output { get; set; }

    #endregion

    #region Commands

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
    public ICommand CommandCopyExcelCsvText { get; }

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
        FixTimeCommand = new RelayCommand(FixTime);
        ExtractContextCommand = new RelayCommand(FixContext);
        FixTitlesCommand = new RelayCommand(FixTitles);
        FixTypesCommand = new RelayCommand(FixTypes);
        CopyTaskCommand = new RelayCommand(async () => await CopyTask());
        ContinueTaskCommand = new RelayCommand(async () => await ContinueTask());
        ShowReportCommand = new RelayCommand(OnGenerateReportChanged);
        ExportSelectedAllAsTextCommand = new RelayCommand(async () => await ExportSelectedAllAsText());
        ExportSelectedAllAsJsonCommand = new RelayCommand(async () => await ExportSelectedAllAsJson());
        SyncDatabaseCommand = new RelayCommand(async () => await SyncDatabaseAsync());
        AddNotebookCommand = new RelayCommand(async () => await AddNotebookAsync());
        CommandCopyExcelCsvText = new RelayCommand(CopyExcelCsvText);

        // Add Context 
        ContextList.AddRange(ModelRules.GlobalContextList);
        WeakReferenceMessenger.Default.Register<ValueChangedMessage<NodeViewModel>>(this, (r, m) => { });
        WeakReferenceMessenger.Default.Register<ValueChangedMessage<TaskViewModel>>(this, NotifyMe);
    }


    private void NotifyMe(object recipient, ValueChangedMessage<NodeViewModel> message)
    {
        var notification = recipient;
        var node = message.Value;
        Logger.LogDebug($"Main={node.Name} {notification}");
        switch (notification)
        {
            case "Modified":
                break;
        }
    }

    private void NotifyMe(object recipient, ValueChangedMessage<TaskViewModel> message)
    {
        var notification = recipient;
        var task = message.Value;
        Logger.LogDebug($"Main={task.Name} {notification}");
        switch (notification)
        {
            case "Modified":
                if (task.Model is IItem item)
                {
                    var id = task.Model.Id;
                    var rootId = item.NotebookId;

                    var rootNode = RootNode.FindNode(rootId);
                    var node = RootNode.FindNode(id);
                    node.Modified = ModifiedStatus.Modified;
                    node.SetParentChildModified();
                }

                break;
        }
    }


    public async Task SyncDatabaseAsync()
    {
        await _storage.SaveChangesAsync();
        RootNode.ResetParentChildModified();
        RootNode.ResetModified();
    }

    public void OpenDatabase(string connectionString)
    {
        _storage.OpenDatabase(connectionString);

        // bind to the source
        var notebookModels = _storage.GetNotebooks();

        var models = new List<INode>();
        foreach (var model in notebookModels)
        {
            var (notebook, nodes) = AddNotebook(model);
            if (notebook != null)
                SetSelectedNode(notebook);

            models.AddRange(nodes);
        }

        // ModelToViewModel Sample
        UpdateTypeListAsync(models);
    }


    private void SetSelectedNode(ItemViewModel item)
    {
        ItemViewModel value;
        if (item.Model is TaskModel taskModel)
        {
            if (TryGetTask(taskModel, out var task))
                value = task;
            else
                value = item;
        }
        else
        {
            value = item;
        }

        SelectedNode = value;
    }

    public async Task CloseDatabaseAsync()
    {
        await SyncDatabaseAsync();
        await _storage.SaveChangesAsync();
        await _storage.CloseConnection();
    }

    private async Task AddNotebookAsync()
    {
        Logger.LogDebug("AddNotebook");
        var notebookNames = _storage.GetNotebooks().Select(notebook => notebook.Name).ToList();
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
            var notebook = new NotebookModel { Name = path };
            var tasks = await ImportHelper.ReadFromFileVersionTwo(path);
            await _storage.ImportData(notebook, tasks);
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
        await _storage.Add(notebookModel);
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

    private readonly WorksheetReport _worksheetReport = new();
    private readonly NotesReport _notesReport = new();

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

            var taskViewModel = new NodeViewModel
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

    public async Task KeyboardAction(ItemViewModel item, KeyboardKeys keyboardKeys, IActionService service)
    {
        if (!(item is NodeViewModel node))
            return;

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
                await DeleteNode(node, service);
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

    private async Task DeleteNode(NodeViewModel node, IActionService service)
    {
        node.DeleteNode(service);

        if (node.Context == "Notebook")
        {
            _storage.DeleteNotebook(node.Id);
            var notebook = await _storage.FirstOrDefaultNotebook();
        }
        else
        {
            var models = node.GetModels();
            _storage.RemoveRange(models);
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
        Guid notebookId;
        switch (parentNode.Model)
        {
            case INotebook notebook:
                notebookId = notebook.Id;
                break;
            case IItem item:
                notebookId = item.NotebookId;
                break;
            default:
                return null;
        }


        var model = parentNode.CreateModel(notebookId);
        var node = parentNode.CreateNode(model);
        _storage.AddModel(model);
        try
        {
            await _storage.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return node;
    }

    #endregion
}