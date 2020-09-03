using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using ProjectK.Notebook.ViewModels.Extensions;
using ProjectK.Utils;
using ProjectK.Utils.Extensions;
using ProjectK.Notebook.Models.Versions.Version2;
namespace ProjectK.Notebook.ViewModels
{
    public class NotebookViewModel : ViewModelBase
    {
        private TaskViewModel _selectedTask;
        private TaskViewModel _selectedTreeTask;
        public DataModel _data;

        public ObservableCollection<TaskViewModel> SelectedTaskList { get; } =
            new ObservableCollection<TaskViewModel>();

        public TaskViewModel RootTask { get; set; } = new TaskViewModel();

        public TaskViewModel SelectedTreeTask
        {
            get => _selectedTreeTask;
            set => Set(ref _selectedTreeTask, value);
        }

        public TaskViewModel SelectedTask
        {
            get => _selectedTask;
            set => Set(ref _selectedTask, value);
        }

        public ObservableCollection<string> ContextList { get; set; } = new ObservableCollection<string>();
        public string Path { get; set; }

        public List<DateTime> GetSelectedDays()
        {
            var dateTimeList = new List<DateTime>();
            foreach (var selectedTask in SelectedTaskList)
                if (selectedTask.Context == "Day")
                    dateTimeList.Add(selectedTask.DateStarted);

            return dateTimeList;
        }

        public event EventHandler SelectedDaysChanged;

        public void OnSelectedDaysChanged()
        {
            SelectedDaysChanged?.Invoke(this, EventArgs.Empty);
        }

        public TaskViewModel FindTask(Guid id)
        {
            return RootTask.FindTask(id);
        }

        public void SelectTreeTask(TaskViewModel task)
        {
            if (task == null)
                return;

            SelectedTreeTask = task;
            SelectedTaskList.Clear();
            SelectedTaskList.AddToList(task);
            OnSelectedDaysChanged();
            SelectedTask = !SelectedTaskList.IsNullOrEmpty() ? SelectedTaskList[0] : task;
            RaisePropertyChanged("SelectedTaskList");
        }

        public void SelectTreeTask(Guid id)
        {
            SelectTreeTask(FindTask(id));
        }

        public void SelectTask(TaskViewModel task)
        {
            if (task == null)
                return;
            SelectedTask = task;
            RaisePropertyChanged("SelectedTaskList");
        }

        public void SelectTask(Guid id)
        {
            SelectTask(FindTask(id));
        }

        public static bool ContainDate(IList dates, DateTime a)
        {
            foreach (var date in dates)
                if (date is DateTime dateTime)
                    if (a.Day == dateTime.Day && a.Month == dateTime.Month && a.Year == dateTime.Year)
                        return true;

            return false;
        }

        private static void AddToList(ICollection<TaskViewModel> list, TaskViewModel task, IList dates)
        {
            if (ContainDate(dates, task.DateStarted))
                list.Add(task);
            foreach (var subTask in task.SubTasks)
                AddToList(list, subTask, dates);
        }


        public void LoadFrom(Models.Versions.Version1.DataModel model)
        {
            Clear();
            RootTask.LoadFrom(model.RootTask);
        }

        public void LoadFrom(Models.Versions.Version2.DataModel model)
        {
            if (model == null)
                return;

            var tasks = model.Tasks;
            Clear();

            // Build Tree
            RootTask.BuildTree(tasks);
        }


        public void FixTime()
        {
            SelectedTreeTask.FixTime();
        }

        public void Clear()
        {
            RootTask.SubTasks.Clear();
        }

        public void ExtractContext()
        {
            ContextList.Clear();
            RootTask.ExtractContext(ContextList);
            RaisePropertyChanged("ContextList");
        }

        public void FixContext()
        {
            SelectedTreeTask.FixContext();
        }

        public void FixTitles()
        {
            SelectedTreeTask.FixTitles();
        }

        public void FixTypes()
        {
            SelectedTreeTask.FixTypes();
        }

        public void UpdateSelectDayTasks(IList dates)
        {
            SelectedTaskList.Clear();
            AddToList(SelectedTaskList, RootTask, dates);
        }
    }
}