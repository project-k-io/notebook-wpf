using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using ProjectK.Notebook.Domain;
using ProjectK.Utils.Extensions;

// using ProjectK.NotebookModel.Models.Versions.Version2;

namespace ProjectK.Notebook.ViewModels
{
    public class TaskViewModel : NodeViewModel
    {
        private TimeSpan _total;

        public TaskModel TaskModel { get; set; }

        public string Description
        {
            get => TaskModel.Description;
            set => this.Set(TaskModel.Description, v => TaskModel.Description = v, value);
        }

        public string Type
        {
            get => TaskModel.Type;
            set => this.Set(TaskModel.Type, v => TaskModel.Type = v, value);
        }

        public string SubType
        {
            get => TaskModel.SubType;
            set => this.Set(TaskModel.SubType, v => TaskModel.SubType = v, value);
        }

        public DateTime DateStarted
        {
            get => TaskModel.DateStarted;
            set
            {
                if (!this.Set(TaskModel.DateStarted, v => TaskModel.DateStarted = v, value)) return;
                RaisePropertyChanged("TimeStarted");
                RaisePropertyChanged("Duration");
            }
        }

        public DateTime DateEnded
        {
            get => TaskModel.DateEnded;
            set
            {
                if (!this.Set(DateEnded, v => TaskModel.DateEnded = v, value)) return;
                RaisePropertyChanged("TimeEnded");
                RaisePropertyChanged("Duration");
            }
        }


        public DateTime TimeStarted
        {
            get => TaskModel.DateStarted;
            set
            {
                var dateStarted = TaskModel.DateStarted;
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
            get => TaskModel.DateEnded;
            set
            {
                var dateEnded = TaskModel.DateEnded;
                var dateTime = value;
                DateEnded = new DateTime(dateEnded.Year, dateEnded.Month, dateEnded.Day, dateTime.Hour, dateTime.Minute,
                    dateTime.Second, dateTime.Millisecond);
                RaisePropertyChanged(); //MC
                RaisePropertyChanged("DateEnded");
                RaisePropertyChanged("Duration");
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

        public TimeSpan Total
        {
            get => _total;
            set => Set(ref _total, value);
        }

        #region Commands

        public ICommand CommandSetStartedTime => new RelayCommand(SetStartedTime);

        public ICommand CommandSetEndedTime => new RelayCommand(SetEndedTime);

        #endregion

        public TaskViewModel()
        {
            TaskModel = new TaskModel();
            Model = TaskModel;
        }

        public TaskViewModel(NodeModel model) : base(model)
        {
            if (model is TaskModel task)
                TaskModel = task;
        }


        public void LoadFrom(Domain.Versions.Version1.TaskModel model)
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

            Nodes = new ObservableCollection<NodeViewModel>();
            foreach (var subTask in model.SubTasks)
            {
                var node = new TaskViewModel();
                node.LoadFrom(subTask);
                Nodes.Add(node);
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

            if (Nodes.IsNullOrEmpty())
            {
                Total = Duration;
            }
            else
            {
                for (var index = 0; index < Nodes.Count; ++index)
                {
                    var subTask = (TaskViewModel) Nodes[index];
                    if (subTask.DateEnded == DateTime.MinValue && index < Nodes.Count - 1)
                        subTask.DateEnded = ((TaskViewModel) Nodes[index + 1]).DateStarted;
                }

                Total = TimeSpan.Zero;
                for (var index = 0; index < Nodes.Count; ++index)
                {
                    var subTask = (TaskViewModel) Nodes[index];
                    subTask.FixTime();
                    Total += subTask.Total;
                }

                var subTask1 = (TaskViewModel) Nodes[Nodes.Count - 1];
                if (subTask1.DateEnded != DateTime.MinValue)
                    DateEnded = subTask1.DateEnded;
                var subTask2 = (TaskViewModel) Nodes[0];
                if (subTask2.DateStarted != DateTime.MinValue)
                    DateStarted = subTask2.DateStarted;
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

            foreach (var subTask in Nodes)
                ((TaskViewModel) subTask).FixTypes();
        }

        protected void FixTitles(string parent, Func<int, TaskViewModel, string> getTitle, TaskViewModel subTask, int ii)
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
            for (var ii = 0; ii < Nodes.Count; ++ii)
            {
                var subTask = (TaskViewModel) Nodes[ii];
                FixTitles(subTask, ii);
                subTask.FixTitles();
            }
        }

        public override NodeViewModel AddNew()
        {
            var subTask = new TaskViewModel() {Title = "New TaskModel", DateStarted = DateTime.Now, DateEnded = DateTime.Now};
            Add(subTask);
            var ii = Nodes.IndexOf(subTask);
            FixContext(subTask);
            FixTitles(subTask, ii);
            return subTask;
        }
    }
}