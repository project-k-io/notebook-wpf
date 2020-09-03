﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Models;
using ProjectK.Notebook.Models.Versions.Version2;
using ProjectK.Notebook.ViewModels.Enums;
using ProjectK.Notebook.ViewModels.Extensions;
using ProjectK.Notebook.ViewModels.Reports;
using ProjectK.Utils;
using ProjectK.Utils.Extensions;
using ProjectK.ViewModels;

namespace ProjectK.Notebook.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Static

        private static readonly ILogger Logger = LogManager.GetLogger<MainViewModel>();

        #endregion

        #region Fields

        private string _excelCsvText;
        private bool _useTimeOptimization;
        private ReportTypes _reportType = ReportTypes.Notes;
        public Action<LogLevel, EventId, string> LogEvent { get; set; }
        public Action CurrentNotebookChanged { get; set; }


        #endregion

        #region Consuctors

        public MainViewModel()
        {
            CanSave = true;
            TypeList = new ObservableCollection<string>();
            ContextList = new ObservableCollection<string>();
            TaskTitleList = new ObservableCollection<string>();
#if AK
            ClearCommand = new RelayCommand(Notebook.Clear);
            EditCommand = new RelayCommand(() => Process.Start("explorer", Notebook.DataFile));
            FixTimeCommand = new RelayCommand(Notebook.FixTime);
            ExtractContextCommand = new RelayCommand(Notebook.FixContext);
            FixTitlesCommand = new RelayCommand(Notebook.FixTitles);
            FixTypesCommand = new RelayCommand(Notebook.FixTypes);
#endif
            CopyTaskCommand = new RelayCommand(CopyTask);
            ContinueTaskCommand = new RelayCommand(ContinueTask);
            ShowReportCommand = new RelayCommand<ReportTypes>(ShowReport);
            ExportSelectedAllAsTextCommand = new RelayCommand(async () => await ExportSelectedAllAsText());
            ExportSelectedAllAsJsonCommand = new RelayCommand(async () => await ExportSelectedAllAsJson());
            ImportToSelectedAsJsonCommand = new RelayCommand(async () => await ImportToSelectedAsJson());
            CurrentNotebookChanged += OnCurrentNotebookChanged;

        }

        private void OnCurrentNotebookChanged()
        {
            var noteBookName = SelectedNotebook != null ? SelectedNotebook.RootTask.Title : "";
            Title = Assembly.GetAssemblyTitle() + " " + Assembly.GetAssemblyVersion() + " - " + noteBookName;
        }

        #endregion

        #region Events

        public event EventHandler<TaskEventArgs> SelectedTaskChanged;

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
        public ICommand ImportToSelectedAsJsonCommand { get; }

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

        private string _title;
        public string Title { 
            get => _title;
            set
            {
                if(_title == null)
                    return;

                _title = value;
                RaisePropertyChanged();
            }
        }


        public Guid LastListTaskId { get; set; }
        public Guid LastTreeTaskId { get; set; }
        public NotebookViewModel SelectedNotebook { get; set; }
        public ObservableCollection<NotebookViewModel> Notebooks { get; } = new ObservableCollection<NotebookViewModel>();
        public TaskViewModel RootTask { get; set; } = new TaskViewModel {Title = "Root"};
        public TaskViewModel SelectedTask { get; set; }



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

#endregion

