using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Models.Versions.Version2;
using ProjectK.Notebook.ViewModels.Enums;
using ProjectK.Utils;
using ProjectK.Utils.Extensions;

namespace ProjectK.Notebook.ViewModels
{
    public class TaskViewModel : ViewModelBase, ITask<TaskViewModel>
    {
        private static readonly ILogger Logger = LogManager.GetLogger<TaskViewModel>();
        private static int _globalLevel;

        #region Override functions

        public override string ToString()
        {
            return Model.ToString();
        }

        #endregion


        public void SaveTo(List<TaskModel> tasks)
        {
            tasks.Add(Model);
            TrySetId();

            foreach (var subTask in SubTasks)
            {
                subTask.SaveTo(tasks);
                subTask.ParentId = Model.Id;
            }
        }

        #region Fields

        private bool _isExpanded;
        private bool _isSelected;
        private TimeSpan _total;

        #endregion


        #region Properties - Model Wrappers

        public TaskModel Model { get; set; }

        public Guid Id => Model.Id;

        public Guid ParentId
        {
            get => Model.ParentId;
            set => Model.ParentId = value;
        }

        public string Description
        {
            get => Model.Description;
            set => this.Set(Description, v => Model.Description = v, value);
        }

        public string Type
        {
            get => Model.Type;
            set => this.Set(Type, v => Model.Type = v, value);
        }

        public string SubType
        {
            get => Model.SubType;
            set => this.Set(SubType, v => Model.SubType = v, value);
        }

        public DateTime DateStarted
        {
            get => Model.DateStarted;
            set
            {
                if (!this.Set(DateStarted, v => Model.DateStarted = v, value)) return;
                RaisePropertyChanged("TimeStarted");
                RaisePropertyChanged("Duration");
            }
        }

        public DateTime DateEnded
        {
            get => Model.DateEnded;
            set
            {
                if (!this.Set(DateEnded, v => Model.DateEnded = v, value)) return;
                RaisePropertyChanged("TimeEnded");
                RaisePropertyChanged("Duration");
            }
        }

        public string Title
        {
            get => Model.Title;
            set => this.Set(Model.Title, v =>
            {
                Model.Level = _globalLevel++;
                Model.Title = v;
            }, value);
        }

        public DateTime TimeStarted
        {
            get => Model.DateStarted;
            set
            {
                var dateStarted = Model.DateStarted;
                var dateTime = value;
                DateStarted = new DateTime(dateStarted.Year, dateStarted.Month, dateStarted.Day, dateTime.Hour,
                    dateTime.Minute, dateTime.Second, dateTime.Millisecond);
                RaisePropertyChanged(); // MC
                RaisePropertyChanged("DateStarted");
                RaisePropertyChanged("Duration");
            }
        }

        public DateTime TimeEnded
        {
            get => Model.DateEnded;
            set
            {
                var dateEnded = Model.DateEnded;
                var dateTime = value;
                DateEnded = new DateTime(dateEnded.Year, dateEnded.Month, dateEnded.Day, dateTime.Hour, dateTime.Minute,
                    dateTime.Second, dateTime.Millisecond);
                RaisePropertyChanged(); //MC
                RaisePropertyChanged("DateEnded");
                RaisePropertyChanged("Duration");
            }
        }

        #endregion

        #region Properties

        public ObservableCollection<string> TypeList { get; set; }
        public ObservableCollection<string> ContextList { get; set; }
        public ObservableCollection<string> TaskTitleList { get; set; }

        public string Context
        {
            get => Model.Context;
            set => this.Set(Context, v => Model.Context = v, value);
        }

        public TaskViewModel Parent { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => Set(ref _isExpanded, value);
        }

        public bool IsPersonalType
        {
            get
            {
                if (string.IsNullOrEmpty(Type))
                    return false;
                var upper = Type.ToUpper();
                return upper.Contains("LUNCH") || upper.Contains("PERSONAL");
            }
        }

        public bool IsSubTypeSleep
        {
            get
            {
                if (string.IsNullOrEmpty(SubType))
                    return false;
                return SubType.ToUpper().Contains("SLEEP");
            }
        }

        public TimeSpan Duration
        {
            get
            {
                if (DateStarted == DateTime.MinValue || DateEnded == DateTime.MinValue)
                    return TimeSpan.Zero;

                return DateEnded - DateStarted;
            }
        }

        public TimeSpan Total
        {
            get => _total;
            set => Set(ref _total, value);
        }

        public TaskViewModel LastSubTask => SubTasks.LastOrDefault();
        public ObservableCollection<TaskViewModel> SubTasks { get; set; } = new ObservableCollection<TaskViewModel>();

        #endregion

        #region Commands

        public ICommand CommandSetStartedTime => new RelayCommand(SetStartedTime);

        public ICommand CommandSetEndedTime => new RelayCommand(SetEndedTime);

        #endregion

        #region Constructors

        public TaskViewModel()
        {
            Parent = null;
            Model = new TaskModel();
        }

        public TaskViewModel(string title)
        {
            Model = new TaskModel();
            Parent = null;
            Title = title;
        }

        #endregion

        #region Public functions

