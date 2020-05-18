using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;

namespace ProjectK.Notebook.ViewModels
{
    public static class ViewModelExtensions
    {
        private static readonly ILogger Logger = LogManager.GetLogger<DomainViewModel>();

        public static bool Set<T>(this ViewModelBase m, T oldValue, Action<T> setValue, T newValue, bool broadcast = false, [CallerMemberName] string propertyName = null)
        {
            try
            {
                if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
                    return false;

                setValue(newValue);

                m.RaisePropertyChanged(propertyName, oldValue, newValue, broadcast);
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                return false;
            }
        }
    }
}