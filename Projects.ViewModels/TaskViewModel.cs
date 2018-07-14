using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Projects.Models.Versions.Version2;
using Vibor.Helpers;
using Vibor.Mvvm;

namespace Projects.ViewModels
{
    public class TaskViewModel : BaseViewModel, XTask.ITask<TaskViewModel>
    {
        public enum KeyboardStates
        {
            None,
            IsShiftPressed,
            IsControlPressed,
            IsCtrlShiftPressed
        }

        public enum KeyStates
        {
            None,
            Insert,
            Delete,
            Left,
            Right,
            Up,
            Down
        }

        private static int _rating;
        private bool _isExpanded;
        private bool _isSelected;
        private TimeSpan _total;

        public TaskViewModel()
        {
            Parent = null;
            Model = new TaskModel();
        }

        public TaskViewModel(string title, int rating)
        {
            Model = new TaskModel();
            Parent = null;
            Title = title;
            Rating = rating;
        }

        public TaskModel Model { get; set; }

        public ObservableCollection<string> TypeList { get; set; }

        public ObservableCollection<string> ContextList { get; set; }

        public ObservableCollection<string> TaskTitleList { get; set; }

        public string Context
        {
            get => Model.Context;
            set
            {
                if (Model.Context == value)
                    return;
                Model.Context = value;
                OnPropertyChanged(nameof(Context));
            }
        }

        public int Rating
        {
            get => Model.Rating;
            set
            {
                if (Model.Rating == value)
                    return;
                Model.Rating = value;
                OnPropertyChanged(nameof(Rating));
            }
        }

        public TaskViewModel Parent { get; set; }

