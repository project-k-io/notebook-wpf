using System.Windows;
using System.Windows.Controls;
using ProjectK.Notebook.ViewModels;
using ProjectK.View.Helpers.Extensions;

namespace ProjectK.Notebook.Views
{
    /// <summary>
    ///     Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            Loaded += MainView_Loaded;
        }

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is MainViewModel model)) return;
            model.OnDispatcher = this.GetAddDelegate();
        }

        private void Calendar_OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(DataContext is MainViewModel model)) return;
            if (!(sender is Calendar calendar)) return;
            model.UpdateSelectDayTasks(calendar.SelectedDates);
            model.OnGenerateReportChanged();
        }
    }
}