// Decompiled with JetBrains decompiler
// Type: Vibor.Generic.ViewModels.OutputViewModel
// Assembly: Vibor.Generic.ViewModels, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 18134161-73B0-45D8-9612-67C25563536B
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Generic.ViewModels.dll

using Microsoft.Win32;
using MvvmFoundation.Wpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;
using Vibor.Helpers;

namespace Vibor.Generic.ViewModels
{
  public class OutputViewModel : ObservableObject
  {
    private readonly ObservableCollection<OutputButtonViewModel> _commandButtons;
    private readonly ObservableCollection<OutputButtonViewModel> _filterButtons;
    private readonly OutputButtonViewModel _outputButtonClear;
    private readonly OutputButtonViewModel _outputButtonDebug;
    private readonly OutputButtonViewModel _outputButtonErrors;
    private readonly OutputButtonViewModel _outputButtonMessages;
    private readonly OutputButtonViewModel _outputButtonWarnings;
    private readonly ObservableCollection<OutputRecordViewModel> _records;

    public string RegistryPath
    {
      get
      {
        return XApp.AppName + "\\Output";
      }
    }

    public ObservableCollection<OutputButtonViewModel> FilterButtons
    {
      get
      {
        return this._filterButtons;
      }
    }

    public ObservableCollection<OutputButtonViewModel> CommandButtons
    {
      get
      {
        return this._commandButtons;
      }
    }

    public ObservableCollection<OutputRecordViewModel> Records
    {
      get
      {
        return this._records;
      }
    }

    public OutputViewModel()
    {
      base.\u002Ector();
      this._filterButtons.Clear();
      this._filterButtons.Add(this._outputButtonMessages);
      this._filterButtons.Add(this._outputButtonErrors);
      this._filterButtons.Add(this._outputButtonWarnings);
      this._filterButtons.Add(this._outputButtonDebug);
      this._outputButtonClear.Clicked += new EventHandler(this.OutputButtonClearOnClicked);
      this._commandButtons.Clear();
      this._commandButtons.Add(this._outputButtonClear);
      this._outputButtonMessages.PropertyChanged += new PropertyChangedEventHandler(this.OutputButtonPropertyChanged);
      this._outputButtonWarnings.PropertyChanged += new PropertyChangedEventHandler(this.OutputButtonPropertyChanged);
      this._outputButtonDebug.PropertyChanged += new PropertyChangedEventHandler(this.OutputButtonPropertyChanged);
      this._outputButtonErrors.PropertyChanged += new PropertyChangedEventHandler(this.OutputButtonPropertyChanged);
    }

    private void OutputButtonClearOnClicked(object sender, EventArgs eventArgs)
    {
      this.ClearOutput();
      this.SaveSettings();
    }

    private void OutputButtonPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "IsChecked"))
        return;
      this.UpdateFilter();
      this.SaveSettings();
    }

    private void UpdateFilter()
    {
      CollectionViewSource.GetDefaultView((object) this._records).Filter = (Predicate<object>) (o =>
      {
        OutputRecordViewModel outputRecordViewModel = o as OutputRecordViewModel;
        if (outputRecordViewModel != null)
        {
          if (outputRecordViewModel.Type == Level.Error)
            return this._outputButtonErrors.IsChecked;
          if (outputRecordViewModel.Type == Level.Info)
            return this._outputButtonMessages.IsChecked;
          if (outputRecordViewModel.Type == Level.Warn)
            return this._outputButtonWarnings.IsChecked;
          if (outputRecordViewModel.Type == Level.Debug)
            return this._outputButtonDebug.IsChecked;
        }
        return false;
      });
      this.SaveSettings();
    }

    public void Init()
    {
      this.ReadSettings();
    }

    public OutputRecordViewModel AddNewRecord(LoggingEventArgs e)
    {
      if (e.Level == Level.Info)
        ++this._outputButtonMessages.Count;
      else if (e.Level == Level.Error)
        ++this._outputButtonErrors.Count;
      else if (e.Level == Level.Warn)
        ++this._outputButtonWarnings.Count;
      else if (e.Level == Level.Debug)
        ++this._outputButtonDebug.Count;
      OutputRecordViewModel outputRecordViewModel = new OutputRecordViewModel() { ID = this._records.Count, Type = e.Level, Date = DateTime.Now, State = e.State, Message = e.Message };
      this._records.Add(outputRecordViewModel);
      return outputRecordViewModel;
    }

    public bool ReadSettings()
    {
      try
      {
        RegistryKey subKey = Registry.CurrentUser.CreateSubKey(this.RegistryPath);
        if (subKey == null)
          return false;
        this._outputButtonErrors.IsChecked = Convert.ToBoolean(subKey.GetValue("Error", (object) "True"));
        this._outputButtonDebug.IsChecked = Convert.ToBoolean(subKey.GetValue("Debug", (object) "True"));
        this._outputButtonMessages.IsChecked = Convert.ToBoolean(subKey.GetValue("Info", (object) "True"));
        this._outputButtonWarnings.IsChecked = Convert.ToBoolean(subKey.GetValue("Warning", (object) "True"));
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
        return false;
      }
      return true;
    }

    public bool SaveSettings()
    {
      try
      {
        RegistryKey subKey = Registry.CurrentUser.CreateSubKey(this.RegistryPath);
        if (subKey == null)
          return false;
        subKey.SetValue("Error", (object) this._outputButtonErrors.IsChecked);
        subKey.SetValue("Debug", (object) this._outputButtonDebug.IsChecked);
        subKey.SetValue("Info", (object) this._outputButtonMessages.IsChecked);
        subKey.SetValue("Warning", (object) this._outputButtonWarnings.IsChecked);
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
        return false;
      }
      return true;
    }

    private void ClearOutput()
    {
      this._records.Clear();
      this._outputButtonErrors.Count = 0;
      this._outputButtonDebug.Count = 0;
      this._outputButtonMessages.Count = 0;
      this._outputButtonWarnings.Count = 0;
    }
  }
}
