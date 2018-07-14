// Decompiled with JetBrains decompiler
// Type: Vibor.Generic.ViewModels.RelayCommand
// Assembly: Vibor.Generic.ViewModels, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 18134161-73B0-45D8-9612-67C25563536B
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Generic.ViewModels.dll

using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Vibor.Generic.ViewModels
{
  public class RelayCommand : ICommand
  {
    private readonly Predicate<object> _canExecute;
    private readonly Action<object> _execute;

    public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
    {
      if (execute == null)
        throw new ArgumentNullException(nameof (execute));
      this._execute = execute;
      this._canExecute = canExecute;
    }

    public RelayCommand(Action execute, Predicate<object> canExecute = null)
    {
      if (execute == null)
        throw new ArgumentNullException(nameof (execute));
      this._execute = (Action<object>) (t => execute());
      this._canExecute = canExecute;
    }

    [DebuggerStepThrough]
    public bool CanExecute(object parameter)
    {
      return this._canExecute == null || this._canExecute(parameter);
    }

    public event EventHandler CanExecuteChanged
    {
      add
      {
        CommandManager.RequerySuggested += value;
      }
      remove
      {
        CommandManager.RequerySuggested -= value;
      }
    }

    public void Execute(object parameter)
    {
      this._execute(parameter);
    }
  }
}