#region Public functions



        public void FileOpenOldFormat()
        {
            //AK Notebook.LoadFrom(Models.Versions.Version1.DataModel.ReadFromFile(DataFile));
        }


        public async Task SaveFileAsync()
        {
            Logger.LogDebug("SaveFileAsync");
            await SaveFileAsync((a, b) => false);
        }

        private async Task ExportSelectedAllAsText()
        {
            if(SelectedNotebook == null)
                return;

            await SelectedNotebook.ExportSelectedAllAsText();
        }

        private async Task ExportSelectedAllAsJson()
        {
            if (SelectedNotebook == null)
                return;

            await SelectedNotebook.ExportSelectedAllAsJson();
        }

        private async Task ImportToSelectedAsJson()
        {
            await SelectedTask.ImportToSelectedAsJson();
        }


        public async Task SaveModifiedFileAsync()
        {
            await SaveFileAsync((a, b) => a.IsSame(b));
        }

        public async Task UpdateTypeListAsync()
        {
            await Task.Run(() =>
            {
                var taskViewModelList = new List<TaskViewModel>();
                taskViewModelList.AddToList(RootTask);
                var sortedSet1 = new SortedSet<string>();
                var sortedSet2 = new SortedSet<string>();
                var sortedSet3 = new SortedSet<string>();
                foreach (var taskViewModel in taskViewModelList)
                {
                    var type = taskViewModel.Type;
                    if (!sortedSet1.Contains(type)) sortedSet1.Add(type);

                    var context = taskViewModel.Context;
                    if (!sortedSet2.Contains(context)) sortedSet2.Add(context);

                    var title = taskViewModel.Title;
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

        public async Task UserNewFileAsync()
        {
            Logger.LogDebug("UserNewFileAsync");
            await SaveFileAsync(); //New
            CanSave = false;

            var notebook = new NotebookViewModel();
            notebook.RootTask.Add(new TaskViewModel("Time Tracker")
            {
                Context = "Time Tracker"
            });

            notebook._data = new DataModel();
            var path = FileHelper.MakeUnique(notebook.DataFile);
            notebook.DataFile = path;

            Notebooks.Add(notebook);
            RootTask.Add(notebook.RootTask);
            SelectedNotebook = notebook;
            CanSave = true;
        }




        public void OnSelectedTaskChanged(TaskViewModel task)
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
                    _notesReport.GenerateReport(this);
                    break;
            }
        }

        public void PrepareSettings()
        {
            if (SelectedTask != null) 
                LastListTaskId = SelectedTask.Id;

            if (SelectedNotebook?.SelectedTreeTask == null) return;

            LastTreeTaskId = SelectedNotebook.SelectedTreeTask.Id;
        }

        public void SelectTreeTask(TaskViewModel task)
        {
            task.TypeList = TypeList;
            task.ContextList = ContextList;
            task.TaskTitleList = TaskTitleList;
            SelectedNotebook?.SelectTreeTask(task);
            OnGenerateReportChanged();
        }

#endregion

#region Private functions

        // Save File and Log
        private async Task SaveFileAsync(TaskViewModel rootTask, Func<DataModel, DataModel, bool> isSame)
        {
            if (SelectedNotebook == null)
                return;

            await SelectedNotebook.SaveFileAsync(isSame);

        }

        // Save File and Log
        private async Task SaveFileAsync(Func<DataModel, DataModel, bool> isSame)
        {
            if(SelectedNotebook == null)
                return;

            await SaveFileAsync(SelectedNotebook.RootTask, isSame);
        }

        private async Task FileSaveOldFormatAsync()
        {
            if (SelectedNotebook == null)
                return;

            await FileHelper.SaveToFileAsync(SelectedNotebook, SelectedNotebook.DataFile);
        }

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

            var selectedTreeTask = SelectedNotebook.SelectedTreeTask;
            if (selectedTreeTask.Context != "Week") return;

            foreach (var excelCsvRecord in excelCsvRecordList)
            {
                var dateTime1 = excelCsvRecord.Day;
                var dayOfTheWeek = dateTime1.DayOfWeek.ToString();
                var taskViewModel1 = selectedTreeTask.SubTasks.FirstOrDefault(t => t.Title == dayOfTheWeek);
                if (taskViewModel1 == null)
                {
                    taskViewModel1 = new TaskViewModel(dayOfTheWeek)
                    {
                        DateStarted = excelCsvRecord.Day,
                        DateEnded = excelCsvRecord.Day
                    };
                    selectedTreeTask.SubTasks.Add(taskViewModel1);
                }

                var taskViewModel2 = new TaskViewModel(excelCsvRecord.Task) {Context = "Task"};
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
                taskViewModel1.SubTasks.Add(taskViewModel2);
            }
        }

        private void OnTreeViewKeyDown(KeyboardKeys keyboardKeys, KeyboardStates keyboardState)
        {
            SelectedTask?.KeyboardAction(
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

        private void ShowReport(ReportTypes reportType)
        {
            Logger.LogDebug($"Show Report: {reportType}");
            OnGenerateReportChanged();
        }

#endregion
    }
}