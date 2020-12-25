using System;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Domain;
using ProjectK.Utils.Extensions;

namespace ProjectK.Notebook.ViewModels
{
    public class TaskViewModel : NodeViewModel
    {
        #region Static Fields

        private static readonly ILogger Logger = LogManager.GetLogger<TaskViewModel>();

        #endregion

        #region Fields

        // Model wrappers
        //private string _type;
        //private string _subType;
        //private DateTime _dateStarted;
        //private DateTime _dateEnded;

        // Misc
        private TimeSpan _total;

        #endregion

        #region Properties

        public TaskViewModel()
        {
            Kind = "Task";
        }

        public TaskViewModel(TaskModel model) : this()
        {
            Model = model;
        }

        public void ViewModelToModel(TaskModel model)
        {
            model.DateStarted = DateStarted;
            model.DateEnded = DateEnded;
            model.Type = Type;
            model.SubType = SubType;
        }

        public TimeSpan Total
        {
            get => _total;
            set => Set(ref _total, value);
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

        #endregion

        #region Commands

        public ICommand CommandSetStartedTime => new RelayCommand(SetStartedTime);
        public ICommand CommandSetEndedTime => new RelayCommand(SetEndedTime);

        #endregion

        #region Private Funtions

        private void SetStartedTime()
        {
            DateStarted = DateTime.Now;
        }

        private void SetEndedTime()
        {
            DateEnded = DateTime.Now;
        }

        private void LoadFrom(Domain.Versions.Version1.TaskModel model)
        {
            IsSelected = model.IsSelected;
            IsExpanded = model.IsExpanded;
            Description = model.Description;
            Type = model.Type;
            DateStarted = model.DateStarted;
            DateEnded = model.DateEnded;
            Name = model.Title;

            if (model.SubTasks.IsNullOrEmpty())
                return;

            foreach (var subTask in model.SubTasks)
            {
                var node = new TaskViewModel();
                node.LoadFrom(subTask);
                Nodes.Add(node);
            }
        }


        private void FixTypes()
        {
            if (string.IsNullOrEmpty(Type))
            {
                var title = Name;
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

        #endregion


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
#if AK
        public override void RaisePropertyChanged<T>(string propertyName = null, T oldValue = default, T newValue =
 default, bool broadcast = false)
        {
            base.RaisePropertyChanged(propertyName, oldValue, newValue, broadcast);
            if (!IsTaskModelProperty(propertyName)) return;

            Logger?.LogDebug($@"[Node] PropertyChanged: {propertyName} | {oldValue} | {newValue}");
            Modified = ModifiedStatus.Modified;
            SetParentChildModified();
            MessengerInstance.Send(new NotificationMessage<TaskModel>(Model, "Modified"));
        }
#endif

        private static bool IsTaskModelProperty(string n)
        {
            return n == "Type" ||
                   n == "SubType" ||
                   n == "DataStarted" ||
                   n == "DateEnded";
        }

        private TaskModel Task => Model as TaskModel;

        public string Type
        {
            get => Task.Type;
            set => this.Set(Type, v => Task.Type = v, value);
        }

        public string SubType
        {
            get => Task.SubType;
            set => this.Set(SubType, v => Task.SubType = v, value);
        }

        public DateTime DateStarted
        {
            get => Task.DateStarted;
            set
            {
                if (!this.Set(DateStarted, v => Task.DateStarted = v, value)) return;
                RaisePropertyChanged("TimeStarted");
                RaisePropertyChanged("Duration");
            }
        }

        public DateTime DateEnded
        {
            get => Task.DateEnded;
            set
            {
                if (!this.Set(DateEnded, v => Task.DateEnded = v, value)) return;
                RaisePropertyChanged("TimeEnded");
                RaisePropertyChanged("Duration");
            }
        }

        // Derived Properties
        public DateTime TimeStarted
        {
            get => DateEnded;
            set
            {
                var d = DateEnded;
                var t = value;
                DateStarted = new DateTime(d.Year, d.Month, d.Day, t.Hour, t.Minute, t.Second, t.Millisecond);
                RaisePropertyChanged(); // MC
                RaisePropertyChanged("DateStarted");
                RaisePropertyChanged("Duration");
            }
        }

        public DateTime TimeEnded
        {
            get => DateEnded;
            set
            {
                var d = DateEnded;
                var t = value;
                DateEnded = new DateTime(d.Year, d.Month, d.Day, t.Hour, t.Minute, t.Second, t.Millisecond);
                RaisePropertyChanged(); //MC
                RaisePropertyChanged("DateEnded");
                RaisePropertyChanged("Duration");
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
    }
}