        public void TrySetId()
        {
            if (Model.Id != Guid.Empty)
                return;

            Model.Id = Guid.NewGuid();
        }

        public void LoadFrom(Models.Versions.Version1.TaskModel model)
        {
            IsSelected = model.IsSelected;
            IsExpanded = model.IsExpanded;
            Description = model.Description;
            Type = model.Type;
            DateStarted = model.DateStarted;
            DateEnded = model.DateEnded;
            Title = model.Title;
            if (model.SubTasks.IsNullOrEmpty())
                return;

            SubTasks = new ObservableCollection<TaskViewModel>();
            foreach (var subTask in model.SubTasks)
            {
                var taskViewModel = new TaskViewModel();
                taskViewModel.LoadFrom(subTask);
                SubTasks.Add(taskViewModel);
            }
        }

        private TaskViewModel AddNewTask()
        {
            var subTask = new TaskViewModel {Title = "New Task", DateStarted = DateTime.Now, DateEnded = DateTime.Now};
            Add(subTask);
            var ii = SubTasks.IndexOf(subTask);
            FixContext(subTask);
            FixTitles(subTask, ii);
            return subTask;
        }

        public TaskViewModel Add(TaskViewModel subTask)
        {
            if (subTask.Title == "Time Tracker2")
                Logger.LogDebug(subTask.Title);

            subTask.Parent = this;
            SubTasks.Add(subTask);
            return subTask;
        }

        private void Insert(int index, TaskViewModel subTask)
        {
            subTask.Parent = this;
            SubTasks.Insert(index, subTask);
        }

        public void SetParents()
        {
            foreach (var subTask in SubTasks)
            {
                subTask.Parent = this;
                subTask.SetParents();
            }
        }

        private void SetStartedTime()
        {
            DateStarted = DateTime.Now;
        }

        private void SetEndedTime()
        {
            DateEnded = DateTime.Now;
        }

        public void FixTime()
        {
            if (IsPersonalType)
                return;

            if (SubTasks.IsNullOrEmpty())
            {
                Total = Duration;
            }
            else
            {
                for (var index = 0; index < SubTasks.Count; ++index)
                {
                    var subTask = SubTasks[index];
                    if (subTask.DateEnded == DateTime.MinValue && index < SubTasks.Count - 1)
                        subTask.DateEnded = SubTasks[index + 1].DateStarted;
                }

                Total = TimeSpan.Zero;
                for (var index = 0; index < SubTasks.Count; ++index)
                {
                    var subTask = SubTasks[index];
                    subTask.FixTime();
                    Total += subTask.Total;
                }

                var subTask1 = SubTasks[SubTasks.Count - 1];
                if (subTask1.DateEnded != DateTime.MinValue)
                    DateEnded = subTask1.DateEnded;
                var subTask2 = SubTasks[0];
                if (subTask2.DateStarted != DateTime.MinValue)
                    DateStarted = subTask2.DateStarted;
            }
        }

        public void ExtractContext(ObservableCollection<string> contextList)
        {
            if (!string.IsNullOrEmpty(Context) && !contextList.Contains(Context))
                contextList.Add(Context);
            foreach (var subTask in SubTasks)
                subTask.ExtractContext(contextList);
        }

        private void FixContext(string parent, string child, TaskViewModel subTask)
        {
            if (!(Context == parent))
                return;
            subTask.Context = child;
        }

        private void FixContext(TaskViewModel subTask)
        {
            FixContext("Time Tracker", "Year", subTask);
            FixContext("Year", "Month", subTask);
            FixContext("Month", "Week", subTask);
            FixContext("Week", "Day", subTask);
            FixContext("Day", "Task", subTask);
            FixContext("Task", "Task", subTask);
        }

        public void FixContext()
        {
            foreach (var subTask in SubTasks)
            {
                FixContext(subTask);
                subTask.FixContext();
            }
        }

        private void FixTitles(string parent, Func<int, TaskViewModel, string> getTitle, TaskViewModel subTask, int ii)
        {
            if (!(Context == parent))
                return;
            subTask.Title = getTitle(ii, subTask);
        }

        private void FixTitles(TaskViewModel subTask, int ii)
        {
            var getTitle1 = (Func<int, TaskViewModel, string>) ((i, t) => t.DateStarted.ToString("yyyy"));
            var getTitle2 = (Func<int, TaskViewModel, string>) ((i, t) => t.DateStarted.ToString("MMMM"));
            var getTitle3 = (Func<int, TaskViewModel, string>) ((i, t) => "Week" + (i + 1));
            var getTitle4 = (Func<int, TaskViewModel, string>) ((i, t) => t.DateStarted.DayOfWeek.ToString());
            FixTitles("Time Tracker", getTitle1, subTask, ii);
            FixTitles("Year", getTitle2, subTask, ii);
            FixTitles("Month", getTitle3, subTask, ii);
            FixTitles("Week", getTitle4, subTask, ii);
        }

        public void FixTitles()
        {
            for (var ii = 0; ii < SubTasks.Count; ++ii)
            {
                var subTask = SubTasks[ii];
                FixTitles(subTask, ii);
                subTask.FixTitles();
            }
        }

