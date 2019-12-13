using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;

namespace Projects.ViewModels
{
    public static class ViewModelExtensions
    {
        public static bool Set<T>(this ViewModelBase m, T oldValue, Action<T> setValue, T newValue, bool broadcast = false, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
                return false;

            setValue(newValue);

            m.RaisePropertyChanged<T>(propertyName, oldValue, newValue, broadcast);
            return true;
        }

    }
}