using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ProjectK.Notebook.ViewModels;
using ProjectK.Notebook.ViewModels.Services;
using ProjectK.Notebook.Views.Helpers;
using ProjectK.View.Helpers.Extensions;
using ProjectK.Views.TreeViewList;

namespace ProjectK.Notebook.Views
{
    public partial class NodeTreeView : UserControl
    {
        public NodeTreeView()
        {
            InitializeComponent();
            Loaded += TasksTreeView_Loaded;
        }

        private void TasksTreeView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is MainViewModel model))
                return;

            model.RootNode.SetParents();
            TreeViewTasks.SelectItem(model.SelectedTreeNode);
            TreeViewTasks.PreviewKeyDown += async (s, e) => await TreeViewTasksOnPreviewKeyDown(s, e);
        }

        private static bool DeleteMessageBox()
        {
            return MessageBox.Show("Are you sure you would like to delete this?", "Delete",
                       MessageBoxButton.OKCancel) ==
                   MessageBoxResult.Cancel;
        }

        private async Task TreeViewTasksOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var keyState = KeyboardHelper.GetKeyState(e.Key);
            if (!(sender is TreeListView treeView))
                return;

            if (!(treeView.DataContext is MainViewModel mainViewModel))
                return;

            if (!(treeView.SelectedItem is NodeViewModel task))
                task = mainViewModel.RootNode;

            void ExpandItem(NodeViewModel t)
            {
                if (!(treeView.ItemContainerGenerator.ContainerFromItem(task) is TreeViewItem treeViewItem))
                    return;

                treeViewItem.IsExpanded = true;
            }

            var addDelegate = this.GetAddDelegate();

            var service = new ActionService
            {
                GetState = () => KeyboardHelper.KeyboardState,
                Handled = () => e.Handled = true,
                SelectItem = treeView.SelectItem,
                ExpandItem = a => ExpandItem(a),
                DeleteMessageBox = DeleteMessageBox,
                Dispatcher = addDelegate
            };

            await mainViewModel.KeyboardAction(task, keyState, service);
        }


        private void TreeViewTasks_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!(sender is TreeListView treeListView))
                return;

            if (!(treeListView.DataContext is MainViewModel model))
                return;

            if(!(treeListView.SelectedItem is NodeViewModel node))
                return;

            model.SelectTreeTask2(node);
        }
    }
}