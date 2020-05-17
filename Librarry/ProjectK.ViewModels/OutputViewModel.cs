﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;

namespace ProjectK.ViewModels
{
    public class OutputViewModel : ViewModelBase
    {
        private readonly OutputButtonViewModel _outputButtonClear = new OutputButtonViewModel { Image = "Clear", Label = "Clear" };
        private readonly OutputButtonViewModel _outputButtonDebug = new OutputButtonViewModel { Image = "Debug", Label = "Logs", IsChecked = true};
        private readonly OutputButtonViewModel _outputButtonErrors = new OutputButtonViewModel{Image = "Error", Label = "Errors", IsChecked = false};
        private readonly OutputButtonViewModel _outputButtonMessages = new OutputButtonViewModel { Image = "Message", Label = "Messages", IsChecked = false};
        private readonly OutputButtonViewModel _outputButtonWarnings = new OutputButtonViewModel { Image = "Warning", Label = "Warnings", IsChecked = false};

        public Action<string, object> SetValue { get; set; }
        public Func<string, string, object> GetValue { get; set; }
        public Action UpdateFilter { get; set; }

        public ObservableCollection<OutputButtonViewModel> FilterButtons { get; } = new ObservableCollection<OutputButtonViewModel>();
        public ObservableCollection<OutputButtonViewModel> CommandButtons { get; } = new ObservableCollection<OutputButtonViewModel>();
        public ObservableCollection<OutputRecordViewModel> Records { get; } = new ObservableCollection<OutputRecordViewModel>();


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

        private void OutputButtonClearOnClicked(object sender, EventArgs eventArgs)
        {
            ClearOutput();
            SaveSettings();
        }

        private void OutputButtRaisePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsChecked")
                return;

            UpdateFilter?.Invoke();
            SaveSettings();
        }

        public bool Filter(object o)
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
        }

        public OutputRecordViewModel AddNewRecord(LoggingEventArgs e)
        {
            switch (e.Level)
            {
                case LogLevel.Information:
                    ++_outputButtonMessages.Count;
                    break;
                case LogLevel.Error:
                    ++_outputButtonErrors.Count;
                    break;
                case LogLevel.Warning:
                    ++_outputButtonWarnings.Count;
                    break;
                case LogLevel.Debug:
                    ++_outputButtonDebug.Count;
                    break;
            }

            var lastRecord = Records.LastOrDefault();
            var now = DateTime.Now;
            var duration = lastRecord != null ? now - lastRecord.Date : new TimeSpan(0);

            var outputRecordViewModel = new OutputRecordViewModel
            {
                Id = Records.Count,
                Type = e.Level,
                Date = now,
                Duration = duration,
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
                if (GetValue != null)
                {
                    _outputButtonErrors.IsChecked = Convert.ToBoolean(GetValue.Invoke("Error", "True"));
                    _outputButtonDebug.IsChecked = Convert.ToBoolean(GetValue("Debug", "True"));
                    _outputButtonMessages.IsChecked = Convert.ToBoolean(GetValue("Info", "True"));
                    _outputButtonWarnings.IsChecked = Convert.ToBoolean(GetValue("Warning", "True"));
                }
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

                SetValue?.Invoke("Error", _outputButtonErrors.IsChecked);
                SetValue?.Invoke("Debug", _outputButtonDebug.IsChecked);
                SetValue?.Invoke("Info", _outputButtonMessages.IsChecked);
                SetValue?.Invoke("Warning", _outputButtonWarnings.IsChecked);
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

        public void LogEvent(LogLevel logLevel, EventId eventId, string message)
        {
            var record =  AddNewRecord(new LoggingEventArgs { Level = logLevel, Message = message });
        }
    }
}