// Decompiled with JetBrains decompiler
// Type: Vibor.Generic.ViewModels.OutputViewModel
// Assembly: Vibor.Generic.ViewModels, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 18134161-73B0-45D8-9612-67C25563536B
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Generic.ViewModels.dll

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;
using Microsoft.Win32;
using Vibor.Generic.ViewModels;
using Vibor.Helpers;
using Vibor.Logging;
using Vibor.Mvvm;

namespace Vibor.ViewModels
{
    public class OutputViewModel : BaseViewModel
    {
        private readonly OutputButtonViewModel _outputButtonClear;
        private readonly OutputButtonViewModel _outputButtonDebug;
        private readonly OutputButtonViewModel _outputButtonErrors;
        private readonly OutputButtonViewModel _outputButtonMessages;
        private readonly OutputButtonViewModel _outputButtonWarnings;

        public OutputViewModel()
        {
            FilterButtons.Clear();
            FilterButtons.Add(_outputButtonMessages);
            FilterButtons.Add(_outputButtonErrors);
            FilterButtons.Add(_outputButtonWarnings);
            FilterButtons.Add(_outputButtonDebug);
            _outputButtonClear.Clicked += OutputButtonClearOnClicked;
            CommandButtons.Clear();
            CommandButtons.Add(_outputButtonClear);
            _outputButtonMessages.PropertyChanged += OutputButtonPropertyChanged;
            _outputButtonWarnings.PropertyChanged += OutputButtonPropertyChanged;
            _outputButtonDebug.PropertyChanged += OutputButtonPropertyChanged;
            _outputButtonErrors.PropertyChanged += OutputButtonPropertyChanged;
        }

        public string RegistryPath => XApp.AppName + "\\Output";

        public ObservableCollection<OutputButtonViewModel> FilterButtons { get; }

        public ObservableCollection<OutputButtonViewModel> CommandButtons { get; }

        public ObservableCollection<OutputRecordViewModel> Records { get; }

        private void OutputButtonClearOnClicked(object sender, EventArgs eventArgs)
        {
            ClearOutput();
            SaveSettings();
        }

        private void OutputButtonPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(e.PropertyName == "IsChecked"))
                return;
            UpdateFilter();
            SaveSettings();
        }

        private void UpdateFilter()
        {
            CollectionViewSource.GetDefaultView(Records).Filter = o =>
            {
                var outputRecordViewModel = o as OutputRecordViewModel;
                if (outputRecordViewModel != null)
                {
                    if (outputRecordViewModel.Type == Level.Error)
                        return _outputButtonErrors.IsChecked;
                    if (outputRecordViewModel.Type == Level.Info)
                        return _outputButtonMessages.IsChecked;
                    if (outputRecordViewModel.Type == Level.Warn)
                        return _outputButtonWarnings.IsChecked;
                    if (outputRecordViewModel.Type == Level.Debug)
                        return _outputButtonDebug.IsChecked;
                }

                return false;
            };
            SaveSettings();
        }

        public void Init()
        {
            ReadSettings();
        }

        public OutputRecordViewModel AddNewRecord(LoggingEventArgs e)
        {
            if (e.Level == Level.Info)
                ++_outputButtonMessages.Count;
            else if (e.Level == Level.Error)
                ++_outputButtonErrors.Count;
            else if (e.Level == Level.Warn)
                ++_outputButtonWarnings.Count;
            else if (e.Level == Level.Debug)
                ++_outputButtonDebug.Count;
            var outputRecordViewModel = new OutputRecordViewModel
            {
                ID = Records.Count,
                Type = e.Level,
                Date = DateTime.Now,
                State = e.State,
                Message = e.Message
            };
            Records.Add(outputRecordViewModel);
            return outputRecordViewModel;
        }

        public bool ReadSettings()
        {
            try
            {
                var subKey = Registry.CurrentUser.CreateSubKey(RegistryPath);
                if (subKey == null)
                    return false;
                _outputButtonErrors.IsChecked = Convert.ToBoolean(subKey.GetValue("Error", "True"));
                _outputButtonDebug.IsChecked = Convert.ToBoolean(subKey.GetValue("Debug", "True"));
                _outputButtonMessages.IsChecked = Convert.ToBoolean(subKey.GetValue("Info", "True"));
                _outputButtonWarnings.IsChecked = Convert.ToBoolean(subKey.GetValue("Warning", "True"));
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
                var subKey = Registry.CurrentUser.CreateSubKey(RegistryPath);
                if (subKey == null)
                    return false;
                subKey.SetValue("Error", _outputButtonErrors.IsChecked);
                subKey.SetValue("Debug", _outputButtonDebug.IsChecked);
                subKey.SetValue("Info", _outputButtonMessages.IsChecked);
                subKey.SetValue("Warning", _outputButtonWarnings.IsChecked);
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
            Records.Clear();
            _outputButtonErrors.Count = 0;
            _outputButtonDebug.Count = 0;
            _outputButtonMessages.Count = 0;
            _outputButtonWarnings.Count = 0;
        }
    }
}