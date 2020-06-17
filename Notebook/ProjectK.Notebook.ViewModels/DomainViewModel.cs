using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using ProjectK.Utils;
using ProjectK.Utils.Extensions;
using ProjectK.ViewModels;

namespace ProjectK.Notebook.ViewModels
{
    public class DomainViewModel : ViewModelBase
    {
        private static readonly ILogger Logger = LogManager.GetLogger<DomainViewModel>();


        public DomainViewModel()
        {
            CanSave = true;
            TypeList = new ObservableCollection<string>();
            ContextList = new ObservableCollection<string>();
            TaskTitleList = new ObservableCollection<string>();

            ClearCommand = new RelayCommand(Notebook.Clear);
            EditCommand = new RelayCommand(() => Process.Start("explorer", DataFile));
            FixTimeCommand = new RelayCommand(Notebook.FixTime);
            ExtractContextCommand = new RelayCommand(Notebook.FixContext);
            FixTitlesCommand = new RelayCommand(Notebook.FixTitles);
            FixTypesCommand = new RelayCommand(Notebook.FixTypes);
            CopyTaskCommand = new RelayCommand(CopyTask);
            ContinueTaskCommand = new RelayCommand(ContinueTask);
        }


        public void FileOpenOldFormat()
        {
            //AK Notebook.LoadFrom(Models.Versions.Version1.DataModel.ReadFromFile(DataFile));
        }

        public async Task FileSaveOldFormatAsync()
        {
            await FileHelper.SaveToFileAsync(Notebook, DataFile);
        }


        public async Task OpenFileAsync(string path)
        {
            DataFile = path;
            Logger.LogDebug($"OpenFileAsync : {path}");
            _data = await FileHelper.ReadFromFileAsync<DataModel>(path);
            Notebook.LoadFrom(_data?.Copy());
            UseSettings();
        }

        public async Task SaveFileAsync()
        {
            Logger.LogDebug("SaveFileAsync");
            await SaveFileAsync((a, b) => false);
        }

        public async Task SaveModifiedFileAsync()
        {
            await SaveFileAsync((a, b) => a.IsSame(b));
        }

        private async Task SaveFileAsync(Func<DataModel, DataModel, bool> isSame)
        {
            var newData = new DataModel();
            Notebook.SaveTo(newData);

            _data ??= new DataModel();
            if (isSame(_data, newData))
                return;

            var path = DataFile;
            FileHelper.SaveFileToLog(path);

            _data.Copy(newData);
            await FileHelper.SaveToFileAsync(_data, path);
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
            Notebook.Clear();
            Notebook.RootTask.Add(new TaskViewModel("Time Tracker")
            {
                Context = "Time Tracker"
            });

            _data = new DataModel();
            var path = FileHelper.MakeUnique(DataFile);
            DataFile = path;
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
            this.GenerateReport(Logger);
        }

        public void UseSettings()
        {
            Notebook.SelectTreeTask(LastListTaskId);
            Notebook.SelectTask(LastTreeTaskId);
        }

        public void PrepareSettings()
        {
            if (Notebook.SelectedTask != null) LastListTaskId = Notebook.SelectedTask.Id;

            if (Notebook.SelectedTreeTask == null) return;

            LastTreeTaskId = Notebook.SelectedTreeTask.Id;
        }

        public void SelectTreeTask(TaskViewModel task)
        {
            task.TypeList = TypeList;
            task.ContextList = ContextList;
            task.TaskTitleList = TaskTitleList;
            Notebook.SelectTreeTask(task);
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

            var selectedTreeTask = Notebook.SelectedTreeTask;
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

        public void OnTreeViewKeyDown(KeyboardKeys keyboardKeys, KeyboardStates keyboardState)
        {
            Notebook.SelectedTask.KeyboardAction(
                keyboardKeys,
                () => keyboardState,
                () => { }, t => t.IsSelected = true,
                t => t.IsExpanded = true,
                () => true,
                OnDispatcher);
        }

        public void CopyTask()
        {
            OnTreeViewKeyDown(KeyboardKeys.Insert, KeyboardStates.IsControlPressed);
        }

        public void ContinueTask()
        {
            OnTreeViewKeyDown(KeyboardKeys.Insert, KeyboardStates.IsShiftPressed);
        }

        #region Fields

        private string _excelCsvText;
        private string _dataFile = "";
        private string _report;
        private bool _useTimeOptimization;
        private DataModel _data;

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

        #endregion

        #region Properties

        public Guid LastListTaskId { get; set; }
        public Guid LastTreeTaskId { get; set; }
        public LayoutViewModel Layout { get; } = new LayoutViewModel();
        public NotebookViewModel Notebook { get; } = new NotebookViewModel();
        public TaskViewModel RootTask => Notebook.RootTask;

        public string DataFile
        {
            get => _dataFile;
            set
            {
                if (!Set(ref _dataFile, value)) return;
                RaisePropertyChanged("Title");
            }
        }

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

        public ObservableCollection<FileInfo> MostRecentFiles { get; } = new ObservableCollection<FileInfo>();
        public ObservableCollection<string> TypeList { get; set; }
        public ObservableCollection<string> ContextList { get; set; }
        public ObservableCollection<string> TaskTitleList { get; set; }
        public bool CanSave { get; set; }
        public Action<Action> OnDispatcher { get; set; }
        public OutputViewModel Output { get; set; } = new OutputViewModel();

        #endregion
    }
}