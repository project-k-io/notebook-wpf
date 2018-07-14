// Decompiled with JetBrains decompiler
// Type: Vibor.Generic.ViewModels.OutputButtonViewModel
// Assembly: Vibor.Generic.ViewModels, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 18134161-73B0-45D8-9612-67C25563536B
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Generic.ViewModels.dll

using System;
using System.Windows.Input;
using Vibor.Mvvm;

namespace Vibor.Generic.ViewModels
{
  public class OutputButtonViewModel : BaseViewModel
  {
    private bool _isChecked = true;
    private bool _isCountVisible = true;
    private int _count;

    public bool IsChecked
    {
      get
      {
        return this._isChecked;
      }
      set
      {
        if (this._isChecked == value)
          return;
        this._isChecked = value;
        this.OnPropertyChanged(nameof (IsChecked));
      }
    }

    public int Image { get; set; }

    public int Count
    {
      get
      {
        return this._count;
      }
      set
      {
        if (this._count == value)
          return;
        this._count = value;
        this.OnPropertyChanged(nameof (Count));
      }
    }

    public string Label { get; set; }

    public bool IsCountVisible
    {
      get
      {
        return this._isCountVisible;
      }
      set
      {
        this._isCountVisible = value;
      }
    }

    public ICommand ClickedCommand
    {
      get
      {
        return (ICommand) new RelayCommand(new Action(this.OnClicked), (Predicate<object>) null);
      }
    }

    public event EventHandler Clicked = null;

    private void OnClicked()
    {
      EventHandler clicked = this.Clicked;
      if (clicked == null)
        return;
      clicked( this, EventArgs.Empty);
    }
  }
}