        public void FixTypes()
        {
            if (string.IsNullOrEmpty(Type))
            {
                var title = Title;
                var upper = title.ToUpper();
                if (upper.Contains("LUNCH") || upper.Contains("BREAKFAST"))
                    Type = "Lunch";
                else if (upper.Contains("TASK") || upper.Contains("CODE REVIEW") || title.Contains("TA") ||
                         title.Contains("US"))
                    Type = "Dev";
                else if (upper.Contains("BUILD"))
                    Type = "Build";
                else if (upper.Contains("TIME SHEET") || upper.Contains("TIMESHEET") || upper.Contains("EMAIL") ||
                         upper.Contains("PAPER WORKS"))
                    Type = "Misc";
                else if (upper.Contains("TALKED") || upper.Contains("MEETING") || upper.Contains("SHOWED"))
                    Type = "Meeting";
                else if (upper.Contains("Trouble"))
                    Type = "Support";
            }

            foreach (var subTask in SubTasks)
                subTask.FixTypes();
        }

        public TaskViewModel FindTask(Guid id)
        {
            if (Id == id)
                return this;
            foreach (var subTask in SubTasks)
            {
                var task = subTask.FindTask(id);
                if (task != null)
                    return task;
            }

            return null;
        }


        public void KeyboardAction(
            KeyboardKeys keyboardKeys,
            Func<KeyboardStates> getState,
            Action handled,
            Action<TaskViewModel> selectItem, Action<TaskViewModel> expandItem,
            Func<bool> deleteMessageBox,
            Action<Action> dispatcher)
        {
            var state = getState();
            
            // don't show logging ctl, alt, shift or arrow keys
            if(keyboardKeys != KeyboardKeys.None && state != KeyboardStates.None)
                Logger.LogDebug($"KeyboardAction: {keyboardKeys}, {state}");

            switch (keyboardKeys)
            {
                case KeyboardKeys.Insert:
                    TaskViewModel task;
                    switch (state)
                    {
                        case KeyboardStates.IsShiftPressed:
                            task = Parent.AddNewTask();
                            task.DateStarted = DateEnded;
                            break;
                        case KeyboardStates.IsControlPressed:
                            var lastSubTask = Parent.LastSubTask;
                            task = Parent.AddNewTask();
                            if (lastSubTask != null)
                            {
                                task.Type = Type;
                                task.Title = Title;
                                task.DateStarted = lastSubTask.DateEnded;
                            }

                            break;
                        default:
                            task = AddNewTask();
                            break;
                    }

                    IsSelected = true;
                    selectItem(this);
                    expandItem(this);
                    handled();
                    Logger.LogDebug($"Added [{task.Title}] to [{Title}]");
                    break;
                case KeyboardKeys.Delete:
                    if (deleteMessageBox())
                        break;
                    var parent = Parent;
                    if (parent == null)
                        break;
                    var num1 = parent.SubTasks.IndexOf(this);
                    dispatcher(() => parent.SubTasks.Remove(this));
                    var taskViewModel2 = num1 > 0 ? parent.SubTasks[num1 - 1] : parent;
                    if (taskViewModel2 == null)
                        break;
                    selectItem(taskViewModel2);
                    handled();
                    break;
                case KeyboardKeys.Left:
                    if (state == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = Parent;
                        if (parent1 == null)
                            break;
                        var parent2 = parent1.Parent;
                        if (parent2 == null)
                            break;
                        parent1.SubTasks.Remove(this);
                        var num2 = parent2.SubTasks.IndexOf(parent1);
                        parent2.Insert(num2 + 1, this);
                        selectItem(this);
                        handled();
                    }

                    break;
                case KeyboardKeys.Right:
                    if (state == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = Parent;
                        if (parent1 == null)
                            break;
                        var num2 = parent1.SubTasks.IndexOf(this);
                        if (num2 <= 0)
                            break;
                        var subTask = parent1.SubTasks[num2 - 1];
                        if (subTask == null)
                            break;
                        parent1.SubTasks.Remove(this);
                        subTask.Add(this);
                        selectItem(this);
                        parent1.IsExpanded = true;
                        IsSelected = true;
                        handled();
                    }

                    break;
                case KeyboardKeys.Up:
                    if (state == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = Parent;
                        if (parent1 == null)
                            break;
                        var num2 = parent1.SubTasks.IndexOf(this);
                        if (num2 <= 0)
                            break;
                        parent1.SubTasks.Remove(this);
                        parent1.Insert(num2 - 1, this);
                        selectItem(this);
                        parent1.IsExpanded = true;
                        IsSelected = true;
                        handled();
                    }

                    break;
                case KeyboardKeys.Down:
                    if (state == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = Parent;
                        if (parent1 == null)
                            break;
                        var num2 = parent1.SubTasks.IndexOf(this);
                        if (num2 >= parent1.SubTasks.Count - 1)
                            break;
                        parent1.SubTasks.Remove(this);
                        parent1.Insert(num2 + 1, this);
                        selectItem(this);
                        parent1.IsExpanded = true;
                        IsSelected = true;
                        handled();
                    }

                    break;
            }
        }

        #endregion
    }
}