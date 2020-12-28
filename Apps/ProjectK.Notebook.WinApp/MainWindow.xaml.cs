using System.Windows;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectK.Logging;
using ProjectK.Notebook.WinApp.Settings;

namespace ProjectK.Notebook.WinApp
{
    public partial class MainWindow : Window
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
            CommandBindings.AddRange(model.CreateCommandBindings());
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