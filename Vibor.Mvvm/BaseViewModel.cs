using System;
using System.ComponentModel;

namespace Vibor.Mvvm
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public Action<Action> Dispatcher { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnDispatcher(Action a)
        {
            Dispatcher?.Invoke(a);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}