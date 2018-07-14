// Decompiled with JetBrains decompiler
// Type: Projects.ViewModels.ProjectViewModel
// Assembly: Projects.ViewModels, Version=1.1.8.29121, Culture=neutral, PublicKeyToken=null
// MVID: AA177939-1C69-401F-8524-6C17EE86E3CA
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.ViewModels.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Projects.Models.Versions.Version2;
using Vibor.Helpers;
using Vibor.Mvvm;

namespace Projects.ViewModels
{
    public class ProjectViewModel : BaseViewModel
    {
        private TaskViewModel _selectedTask;
        private TaskViewModel _selectedTreeTask;

        public ObservableCollection<TaskViewModel> SelectedTaskList { get; } =
            new ObservableCollection<TaskViewModel>();

        public TaskViewModel RootTask { get; set; } = new TaskViewModel();

        public TaskViewModel SelectedTreeTask
        {
            get => _selectedTreeTask;
            set
            {
                if (_selectedTreeTask == value)
                    return;
                _selectedTreeTask = value;
                OnPropertyChanged(nameof(SelectedTreeTask));
            }
        }

        public TaskViewModel SelectedTask
        {
            get => _selectedTask;
            set
            {
                if (_selectedTask == value)
                    return;
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
            }
        }

        public ObservableCollection<string> ContextList { get; set; } = new ObservableCollection<string>();

        public List<DateTime> GetSelectedDays()
        {
            var dateTimeList = new List<DateTime>();
            foreach (var selectedTask in SelectedTaskList)
                if (!(selectedTask.Context != "Day"))
                    dateTimeList.Add(selectedTask.DateStarted);
            return dateTimeList;
        }

        public event EventHandler SelectedDaysChanged;

        public void OnSelectedDaysChanged()
        {
            var selectedDaysChanged = SelectedDaysChanged;
            if (selectedDaysChanged == null)
                return;
            selectedDaysChanged(this, EventArgs.Empty);
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
            XTask.AddToList(SelectedTaskList, task);
            OnSelectedDaysChanged();
            SelectedTask = !XList.IsNullOrEmpty(SelectedTaskList) ? SelectedTaskList[0] : task;
            OnPropertyChanged("SelectedTaskList");
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
            OnPropertyChanged("SelectedTaskList");
        }

        public void SelectTask(Guid id)
        {
            SelectTask(FindTask(id));
        }

        public static bool ContainDate(IList dates, DateTime a)
        {
            foreach (var date in dates)
                if (date is DateTime)
                {
                    var dateTime = (DateTime) date;
                    if (a.Day == dateTime.Day && a.Month == dateTime.Month && a.Year == dateTime.Year)
                        return true;
                }

            return false;
        }

        private static void AddToList(ICollection<TaskViewModel> list, TaskViewModel task, IList dates)
        {
            if (ContainDate(dates, task.DateStarted))
                list.Add(task);
            foreach (var subTask in task.SubTasks)
                AddToList(list, subTask, dates);
        }

        private static void SaveTo(DataModel model, TaskViewModel task)
        {
            model.Tasks.Add(task.Model);
            if (task.Model.Id == Guid.Empty)
                task.Model.Id = Guid.NewGuid();
            foreach (var subTask in task.SubTasks)
            {
                SaveTo(model, subTask);
                subTask.Model.ParentId = task.Model.Id;
            }
        }

        public void LoadFrom(Models.Versions.Version1.DataModel model)
        {
            Clear();
            RootTask.LoadFrom(model.RootTask);
        }

        public void LoadFrom(DataModel model)
        {
            if (model == null)
                return;
            Clear();
            var sortedList = new SortedList<Guid, TaskViewModel>();
            foreach (var task in model.Tasks)
                if (task.ParentId == Guid.Empty)
                {
                    RootTask.Model = task;
                    sortedList.Add(task.Id, RootTask);
                }
                else if (!sortedList.ContainsKey(task.Id))
                {
                    var taskViewModel = new TaskViewModel {Model = task};
                    sortedList.Add(task.Id, taskViewModel);
                }

            foreach (var task in model.Tasks)
                if (!(task.ParentId == Guid.Empty))
                    sortedList[task.ParentId].SubTasks.Add(sortedList[task.Id]);
        }

        public void SaveTo(DataModel model)
        {
            model.Tasks.Clear();
            SaveTo(model, RootTask);
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
            OnPropertyChanged("ContextList");
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

        public void UpdateSelectDayTaks(IList dates)
        {
            SelectedTaskList.Clear();
            AddToList(SelectedTaskList, RootTask, dates);
        }
    }
}