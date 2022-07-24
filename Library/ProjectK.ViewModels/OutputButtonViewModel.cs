using System;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace ProjectK.ToolKit.ViewModels;

public class OutputButtonViewModel : ObservableObject
{
    private int _count;
    private bool _isChecked = true;

    public bool IsChecked
    {
        get => _isChecked;
        set => SetProperty(ref _isChecked, value);
    }


    public int Count
    {
        get => _count;
        set => SetProperty(ref _count, value);
    }

    public string Label { get; set; }

    public bool IsCountVisible { get; set; } = true;

    public ICommand ClickedCommand => new RelayCommand(OnClicked);


    public string Image { get; set; }

    public event EventHandler Clicked;

    private void OnClicked()
    {
        var clicked = Clicked;
        clicked?.Invoke(this, EventArgs.Empty);
    }
}