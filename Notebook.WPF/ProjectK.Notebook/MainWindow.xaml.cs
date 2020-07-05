using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Extensions;
using ProjectK.Notebook.ViewModels;
using ProjectK.View.Helpers.Misc;

namespace ProjectK.Notebook
{
    public partial class MainWindow : Window
    {
        private readonly ILogger _logger = LogManager.GetLogger<MainWindow>();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainView_Loaded;
        }

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            _logger.LogDebug("Loaded()");
            if (!(DataContext is MainViewModel model)) return;

            model.OnDispatcher = ViewLib.GetAddDelegate(this);
            CommandBindings.AddRange(model.CreateCommandBindings());
        }

        private void Calendar_OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(DataContext is MainViewModel model)) return;
            if (!(sender is Calendar calendar)) return;
            model.Notebook.UpdateSelectDayTasks(calendar.SelectedDates);
            model.OnGenerateReportChanged();
        }
    }
}