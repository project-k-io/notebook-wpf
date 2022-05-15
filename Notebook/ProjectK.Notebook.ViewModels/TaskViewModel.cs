using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Models;
using ProjectK.Notebook.Models.Interfaces;
using ProjectK.Notebook.ViewModels.Enums;
using ProjectK.Notebook.ViewModels.Extensions;
using System;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace ProjectK.Notebook.ViewModels
{
    public class TaskViewModel : ItemViewModel
    {
        #region Static Fields

        private static readonly ILogger Logger = LogManager.GetLogger<TaskViewModel>();

        #endregion

        #region Fields

        // Main wrappers
        //private string _type;
        //private string _subType;
        //private DateTime _dateStarted;
        //private DateTime _dateEnded;

        // Misc
        private TimeSpan _total;

        #endregion


        private TaskModel Task => Model as TaskModel;

        public string Type
        {
            get => Task.Type;
            set
            {
                if(Task.Type == value)
                    return;

                Task.Type = value;
                OnPropertyChanged();
            }
        }

        public string SubType
        {
            get => Task.SubType;
            set
            {
                if (Task.SubType == value)
                    return;

                Task.SubType = value;
                OnPropertyChanged();
            }
        }

        public DateTime DateStarted
        {
            get => Task.DateStarted;
            set
            {
                if (Task.DateStarted == value)
                    return;

                Task.DateStarted = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(TimeStarted));
                OnPropertyChanged(nameof(Duration));
            }
        }

        public DateTime DateEnded
        {
            get => Task.DateEnded;
            set
            {
                if (Task.DateEnded == value)
                    return;

                Task.DateEnded = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TimeEnded));
                OnPropertyChanged(nameof(Duration));
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
                OnPropertyChanged(); // MC
                OnPropertyChanged(nameof(DateStarted));
                OnPropertyChanged(nameof(Duration));
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
                OnPropertyChanged(); //MC
                OnPropertyChanged(nameof(DateEnded));
                OnPropertyChanged(nameof(Duration));
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

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (!ModelRules.IsTaskModelProperty(e.PropertyName))
                return;

            Logger?.LogDebug($@"[Node] PropertyChanged: {e.PropertyName}");
            Modified = ModifiedStatus.Modified;
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<TaskViewModel>(this));
        }

        #region Properties

        public TaskViewModel()
        {
            Kind = "Task";
        }

        public TaskViewModel(INode model) : this()
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
            set => SetProperty(ref _total, value);
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

        #endregion
    }
}