using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectK.Logging;
using ProjectK.Notebook.Settings;
using ProjectK.View.Helpers.Extensions;

namespace ProjectK.Notebook
{
    public partial class MainWindow : Window
    {
        private readonly ILogger _logger = LogManager.GetLogger<MainWindow>();
        private readonly AppSettings _settings;

        public MainWindow(IOptions<AppSettings> settings)
        {
            InitializeComponent();
            _settings = settings.Value;

            Loaded += MainView_Loaded;
        }

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            _logger.LogDebug("Loaded()");
            if (!(DataContext is AppViewModel model)) return;
            model.OnDispatcher = this.GetAddDelegate();
            CommandBindings.AddRange(model.CreateCommandBindings());
        }

        private void Calendar_OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(DataContext is AppViewModel model)) return;
            if (!(sender is Calendar calendar)) return;
            model.SelectedNotebook.UpdateSelectDayTasks(calendar.SelectedDates);
            model.OnGenerateReportChanged();
        }

        public void LoadSettings()
        {
            var settings = _settings.Window;
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
            var settings = _settings.Window;

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
}