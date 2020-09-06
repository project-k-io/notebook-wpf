using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Extensions;
using ProjectK.Notebook.ViewModels;
using ProjectK.View.Helpers.Misc;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;

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
            if (!(DataContext is AppViewModel model)) return;

            model.OnDispatcher = ViewLib.GetAddDelegate(this);
            CommandBindings.AddRange(model.CreateCommandBindings());
        }

        private void Calendar_OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(DataContext is AppViewModel model)) return;
            if (!(sender is Calendar calendar)) return;
            model.SelectedNotebook.UpdateSelectDayTasks(calendar.SelectedDates);
            model.OnGenerateReportChanged();
        }




        private void DockingManager_OnDockStateChanged(FrameworkElement sender, DockStateEventArgs e)
        {
            if (e.NewState == DockState.Hidden)
            {
                DockingManager.Children.Remove(sender);
            }
        }
    }
}