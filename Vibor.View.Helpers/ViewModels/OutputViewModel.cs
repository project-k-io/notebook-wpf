using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using ProjectK.Utils;

namespace ProjectK.View.Helpers.ViewModels
{
    public class OutputViewModel : ViewModelBase
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
            _outputButtonMessages.PropertyChanged += OutputButtRaisePropertyChanged;
            _outputButtonWarnings.PropertyChanged += OutputButtRaisePropertyChanged;
            _outputButtonDebug.PropertyChanged += OutputButtRaisePropertyChanged;
            _outputButtonErrors.PropertyChanged += OutputButtRaisePropertyChanged;
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

        private void OutputButtRaisePropertyChanged(object sender, PropertyChangedEventArgs e)
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
                if (o is OutputRecordViewModel outputRecordViewModel)
                {
                    if (outputRecordViewModel.Type == LogLevel.Error)
                        return _outputButtonErrors.IsChecked;
                    if (outputRecordViewModel.Type == LogLevel.Information)
                        return _outputButtonMessages.IsChecked;
                    if (outputRecordViewModel.Type == LogLevel.Warning)
                        return _outputButtonWarnings.IsChecked;
                    if (outputRecordViewModel.Type == LogLevel.Debug)
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
            if (e.Level == LogLevel.Information)
                ++_outputButtonMessages.Count;
            else if (e.Level == LogLevel.Error)
                ++_outputButtonErrors.Count;
            else if (e.Level == LogLevel.Warning)
                ++_outputButtonWarnings.Count;
            else if (e.Level == LogLevel.Debug)
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