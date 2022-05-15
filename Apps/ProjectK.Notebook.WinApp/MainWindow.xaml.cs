using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectK.Logging;
using ProjectK.Notebook.ViewModels;
using ProjectK.Notebook.WinApp.Models;
using ProjectK.Notebook.WinApp.ViewModels;
using ProjectK.View.Helpers.Extensions;

namespace ProjectK.Notebook.WinApp;

public partial class MainWindow : RibbonWindow
{
    private readonly ILogger _logger = LogManager.GetLogger<MainWindow>();
    private readonly AppSettings _settings;

    public MainWindow(IOptions<AppSettings> settings)
    {
        InitializeComponent();
        _settings = settings.Value;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _logger.LogDebug("Loaded()");
        if (!(DataContext is AppViewModel model)) return;

        model.OnDispatcher = this.GetAddDelegate();
        CommandBindings.AddRange(model.CreateCommandBindings());
    }


    private void Calendar_OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!(DataContext is MainViewModel model)) return;
        if (!(sender is Calendar calendar)) return;
        model.UpdateSelectDayTasks(calendar.SelectedDates);
        model.OnGenerateReportChanged();
    }

    public void LoadSettings()
    {
        var settings = _settings.Layout.Window;
        settings.SizeToFit();
        settings.MoveIntoView();

        Top = settings.Top;
        Left = settings.Left;
        Width = settings.Width;
        Height = settings.Height;
        WindowState = settings.WindowState;
    }

    public void SaveSettings()
    {
        var settings = _settings.Layout.Window;

        if (WindowState != WindowState.Minimized)
        {
            settings.Top = Top;
            settings.Left = Left;
            settings.Height = Height;
            settings.Width = Width;
            settings.WindowState = WindowState;
        }
    }
}