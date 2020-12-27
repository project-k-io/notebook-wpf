using System.Windows;
using System.Windows.Controls;
using ProjectK.Notebook.ViewModels;
using ProjectK.View.Helpers.Misc;

namespace ProjectK.Notebook.Views
{
    public partial class NodeListView : UserControl
    {
        private readonly ListViewSorterHelper _sorterHelper = new ListViewSorterHelper();

        public NodeListView()
        {
            InitializeComponent();
            Loaded += TasksListView_Loaded;
        }

        private void TasksListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is MainViewModel model))
                return;

            ListViewTasks.SelectedItem = model.SelectedNode;
        }


        private void ListViewTasks_OnClick(object sender, RoutedEventArgs e)
        {
            _sorterHelper.Clicked(this, sender, e);
        }
    }
}