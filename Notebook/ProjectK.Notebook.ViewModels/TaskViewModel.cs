using System;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Models;
using ProjectK.Notebook.Models.Interfaces;
using ProjectK.Notebook.ViewModels.Enums;
using ProjectK.Notebook.ViewModels.Extensions;
using ProjectK.Utils.Extensions;

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
            set => Set(ref _total, value);
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


        public override void RaisePropertyChanged<T>(string propertyName = null, T oldValue = default, T newValue = default, bool broadcast = false)
        {
            base.RaisePropertyChanged(propertyName, oldValue, newValue, broadcast);

            if (!ModelRules.IsTaskModelProperty(propertyName)) 
                return;

            Logger?.LogDebug($@"[Node] PropertyChanged: {propertyName} | {oldValue} | {newValue}");
            Modified = ModifiedStatus.Modified;
            MessengerInstance.Send(new NotificationMessage<TaskViewModel>(this, "Modified"));
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
                RaisePropertyChanged(nameof(TimeStarted));
                RaisePropertyChanged(nameof(Duration));
            }
        }

        public DateTime DateEnded
        {
            get => Task.DateEnded;
            set
            {
                if (!this.Set(DateEnded, v => Task.DateEnded = v, value)) return;
                RaisePropertyChanged(nameof(TimeEnded));
                RaisePropertyChanged(nameof(Duration));
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
                RaisePropertyChanged(nameof(DateStarted));
                RaisePropertyChanged(nameof(Duration));
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
                RaisePropertyChanged(nameof(DateEnded));
                RaisePropertyChanged(nameof(Duration));
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