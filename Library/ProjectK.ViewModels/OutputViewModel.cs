using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using ProjectK.Extensions.Logging;

namespace ProjectK.ToolKit.ViewModels;

public class OutputViewModel : ObservableObject
{
    public OutputViewModel()
    {
        FilterButtons.Clear();
        FilterButtons.Add(ButtonMessages);
        FilterButtons.Add(ButtonErrors);
        FilterButtons.Add(ButtonWarnings);
        FilterButtons.Add(ButtonDebug);
        ButtonClear.Clicked += OutputButtonClearOnClicked;
        CommandButtons.Clear();
        CommandButtons.Add(ButtonClear);
        ButtonMessages.PropertyChanged += OutputButtonRaisePropertyChanged;
        ButtonWarnings.PropertyChanged += OutputButtonRaisePropertyChanged;
        ButtonDebug.PropertyChanged += OutputButtonRaisePropertyChanged;
        ButtonErrors.PropertyChanged += OutputButtonRaisePropertyChanged;
    }

    public OutputButtonViewModel ButtonClear { get; set; } =
        new() { Image = "Clear", Label = "Clear" };

    public OutputButtonViewModel ButtonDebug { get; set; } = new() { Image = "Debug", Label = "Logs", IsChecked = true };

    public OutputButtonViewModel ButtonErrors { get; set; } = new() { Image = "Error", Label = "Errors", IsChecked = false };

    public OutputButtonViewModel ButtonMessages { get; set; } = new() { Image = "Message", Label = "Messages", IsChecked = false };

    public OutputButtonViewModel ButtonWarnings { get; set; } = new() { Image = "Warning", Label = "Warnings", IsChecked = false };

    public Action UpdateFilter { get; set; }

    public ObservableCollection<OutputButtonViewModel> FilterButtons { get; } =
        new();

    public ObservableCollection<OutputButtonViewModel> CommandButtons { get; } =
        new();

    public ObservableCollection<OutputRecordViewModel> Records { get; } =
        new();

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
                return ButtonErrors.IsChecked;
            if (outputRecordViewModel.Type == LogLevel.Information)
                return ButtonMessages.IsChecked;
            if (outputRecordViewModel.Type == LogLevel.Warning)
                return ButtonWarnings.IsChecked;
            if (outputRecordViewModel.Type == LogLevel.Debug)
                return ButtonDebug.IsChecked;
        }

        return false;
    }

    public OutputRecordViewModel AddNewRecord(LoggingEventArgs e)
    {
        switch (e.Level)
        {
            case LogLevel.Information:
                ++ButtonMessages.Count;
                break;
            case LogLevel.Error:
                ++ButtonErrors.Count;
                break;
            case LogLevel.Warning:
                ++ButtonWarnings.Count;
                break;
            case LogLevel.Debug:
                ++ButtonDebug.Count;
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
        ButtonErrors.Count = 0;
        ButtonDebug.Count = 0;
        ButtonMessages.Count = 0;
        ButtonWarnings.Count = 0;
    }

    public void LogEvent(LogLevel logLevel, EventId eventId, string message)
    {
        AddNewRecord(new LoggingEventArgs { Level = logLevel, EventId = eventId, Message = message });
    }
}