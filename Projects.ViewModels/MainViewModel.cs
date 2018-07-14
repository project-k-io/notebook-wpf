﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Projects.Models;
using Projects.Models.Versions.Version2;
using Vibor.Helpers;
using Vibor.Logging;
using Vibor.Mvvm;

namespace Projects.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private static readonly ILog Logger = LogManager.GetLogger(nameof(MainViewModel));

        private string _excelCsvText;
        private string _folder;
        private string _recentFile;
        private string _report;
        private bool _useTimeOptimization;

        public MainViewModel()
        {
            Assembly = Assembly.GetExecutingAssembly();
            CanSave = true;
            TypeList = new ObservableCollection<string>();
            ContextList = new ObservableCollection<string>();
            TaskTitleList = new ObservableCollection<string>();
        }

        public ConfigModel Config { get; set; }

        public Guid LastListTaskId { get; set; }

        public Guid LastTreeTaskId { get; set; }

        public LayoutViewModel Layout { get; } = new LayoutViewModel();

        public ProjectViewModel Project { get; } = new ProjectViewModel();

        public TaskViewModel RootTask => Project.RootTask;

        private Assembly Assembly { get; }

        public string Title => XAttribute.GetAssemblyTitle(Assembly) + " " + XAttribute.GetAssemblyVersion(Assembly) +
                               " - " + Folder;

        public string Folder
        {
            get => _folder;
            set
            {
                if (_folder == value) return;

                _folder = value;
                OnPropertyChanged(nameof(Folder));
                OnPropertyChanged("Title");
            }
        }

        public string RecentFile
        {
            get => _recentFile;
            set
            {
                if (_recentFile == value) return;

                _recentFile = value;
                OnPropertyChanged(nameof(RecentFile));
                OnPropertyChanged("Title");
            }
        }

        public DataModel Data { get; set; }

        public string Report
        {
            get => _report;
            set
            {
                if (_report == value) return;

                _report = value;
                OnPropertyChanged(nameof(Report));
            }
        }

        public string ExcelCsvText
        {
            get => _excelCsvText;
            set
            {
                if (_excelCsvText == value) return;

                _excelCsvText = value;
                OnPropertyChanged(nameof(ExcelCsvText));
            }
        }

        public bool UseTimeOptimization
        {
            get => _useTimeOptimization;
            set
            {
                if (_useTimeOptimization == value) return;

                _useTimeOptimization = value;
                OnPropertyChanged(nameof(UseTimeOptimization));
            }
        }

        private string ConfigPath
        {
            get
            {
                var assemblyCompany = XAttribute.GetAssemblyCompany(Assembly);
                var assemblyProduct = XAttribute.GetAssemblyProduct(Assembly);
                var str = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    assemblyCompany);
                if (!Directory.Exists(str)) Directory.CreateDirectory(str);

                var path = Path.Combine(str, assemblyProduct);
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                return path;
            }
        }

        private string ConfigFile => Path.Combine(ConfigPath, "Config.xml");

        public ObservableCollection<FileInfo> MostRecentFiles { get; } = new ObservableCollection<FileInfo>();

        public ObservableCollection<string> TypeList { get; set; }

        public ObservableCollection<string> ContextList { get; set; }

        public ObservableCollection<string> TaskTitleList { get; set; }

        public bool CanSave { get; set; }

        public void FileOpenOldFormat()
        {
            Project.LoadFrom(Models.Versions.Version1.DataModel.ReadFromFile(RecentFile));
        }

        public async Task FileSaveOldFormatAsync()
        {
            await XFile.SaveToFileAsync(Project, RecentFile);
        }

        public async Task FileOpenNewFormatAsync()
        {
            if (!Directory.Exists(Folder)) return;

            var fileName = DataModel.GetTasksFileName(Folder);
            Data = await XFile.ReadFromFileAsync<DataModel>(fileName);
            Project.LoadFrom(Data);
            UseSettings();
        }

        public async Task FileSaveNewFormatAsync()
        {
            var fileName = DataModel.GetTasksFileName(Folder);
            if (Data == null) Data = new DataModel();

            XFile.SaveOldFile(fileName);
            Project.SaveTo(Data);
            await XFile.SaveToFileAsync(Data, fileName);
        }

        public event EventHandler<EventArgs> GenerateReportChanged;

        public event EventHandler<TaskEventArgs> SelectedTaskChanged;

        public void OnSelectedTaskChanged(TaskViewModel task)
        {
            var selectedTaskChanged = SelectedTaskChanged;
            if (selectedTaskChanged == null) return;

            selectedTaskChanged(this, new TaskEventArgs
            {
                Task = task
            });
        }

        public void OnGenerateReportChanged()
        {
            var generateReportChanged = GenerateReportChanged;
            if (generateReportChanged == null) return;

            generateReportChanged(this, EventArgs.Empty);
        }

        public async Task SaveDataAsync()
        {
            if (!CanSave) return;

            await SaveConfigurationAsync();
            await FileSaveNewFormatAsync();
        }

        public async Task LoadDataAsync()
        {
            await LoadConfigurationAsync();
            await FileOpenNewFormatAsync();
        }

        public async Task UpdateTypeListAsync()
        {
            await Task.Run(() =>
            {
                var taskViewModelList = new List<TaskViewModel>();
                XTask.AddToList(taskViewModelList, RootTask);
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

        public async Task LoadConfigurationAsync()
        {
            Config = await XFile.ReadFromFileAsync<ConfigModel>(ConfigFile);
            if (Config == null) Config = new ConfigModel();

            LastListTaskId = Config.App.LastListTaskId;
            LastTreeTaskId = Config.App.LastTreeTaskId;
            Folder = Config.App.RecentFolder;
            RecentFile = Config.App.RecentFile;
            MostRecentFiles.Clear();
            if (!Directory.Exists(Folder)) return;

            MostRecentFiles.Add(new FileInfo(Folder));
        }

        public async Task SaveConfigurationAsync()
        {
            if (Config == null) Config = new ConfigModel();

            Config.App.RecentFolder = Folder;
            Config.App.LastListTaskId = LastListTaskId;
            Config.App.LastTreeTaskId = LastTreeTaskId;
            Config.App.RecentFile = RecentFile;
            await XFile.SaveToFileAsync(Config, ConfigFile);
        }

        public void UseSettings()
        {
            Project.SelectTreeTask(LastListTaskId);
            Project.SelectTask(LastTreeTaskId);
        }

        public void PrepareSettings()
        {
            if (Project.SelectedTask != null) LastListTaskId = Project.SelectedTask.Id;

            if (Project.SelectedTreeTask == null) return;

            LastTreeTaskId = Project.SelectedTreeTask.Id;
        }

        public void SelectTreeTask(TaskViewModel task)
        {
            task.TypeList = TypeList;
            task.ContextList = ContextList;
            task.TaskTitleList = TaskTitleList;
            Project.SelectTreeTask(task);
            OnGenerateReportChanged();
        }

        public void CopyExcelCsvText()
        {
            var stringReader = new StringReader(ExcelCsvText);
            var excelCsvRecordList = new List<ExcelCsvRecord>();
            string line;
            while ((line = stringReader.ReadLine()) != null)
                if (!string.IsNullOrEmpty(line))
                {
                    var excelCsvRecord = new ExcelCsvRecord();
                    if (excelCsvRecord.TryParse(line)) excelCsvRecordList.Add(excelCsvRecord);
                }

            var selectedTreeTask = Project.SelectedTreeTask;
            if (selectedTreeTask.Context != "Week") return;

            foreach (var excelCsvRecord in excelCsvRecordList)
            {
                var dateTime1 = excelCsvRecord.Day;
                var dayOfTheWeek = dateTime1.DayOfWeek.ToString();
                var taskViewModel1 = selectedTreeTask.SubTasks.FirstOrDefault(t => t.Title == dayOfTheWeek);
                if (taskViewModel1 == null)
                {
                    taskViewModel1 = new TaskViewModel(dayOfTheWeek, 0);
                    taskViewModel1.DateStarted = excelCsvRecord.Day;
                    taskViewModel1.DateEnded = excelCsvRecord.Day;
                    selectedTreeTask.SubTasks.Add(taskViewModel1);
                }

                var taskViewModel2 = new TaskViewModel(excelCsvRecord.Task, 0);
                taskViewModel2.Context = "Task";
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
                taskViewModel2.Description = string.Format("{0}:{1}:{2}", excelCsvRecord.Type1, excelCsvRecord.Type2,
                    excelCsvRecord.SubTask);
                taskViewModel1.SubTasks.Add(taskViewModel2);
            }
        }

        public async Task NewProjectAsync()
        {
            await SaveDataAsync();
            CanSave = false;
            Project.Clear();
            Project.RootTask.Add(new TaskViewModel("Time Tracker", 1)
            {
                Context = "Time Tracker"
            });
            Data = new DataModel();
            Folder = string.Empty;
            CanSave = true;
        }

        public void OnTreeViewKeyDown(TaskViewModel.KeyStates keyState, TaskViewModel.KeyboardStates keyboardState)
        {
            TaskViewModel.OnTreeViewKeyDown(
                Project.SelectedTask,
                keyState,
                () => keyboardState,
                () => { }, t => t.IsSelected = true,
                t => t.IsExpanded = true,
                () => true,
                OnDispatcher);
        }

        public void CopyTask()
        {
            OnTreeViewKeyDown(TaskViewModel.KeyStates.Insert, TaskViewModel.KeyboardStates.IsControlPressed);
        }

        public void ContinueTask()
        {
            OnTreeViewKeyDown(TaskViewModel.KeyStates.Insert, TaskViewModel.KeyboardStates.IsShiftPressed);
        }
    }
}