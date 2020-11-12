using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ProjectK.Notebook.ViewModels;
using ProjectK.Notebook.ViewModels.Enums;
using ProjectK.Notebook.ViewModels.Extensions;
using ProjectK.Notebook.ViewModels.Services;
using ProjectK.Notebook.Views.Helpers;
using ProjectK.View.Helpers;
using ProjectK.View.Helpers.Extensions;
using ProjectK.View.Helpers.Misc;
using ProjectK.Views.TreeViewList;

namespace ProjectK.Notebook.Views
{
    public partial class TasksTreeView : UserControl
    {
        public TasksTreeView()
        {
            InitializeComponent();
            Loaded += TasksTreeView_Loaded;
        }

        private void TasksTreeView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is MainViewModel model))
                return;

            model.RootTask.SetParents();
            TreeViewTasks.SelectItem(model.SelectedNotebook?.SelectedTreeNode);
            TreeViewTasks.PreviewKeyDown += TreeViewTasksOnPreviewKeyDown;
        }

        static bool DeleteMessageBox()
        {
            return MessageBox.Show("Are you sure you would like to delete this?", "Delete", MessageBoxButton.OKCancel) ==
                   MessageBoxResult.Cancel;
        }

        private void TreeViewTasksOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var keyState = KeyboardHelper.GetKeyState(e.Key);
            if (!(sender is TreeListView treeView))
                return;

            if (!(treeView.DataContext is MainViewModel mainViewModel))
                return;

            if (!(treeView.SelectedItem is NodeViewModel task))
                task = mainViewModel.RootTask;

            void ExpandItem(NodeViewModel t)
            {
                if (!(treeView.ItemContainerGenerator.ContainerFromItem(task) is TreeViewItem treeViewItem))
                    return;

                treeViewItem.IsExpanded = true;
            }

            var addDelegate = ViewLib.GetAddDelegate(this);

            var service = new ActionService
            {
                GetState = () => KeyboardHelper.KeyboardState,
                Handled = () => e.Handled = true,
                SelectItem = treeView.SelectItem,
                ExpandItem = (a) => ExpandItem((NodeViewModel)a),
                DeleteMessageBox = DeleteMessageBox,
                Dispatcher = addDelegate
            };

            NodeViewModel.KeyboardAction(task, keyState, service);
        }


        private void TreeViewTasks_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!(sender is TreeListView treeListView))
                return;

            if (!(treeListView.DataContext is MainViewModel model))
                return;

            var task = treeListView.SelectedItem as NodeViewModel ?? model.RootTask;
            model.SelectTreeTask(task);
        }
    }
}