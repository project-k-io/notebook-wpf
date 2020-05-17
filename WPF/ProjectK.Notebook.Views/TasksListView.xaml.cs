using System.Windows;
using System.Windows.Controls;
using ProjectK.Notebook.ViewModels;
using ProjectK.View.Helpers.Misc;

namespace ProjectK.Notebook.Views
{
    public partial class TasksListView : UserControl
    {
        private readonly ListViewSorterHelper _helper = new ListViewSorterHelper();

        public TasksListView()
        {
            InitializeComponent();
            Loaded += TasksListView_Loaded;
        }

        private void TasksListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is DomainViewModel dataContext))
                return;
            listViewTasks.SelectedItem = dataContext.Notebook.SelectedTask;
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ListViewTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ListViewTasks_OnClick(object sender, RoutedEventArgs e)
        {
            _helper.Clicked(this, sender, e);
        }
    }
}