        public Guid Id => Model.Id;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                    return;
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded == value)
                    return;
                _isExpanded = value;
                OnPropertyChanged("IsExpnaded");
            }
        }

        public string Description
        {
            get => Model.Description;
            set
            {
                if (Model.Description == value)
                    return;
                Model.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string Type
        {
            get => Model.Type;
            set
            {
                if (Model.Type == value)
                    return;
                Model.Type = value;
                OnPropertyChanged(nameof(Type));
            }
        }

        public string SubType
        {
            get => Model.SubType;
            set
            {
                if (Model.SubType == value)
                    return;
                Model.SubType = value;
                OnPropertyChanged(nameof(SubType));
            }
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

        public DateTime DateStarted
        {
            get => Model.DateStarted;
            set
            {
                if (Model.DateStarted == value)
                    return;
                Model.DateStarted = value;
                OnPropertyChanged(nameof(DateStarted));
                OnPropertyChanged("TimeStarted");
                OnPropertyChanged("Duration");
            }
        }

        public DateTime DateEnded
        {
            get => Model.DateEnded;
            set
            {
                if (Model.DateEnded == value)
                    return;
                Model.DateEnded = value;
                OnPropertyChanged(nameof(DateEnded));
                OnPropertyChanged("TimeEnded");
                OnPropertyChanged("Duration");
            }
        }

        public string Title
        {
            get => Model.Title;
            set
            {
                if (Model.Title == value)
                    return;
                Model.Title = value;
                OnPropertyChanged(nameof(Title));
            }
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
                OnPropertyChanged(nameof(TimeStarted));
                OnPropertyChanged("DateStarted");
                OnPropertyChanged("Duration");
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
                OnPropertyChanged(nameof(TimeEnded));
                OnPropertyChanged("DateEnded");
                OnPropertyChanged("Duration");
            }
        }

        public TimeSpan Total
        {
            get => _total;
            set
            {
                if (_total == value)
                    return;
                _total = value;
                OnPropertyChanged(nameof(Total));
            }
        }

        public TaskViewModel LastSubTask => SubTasks.LastOrDefault();

        public ICommand CommandSetStartedTime => new RelayCommand(SetStartedTime, null);

        public ICommand CommandSetEndedTime => new RelayCommand(SetEndedTime, null);

        public ObservableCollection<TaskViewModel> SubTasks { get; set; } = new ObservableCollection<TaskViewModel>();

        public void LoadFrom(Models.Versions.Version1.TaskModel model)
        {
            Rating = model.Rating;
            IsSelected = model.IsSelected;
            IsExpanded = model.IsExpanded;
            Description = model.Description;
            Type = model.Type;
            DateStarted = model.DateStarted;
            DateEnded = model.DateEnded;
            Title = model.Title;
            if (XList.IsNullOrEmpty(model.SubTasks))
                return;
            SubTasks = new ObservableCollection<TaskViewModel>();
            foreach (var subTask in model.SubTasks)
            {
                var taskViewModel = new TaskViewModel();
                taskViewModel.LoadFrom(subTask);
                SubTasks.Add(taskViewModel);
            }
        }

        public void PopulateModel()
        {
            if (!(Model.Id == Guid.Empty))
                return;
            Model.Id = Guid.NewGuid();
        }

        public TaskViewModel AddNewTask()
        {
            var subTask = new TaskViewModel {Title = "New Task", DateStarted = DateTime.Now, DateEnded = DateTime.Now};
            Add(subTask);
            var ii = SubTasks.IndexOf(subTask);
            FixContext(subTask);
            FixTitles(subTask, ii);
            return subTask;
        }

        public void Add(TaskViewModel subTask)
        {
            subTask.Parent = this;
            SubTasks.Add(subTask);
        }

        public void Insert(int index, TaskViewModel subTask)
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

        public void SetStartedTime()
        {
            DateStarted = DateTime.Now;
        }

        public void SetEndedTime()
        {
            DateEnded = DateTime.Now;
        }

        public void FixTime()
        {
            if (IsPersonalType)
                return;
            if (XList.IsNullOrEmpty(SubTasks))
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

        public void FixTitles(TaskViewModel subTask, int ii)
        {
            var getTitle1 = (Func<int, TaskViewModel, string>) ((i, t) => t.DateStarted.ToString("yyyy"));
            var getTitle2 = (Func<int, TaskViewModel, string>) ((i, t) => t.DateStarted.ToString("MMMM"));
            var getTitle3 = (Func<int, TaskViewModel, string>) ((i, t) => "Week" + (i + 1).ToString());
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

        public override string ToString()
        {
            return Model.ToString();
        }

        public static void OnTreeViewKeyDown(TaskViewModel task, KeyStates key, Func<KeyboardStates> getState,
            Action handled, Action<TaskViewModel> selectItem, Action<TaskViewModel> expandItem,
            Func<bool> deleteMessageBox, Action<Action> dispatcher)
        {
            switch (key)
            {
                case KeyStates.Insert:
                    TaskViewModel taskViewModel1;
                    if (getState() == KeyboardStates.IsShiftPressed)
                    {
                        taskViewModel1 = task.Parent.AddNewTask();
                        taskViewModel1.DateStarted = task.DateEnded;
                    }
                    else if (getState() == KeyboardStates.IsControlPressed)
                    {
                        var lastSubTask = task.Parent.LastSubTask;
                        taskViewModel1 = task.Parent.AddNewTask();
                        if (lastSubTask != null)
                        {
                            taskViewModel1.Type = task.Type;
                            taskViewModel1.Title = task.Title;
                            taskViewModel1.DateStarted = lastSubTask.DateEnded;
                        }
                    }
                    else
                    {
                        taskViewModel1 = task.AddNewTask();
                    }

                    taskViewModel1.Rating = _rating++;
                    task.IsSelected = true;
                    selectItem(task);
                    expandItem(task);
                    handled();
                    break;
                case KeyStates.Delete:
                    if (deleteMessageBox())
                        break;
                    var parent = task.Parent;
                    if (parent == null)
                        break;
                    var num1 = parent.SubTasks.IndexOf(task);
                    dispatcher(() => parent.SubTasks.Remove(task));
                    var taskViewModel2 = num1 > 0 ? parent.SubTasks[num1 - 1] : parent;
                    if (taskViewModel2 == null)
                        break;
                    selectItem(taskViewModel2);
                    handled();
                    break;
                case KeyStates.Left:
                    if (getState() == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = task.Parent;
                        if (parent1 == null)
                            break;
                        var parent2 = parent1.Parent;
                        if (parent2 == null)
                            break;
                        parent1.SubTasks.Remove(task);
                        var num2 = parent2.SubTasks.IndexOf(parent1);
                        parent2.Insert(num2 + 1, task);
                        selectItem(task);
                        handled();
                    }

                    break;
                case KeyStates.Right:
                    if (getState() == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = task.Parent;
                        if (parent1 == null)
                            break;
                        var num2 = parent1.SubTasks.IndexOf(task);
                        if (num2 <= 0)
                            break;
                        var subTask = parent1.SubTasks[num2 - 1];
                        if (subTask == null)
                            break;
                        parent1.SubTasks.Remove(task);
                        subTask.Add(task);
                        selectItem(task);
                        parent1.IsExpanded = true;
                        task.IsSelected = true;
                        handled();
                    }

                    break;
                case KeyStates.Up:
                    if (getState() == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = task.Parent;
                        if (parent1 == null)
                            break;
                        var num2 = parent1.SubTasks.IndexOf(task);
                        if (num2 <= 0)
                            break;
                        parent1.SubTasks.Remove(task);
                        parent1.Insert(num2 - 1, task);
                        selectItem(task);
                        parent1.IsExpanded = true;
                        task.IsSelected = true;
                        handled();
                    }

                    break;
                case KeyStates.Down:
                    if (getState() == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = task.Parent;
                        if (parent1 == null)
                            break;
                        var num2 = parent1.SubTasks.IndexOf(task);
                        if (num2 >= parent1.SubTasks.Count - 1)
                            break;
                        parent1.SubTasks.Remove(task);
                        parent1.Insert(num2 + 1, task);
                        selectItem(task);
                        parent1.IsExpanded = true;
                        task.IsSelected = true;
                        handled();
                    }

                    break;
            }
        }
    }
}