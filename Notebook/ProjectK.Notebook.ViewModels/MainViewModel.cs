﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Extensions.Logging;
using ProjectK.Notebook.Domain;
using ProjectK.Notebook.Domain.Reports;
using ProjectK.Notebook.ViewModels.Enums;
using ProjectK.Notebook.ViewModels.Extensions;
using ProjectK.Notebook.ViewModels.Reports;
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

        #endregion

        #region Fields

        private string _excelCsvText;
        private bool _useTimeOptimization;
        private ReportTypes _reportType = ReportTypes.Notes;
        private string _textReport;
        private string _title;
        private NotebookViewModel _selectedNotebook;
        private NodeViewModel _selectedTask;

        #endregion

        #region Consuctors

        private NodeModel _rootModel;

        public MainViewModel()
        {
            _rootModel = new NodeModel
            {
                Context = "Root",
                Created = DateTime.Now,
                Description = "Root Node",
                Name = "Root",
                NodeId = Guid.Empty,
                ParentId = Guid.Empty
            };

            RootTask = new NodeViewModel {Model = _rootModel};


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
            ExportSelectedAllAsTextCommand = new RelayCommand(async () => await this.UserAction_ExportSelectedAllAsText());
            ExportSelectedAllAsJsonCommand = new RelayCommand(async () => await this.UserAction_ExportSelectedAllAsJson());
            OpenDatabaseCommand = new RelayCommand(OpenDatabase);
            CloseDatabaseCommand = new RelayCommand(CloseDatabase);

            CurrentNotebookChanged += OnCurrentNotebookChanged;

        }

        public virtual void CloseDatabase()
        {
            throw new NotImplementedException();
        }

        public virtual void OpenDatabase()
        {
            throw new NotImplementedException();
        }


        private void OnCurrentNotebookChanged()
        {
            var noteBookName = SelectedNotebook != null ? SelectedNotebook.RootTask.Model.Name : "";
            Title = Assembly.GetAssemblyTitle() + " " + Assembly.GetAssemblyVersion() + " - " + noteBookName;
        }

        #endregion

        #region Delegates

        public event EventHandler<TaskEventArgs> SelectedTaskChanged;
        public Action CurrentNotebookChanged { get; set; }

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
        public ICommand CloseDatabaseCommand { get; }



        #endregion

        #region Properties

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
        public ObservableCollection<NotebookViewModel> Notebooks { get; set; } = new ObservableCollection<NotebookViewModel>();


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
        public ObservableCollection<FileInfo> MostRecentFiles { get; } = new ObservableCollection<FileInfo>();
        public ObservableCollection<string> TypeList { get; set; }
        public ObservableCollection<string> ContextList { get; set; }
        public ObservableCollection<string> TaskTitleList { get; set; }
        public bool CanSave { get; set; }
        public Action<Action> OnDispatcher { get; set; }
        public OutputViewModel Output { get; set; } = new OutputViewModel();

        public ObservableCollection<NotebookModel> NotebookModels { get; set; }

        #endregion

        #region Public functions



        public void FileOpenOldFormat()
        {
            //AK SelectedNotebook.LoadFrom(Models.Versions.Version1.NotebookModel.ReadFromFile(Title));
        }


        public async Task UpdateTypeListAsync()
        {
            await Task.Run(() =>
            {
                var nodes = new List<NodeViewModel>();
                nodes.AddToList(RootTask);
                var sortedSet1 = new SortedSet<string>();
                var sortedSet2 = new SortedSet<string>();
                var sortedSet3 = new SortedSet<string>();
                foreach (var node in nodes)
                {
                    if (node.Model is TaskModel task)
                    {
                        var type = task.Type;
                        if (!sortedSet1.Contains(type)) sortedSet1.Add(type);
                    }

                    var context = node.Model.Context;
                    if (!sortedSet2.Contains(context)) sortedSet2.Add(context);

                    var title = node.Model.Name;
                    if (!sortedSet3.Contains(title)) sortedSet3.Add(title);
                }

                TypeList.Clear();
                foreach (var str in sortedSet1) TypeList.Add(str);

                ContextList.Clear();
                foreach (var str in sortedSet2) ContextList.Add(str);

                TaskTitleList.Clear();
                foreach (var str in sortedSet3) TaskTitleList.Add(str);
            });
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
                    TextReport = _notesReport.GenerateReport(SelectedNotebook.SelectedNode);
                    break;
            }
        }

        public void PrepareSettings()
        {
            if (SelectedNotebook.SelectedNode != null)
                LastListTaskId = SelectedNotebook.SelectedNode.Model.NodeId;

            if (SelectedNotebook?.SelectedTreeNode == null) return;

            LastTreeTaskId = SelectedNotebook.SelectedTreeNode.Model.NodeId;
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
            var (ok, notebookNode) = task.FindNode(t => t.Model.Context == "NotebookModel");
            if (!ok)
                return null;

            var path = notebookNode.Model.Name;

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
            if (selectedTreeTask.Model.Context != "Week") return;

            foreach (var excelCsvRecord in excelCsvRecordList)
            {
                var dateTime1 = excelCsvRecord.Day;
                var dayOfTheWeek = dateTime1.DayOfWeek.ToString();
                var taskViewModel1 = selectedTreeTask.Nodes.FirstOrDefault(t => t.Model.Name == dayOfTheWeek);
                if (taskViewModel1 == null)
                {
                    taskViewModel1 = new TaskViewModel()
                    {
                        Model= { Name = dayOfTheWeek,
                        DateStarted = excelCsvRecord.Day,
                        DateEnded = excelCsvRecord.Day
                    }};
                    selectedTreeTask.Nodes.Add(taskViewModel1);
                }

                var taskViewModel2 = new TaskViewModel()
                {
                    Model = { Name = excelCsvRecord.Task, Context = "TaskModel"}
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
                taskViewModel4.Model.DateEnded = dateTime3;
                taskViewModel2.Model.Description = $"{excelCsvRecord.Type1}:{excelCsvRecord.Type2}:{excelCsvRecord.SubTask}";
                taskViewModel1.Nodes.Add(taskViewModel2);
            }
        }

        private void OnTreeViewKeyDown(KeyboardKeys keyboardKeys, KeyboardStates keyboardState)
        {
            SelectedNotebook.SelectedNode?.KeyboardAction(
                keyboardKeys,
                () => keyboardState,
                () => { }, t => t.IsSelected = true,
                t => t.IsExpanded = true,
                () => true,
                OnDispatcher);
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


        public virtual void ImportNotebook(NotebookModel model, Domain.Versions.Version2.DataModel dataModel)
        {
        }
    }

}