using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Projects.ViewModels;
using Vibor.View.Helpers.Misc;

namespace Projects.Views
{
    public partial class TasksListView : UserControl, IComponentConnector
    {
        private readonly ListViewSorterHelper _helper = new ListViewSorterHelper();

        public TasksListView()
        {
            InitializeComponent();
            Loaded += TasksListView_Loaded;
        }

        private void TasksListView_Loaded(object sender, RoutedEventArgs e)
        {
            var dataContext = DataContext as MainViewModel;
            if (dataContext == null)
                return;
            listViewTasks.SelectedItem = dataContext.Project.SelectedTask;
        }

        private void buttonTest_Click(object sender, RoutedEventArgs e)
        {
        }

        private void listViewTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ListViewTasks_OnClick(object sender, RoutedEventArgs e)
        {
            _helper.Clicked(this, sender, e);
        }
    }
}