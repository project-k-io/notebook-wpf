using System;
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
using ProjectK.Utils;
using ProjectK.ViewModels;

namespace ProjectK.Notebook.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private static readonly ILogger Logger = LogManager.GetLogger<MainViewModel>();

        #region Fields

        private string _excelCsvText;
        
        private string _dataFolder = "";
        private string _dataFile = "";

        private string _report;
        private bool _useTimeOptimization;

        #endregion

        #region Events

        public event EventHandler<EventArgs> GenerateReportChanged;
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

        #endregion

        #region Properties

        public Guid LastListTaskId { get; set; }
        public Guid LastTreeTaskId { get; set; }
        public LayoutViewModel Layout { get; } = new LayoutViewModel();
        public ProjectViewModel Project { get; } = new ProjectViewModel();
        public TaskViewModel RootTask => Project.RootTask;
        private Assembly Assembly { get; }
        public string Title => XAttribute.GetAssemblyTitle(Assembly) + " " + XAttribute.GetAssemblyVersion(Assembly) + " - " + DataFile;

        public string DataFile
        {
            get => _dataFile;
            set
            {
                if (!Set(ref _dataFile, value)) return;
                RaisePropertyChanged("Title");
            }
        }

        public DataModel Data { get; set; }
        public string Report
        {
            get => _report;
            set => Set(ref _report, value);
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
        public bool CanSave { get; set; } = false;
        public Action<Action> OnDispatcher { get; set; }
        public OutputViewModel Output { get; set; } = new OutputViewModel();

        #endregion


        public MainViewModel()
        {
            Assembly = Assembly.GetExecutingAssembly();
            CanSave = true;
            TypeList = new ObservableCollection<string>();
            ContextList = new ObservableCollection<string>();
            TaskTitleList = new ObservableCollection<string>();

            ClearCommand = new RelayCommand(Project.Clear);
            EditCommand = new RelayCommand(() => Process.Start("notepad.exe", DataFile));
            FixTimeCommand = new RelayCommand(Project.FixTime);
            ExtractContextCommand = new RelayCommand(Project.FixContext);
            FixTitlesCommand = new RelayCommand(Project.FixTitles);
            FixTypesCommand = new RelayCommand(Project.FixTypes);
            CopyTaskCommand = new RelayCommand(CopyTask);
            ContinueTaskCommand = new RelayCommand(ContinueTask);
        }


        public void FileOpenOldFormat()
        {
            Project.LoadFrom(Models.Versions.Version1.DataModel.ReadFromFile(DataFile));
        }
        public async Task FileSaveOldFormatAsync()
        {
            await XFile.SaveToFileAsync(Project, DataFile);
        }

        public async Task OpenFileNewFormatAsync()
        {
            Logger.LogDebug("OpenFileNewFormatAsync");
            var path = DataFile;
            if (!File.Exists(path)) return;
            Data = await XFile.ReadFromFileAsync<DataModel>(path);
            Project.LoadFrom(Data);
            UseSettings();
        }

        public async Task FileSaveNewFormatAsync()
        {
            if (Data == null) 
                Data = new DataModel();

            var path = DataFile;
            XFile.SaveOldFile(path);
            Project.SaveTo(Data);

            await XFile.SaveToFileAsync(Data, path);
        }
        public async Task SaveDataAsync()
        {
            // Todo" AK !!! Temp
            return;

            Logger.LogDebug("SaveDataAsync");
            if (!CanSave) return;
            await FileSaveNewFormatAsync();
        }
        public async Task LoadDataAsync()
        {
            await OpenFileNewFormatAsync();
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
        public async Task NewProjectAsync()
        {
            Logger.LogDebug("NewProjectAsync");
            await SaveDataAsync();
            CanSave = false;
            Project.Clear();
            Project.RootTask.Add(new TaskViewModel("Time Tracker", 1)
            {
                Context = "Time Tracker"
            });

            Data = new DataModel();
            var path = XFile2.MakeUnique(DataFile);
            var name = Path.GetFileName(path);
            DataFile = name;
            CanSave = true;
        }

        public void OnSelectedTaskChanged(TaskViewModel task)
        {
            SelectedTaskChanged?.Invoke(this, new TaskEventArgs
            {
                Task = task
            });
        }
        public void OnGenerateReportChanged()
        {
            GenerateReportChanged?.Invoke(this, EventArgs.Empty);
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
                    taskViewModel1 = new TaskViewModel(dayOfTheWeek, 0)
                    {
                        DateStarted = excelCsvRecord.Day,
                        DateEnded = excelCsvRecord.Day
                    };
                    selectedTreeTask.SubTasks.Add(taskViewModel1);
                }

                var taskViewModel2 = new TaskViewModel(excelCsvRecord.Task, 0) { Context = "Task" };
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