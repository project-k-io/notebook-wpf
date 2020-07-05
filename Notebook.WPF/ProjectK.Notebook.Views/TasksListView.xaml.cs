using System.Windows;
using System.Windows.Controls;
using ProjectK.Notebook.ViewModels;
using ProjectK.View.Helpers.Misc;

namespace ProjectK.Notebook.Views
{
    public partial class TasksListView : UserControl
    {
        private readonly ListViewSorterHelper _sorterHelper = new ListViewSorterHelper();

        public TasksListView()
        {
            InitializeComponent();
            Loaded += TasksListView_Loaded;
        }

        private void TasksListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is MainViewModel model))
                return;

            listViewTasks.SelectedItem = model.Notebook.SelectedTask;
        }


        private void ListViewTasks_OnClick(object sender, RoutedEventArgs e)
        {
            _sorterHelper.Clicked(this, sender, e);
        }
    }
}