using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using ProjectK.Logging;
using ProjectK.Notebook.ViewModels;
using ProjectK.Utils;
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