using System;
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
        public OutputButtonViewModel OutputButtonClear { get; set; } = new OutputButtonViewModel {Image = "Clear", Label = "Clear"};
        public OutputButtonViewModel OutputButtonDebug { get; set; } = new OutputButtonViewModel {Image = "Debug", Label = "Logs", IsChecked = true};
        public OutputButtonViewModel OutputButtonErrors { get; set; } = new OutputButtonViewModel {Image = "Error", Label = "Errors", IsChecked = false};
        public OutputButtonViewModel OutputButtonMessages { get; set; } = new OutputButtonViewModel {Image = "Message", Label = "Messages", IsChecked = false};
        public OutputButtonViewModel OutputButtonWarnings { get; set; } = new OutputButtonViewModel {Image = "Warning", Label = "Warnings", IsChecked = false};


        public OutputViewModel()
        {
            FilterButtons.Clear();
            FilterButtons.Add(OutputButtonMessages);
            FilterButtons.Add(OutputButtonErrors);
            FilterButtons.Add(OutputButtonWarnings);
            FilterButtons.Add(OutputButtonDebug);
            OutputButtonClear.Clicked += OutputButtonClearOnClicked;
            CommandButtons.Clear();
            CommandButtons.Add(OutputButtonClear);
            OutputButtonMessages.PropertyChanged += OutputButtonRaisePropertyChanged;
            OutputButtonWarnings.PropertyChanged += OutputButtonRaisePropertyChanged;
            OutputButtonDebug.PropertyChanged += OutputButtonRaisePropertyChanged;
            OutputButtonErrors.PropertyChanged += OutputButtonRaisePropertyChanged;
        }

        public Action UpdateFilter { get; set; }

        public ObservableCollection<OutputButtonViewModel> FilterButtons { get; } = new ObservableCollection<OutputButtonViewModel>();
        public ObservableCollection<OutputButtonViewModel> CommandButtons { get; } = new ObservableCollection<OutputButtonViewModel>();
        public ObservableCollection<OutputRecordViewModel> Records { get; } = new ObservableCollection<OutputRecordViewModel>();

        private void OutputButtonClearOnClicked(object sender, EventArgs eventArgs)
        {
            ClearOutput();
        }

        private void OutputButtonRaisePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsChecked")
                return;

            UpdateFilter?.Invoke();
        }

        public bool Filter(object o)
        {
            if (o is OutputRecordViewModel outputRecordViewModel)
            {
                if (outputRecordViewModel.Type == LogLevel.Error)
                    return OutputButtonErrors.IsChecked;
                if (outputRecordViewModel.Type == LogLevel.Information)
                    return OutputButtonMessages.IsChecked;
                if (outputRecordViewModel.Type == LogLevel.Warning)
                    return OutputButtonWarnings.IsChecked;
                if (outputRecordViewModel.Type == LogLevel.Debug)
                    return OutputButtonDebug.IsChecked;
            }

            return false;
        }

        public OutputRecordViewModel AddNewRecord(LoggingEventArgs e)
        {
            switch (e.Level)
            {
                case LogLevel.Information:
                    ++OutputButtonMessages.Count;
                    break;
                case LogLevel.Error:
                    ++OutputButtonErrors.Count;
                    break;
                case LogLevel.Warning:
                    ++OutputButtonWarnings.Count;
                    break;
                case LogLevel.Debug:
                    ++OutputButtonDebug.Count;
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

        private void ClearOutput()
        {
            Records.Clear();
            OutputButtonErrors.Count = 0;
            OutputButtonDebug.Count = 0;
            OutputButtonMessages.Count = 0;
            OutputButtonWarnings.Count = 0;
        }

        public void LogEvent(LogLevel logLevel, EventId eventId, string message)
        {
            var record = AddNewRecord(new LoggingEventArgs {Level = logLevel, Message = message});
        }
    }
}