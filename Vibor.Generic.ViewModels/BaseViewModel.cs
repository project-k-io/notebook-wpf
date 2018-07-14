// Decompiled with JetBrains decompiler
// Type: Vibor.Generic.ViewModels.BaseViewModel
// Assembly: Vibor.Generic.ViewModels, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 18134161-73B0-45D8-9612-67C25563536B
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Generic.ViewModels.dll

using System;
using System.ComponentModel;

namespace Vibor.Generic.ViewModels
{
  public class BaseViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    public Action<Action> Dispatcher { get; set; }

    protected void OnDispatcher(Action a)
    {
      Action<Action> dispatcher = this.Dispatcher;
      if (dispatcher == null)
        return;
      dispatcher(a);
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
