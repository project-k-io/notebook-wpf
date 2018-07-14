// Decompiled with JetBrains decompiler
// Type: Projects.ViewModels.LayoutViewModel
// Assembly: Projects.ViewModels, Version=1.1.8.29121, Culture=neutral, PublicKeyToken=null
// MVID: AA177939-1C69-401F-8524-6C17EE86E3CA
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.ViewModels.dll

using Vibor.Mvvm;

namespace Projects.ViewModels
{
    public class LayoutViewModel : BaseViewModel
    {
        private int _navigatorWidth;

        public int NavigatorWidth
        {
            get => _navigatorWidth;
            set
            {
                if (_navigatorWidth == value)
                    return;
                _navigatorWidth = value;
                OnPropertyChanged(nameof(NavigatorWidth));
            }
        }
    }
}