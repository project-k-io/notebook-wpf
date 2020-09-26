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

        public DateTime DateStarted
        {
            get => Model.DateStarted;
            set
            {
                if (!this.Set((DateTime)Model.DateStarted, v => Model.DateStarted = v, value)) return;
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


        public DateTime TimeStarted
        {
            get => (DateTime)Model.DateStarted;
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
            get => (DateTime)Model.DateEnded;
            set
            {
                var dateEnded = Model.DateEnded;
                var dateTime = value;
                Model.DateEnded = new DateTime(dateEnded.Year, dateEnded.Month, dateEnded.Day, dateTime.Hour, dateTime.Minute,
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
                if (string.IsNullOrEmpty(Model.Type))
                    return false;
                var upper = Model.Type.ToUpper();
                return upper.Contains("LUNCH") || upper.Contains("PERSONAL");
            }
        }


        public TimeSpan Duration
        {
            get
            {
                if (Model.DateStarted == DateTime.MinValue || Model.DateEnded == DateTime.MinValue)
                    return TimeSpan.Zero;

                return Model.DateEnded - Model.DateStarted;
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


        public void LoadFrom(Domain.Versions.Version1.TaskModel model)
        {
            IsSelected = model.IsSelected;
            IsExpanded = model.IsExpanded;
            Model.Description = model.Description;
            Model.Type = model.Type;
            DateStarted = model.DateStarted;
            DateEnded = model.DateEnded;
            Model.Name = model.Title;

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
                    if (subTask.Model.DateEnded == DateTime.MinValue && index < Nodes.Count - 1)
                        subTask.Model.DateEnded = ((TaskViewModel) Nodes[index + 1]).Model.DateStarted;
                }

                Total = TimeSpan.Zero;
                for (var index = 0; index < Nodes.Count; ++index)
                {
                    var subTask = (TaskViewModel) Nodes[index];
                    subTask.FixTime();
                    Total += subTask.Total;
                }

                var subTask1 = (TaskViewModel) Nodes[Nodes.Count - 1];
                if (subTask1.Model.DateEnded != DateTime.MinValue)
                    DateEnded = subTask1.Model.DateEnded;
                var subTask2 = (TaskViewModel) Nodes[0];
                if (subTask2.Model.DateStarted != DateTime.MinValue)
                    DateStarted = subTask2.Model.DateStarted;
            }
        }

        public void FixTypes()
        {
            var type = Model.Type;
            if (string.IsNullOrEmpty(type))
            {
                var title = Model.Name;
                var upper = title.ToUpper();
                if (upper.Contains("LUNCH") || upper.Contains("BREAKFAST"))
                    type = "Lunch";
                else if (upper.Contains("TASK") || upper.Contains("CODE REVIEW") || title.Contains("TA") ||
                         title.Contains("US"))
                    type = "Dev";
                else if (upper.Contains("BUILD"))
                    type = "Build";
                else if (upper.Contains("TIME SHEET") || upper.Contains("TIMESHEET") || upper.Contains("EMAIL") ||
                         upper.Contains("PAPER WORKS"))
                    type = "Misc";
                else if (upper.Contains("TALKED") || upper.Contains("MEETING") || upper.Contains("SHOWED"))
                    type = "Meeting";
                else if (upper.Contains("Trouble"))
                    type = "Support";
            }

            foreach (var subTask in Nodes)
                ((TaskViewModel) subTask).FixTypes();
        }

        protected void FixTitles(string parent, Func<int, TaskViewModel, string> getTitle, TaskViewModel subTask, int ii)
        {
            if (Model.Context != parent)
                return;
            subTask.Model.Name = getTitle(ii, subTask);
        }

        private void FixTitles(TaskViewModel subTask, int ii)
        {
            var getTitle1 = (Func<int, TaskViewModel, string>) ((i, t) => t.Model.DateStarted.ToString("yyyy"));
            var getTitle2 = (Func<int, TaskViewModel, string>) ((i, t) => t.Model.DateStarted.ToString("MMMM"));
            var getTitle3 = (Func<int, TaskViewModel, string>) ((i, t) => "Week" + (i + 1));
            var getTitle4 = (Func<int, TaskViewModel, string>) ((i, t) => t.Model.DateStarted.DayOfWeek.ToString());
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
            var subTask = new TaskViewModel() { Model = { Name = "New Model", DateStarted = DateTime.Now, DateEnded = DateTime.Now}};
            Add(subTask);
            var ii = Nodes.IndexOf(subTask);
            FixContext(subTask);
            FixTitles(subTask, ii);
            return subTask;
        }

    }
}