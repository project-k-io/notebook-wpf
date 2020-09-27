﻿using System;
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
        #region Fields
        
        private string _description;
        private string _type;
        private string _subType;
        private DateTime _dateStarted;
        private DateTime _dateEnded;
        private TimeSpan _total;

        #endregion

        #region Properties

        public override dynamic ModelA
        {
            get
            {
                var model = new TaskModel
                {
                    NodeId = this.Id,
                    ParentId = ParentId,
                    Name = Name,
                    DateStarted = DateStarted,
                    DateEnded = DateEnded,
                    Description = Description,
                    Type = Type,
                    SubType = SubType,
                };
                return model;
            }
            set
            {
                if (!(value is TaskModel m)) return;
                Kind = "Task";
                Id = m.NodeId;
                ParentId = m.ParentId;
                Name = m.Name;
                DateStarted = m.DateStarted;
                DateEnded = m.DateEnded;
                Context = m.Context;
                Description = m.Description;
                Type = m.Type;
                SubType = m.SubType;
            }
        }
        // Model Properties
        public string Description { get => _description; set => Set(ref _description, value); }
        public string Type { get => _type; set => Set(ref _type, value); }
        public string SubType { get => _subType; set => Set(ref _subType, value); }
        public DateTime DateStarted
        {
            get => _dateStarted;
            set
            {
                if (!this.Set(ref _dateStarted, value)) return;
                RaisePropertyChanged("TimeStarted");
                RaisePropertyChanged("Duration");
            }
        }
        public DateTime DateEnded
        {
            get => _dateEnded;
            set
            {
                if (!this.Set(ref _dateEnded, value)) return;
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
        public TimeSpan Total
        {
            get => _total;
            set => Set(ref _total, value);
        }
        public bool IsPersonalType
        {
            get
            {
                if (string.IsNullOrEmpty(this.Type))
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
        private void FixTitles(TaskViewModel subTask, int ii)
        {
            var getTitle1 = (Func<int, TaskViewModel, string>)((i, t) => t.DateStarted.ToString("yyyy"));
            var getTitle2 = (Func<int, TaskViewModel, string>)((i, t) => t.DateStarted.ToString("MMMM"));
            var getTitle3 = (Func<int, TaskViewModel, string>)((i, t) => "Week" + (i + 1));
            var getTitle4 = (Func<int, TaskViewModel, string>)((i, t) => t.DateStarted.DayOfWeek.ToString());
            FixTitles("Time Tracker", getTitle1, subTask, ii);
            FixTitles("Year", getTitle2, subTask, ii);
            FixTitles("Month", getTitle3, subTask, ii);
            FixTitles("Week", getTitle4, subTask, ii);
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

            Nodes = new ObservableCollection<NodeViewModel>();
            foreach (var subTask in model.SubTasks)
            {
                var node = new TaskViewModel();
                node.LoadFrom(subTask);
                Nodes.Add(node);
            }
        }
        private void FixTypes()
        {
            var type = Type;
            if (string.IsNullOrEmpty(type))
            {
                var title = Name;
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
                ((TaskViewModel)subTask).FixTypes();
        }
        private void FixTitles()
        {
            for (var ii = 0; ii < Nodes.Count; ++ii)
            {
                var subTask = (TaskViewModel)Nodes[ii];
                FixTitles(subTask, ii);
                subTask.FixTitles();
            }
        }

        #endregion

        protected void FixTitles(string parent, Func<int, TaskViewModel, string> getTitle, TaskViewModel subTask, int ii)
        {
            if (Context != parent)
                return;
            subTask.Name = getTitle(ii, subTask);
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
                    var subTask = (TaskViewModel)Nodes[index];
                    if (subTask.DateEnded == DateTime.MinValue && index < Nodes.Count - 1)
                        subTask.DateEnded = ((TaskViewModel)Nodes[index + 1]).DateStarted;
                }

                Total = TimeSpan.Zero;
                for (var index = 0; index < Nodes.Count; ++index)
                {
                    var subTask = (TaskViewModel)Nodes[index];
                    subTask.FixTime();
                    Total += subTask.Total;
                }

                var subTask1 = (TaskViewModel)Nodes[Nodes.Count - 1];
                if (subTask1.DateEnded != DateTime.MinValue)
                    DateEnded = subTask1.DateEnded;
                var subTask2 = (TaskViewModel)Nodes[0];
                if (subTask2.DateStarted != DateTime.MinValue)
                    DateStarted = subTask2.DateStarted;
            }
        }
        public override NodeViewModel AddNew()
        {
            var subTask = new TaskViewModel
            {
                Kind = "Task",
                Name = "New Model", 
                DateStarted = DateTime.Now, 
                DateEnded = DateTime.Now
            };
            Add(subTask);
            var ii = Nodes.IndexOf(subTask);
            FixContext(subTask);
            FixTitles(subTask, ii);
            return subTask;
        }

    }
}