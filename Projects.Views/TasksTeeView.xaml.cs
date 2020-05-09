using Projects.ViewModels;
using Projects.Views.Controls.TreeViewList;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Vibor.View.Helpers;
using Vibor.View.Helpers.Misc;

namespace Projects.Views
{
    public partial class TasksTreeView : UserControl, IComponentConnector
    {
        public TasksTreeView()
        {
            InitializeComponent();
            Loaded += TasksTreeView_Loaded;
        }

        private static TaskViewModel.KeyboardStates KeyboardState
        {
            get
            {
                var keyboardStates = TaskViewModel.KeyboardStates.None;
                if (XKeyboard.IsCtrlShiftPressed)
                    keyboardStates = TaskViewModel.KeyboardStates.IsCtrlShiftPressed;
                else if (XKeyboard.IsShiftPressed)
                    keyboardStates = TaskViewModel.KeyboardStates.IsShiftPressed;
                else if (XKeyboard.IsControlPressed)
                    keyboardStates = TaskViewModel.KeyboardStates.IsControlPressed;
                return keyboardStates;
            }
        }

        private void TasksTreeView_Loaded(object sender, RoutedEventArgs e)
        {
            var dataContext = DataContext as MainViewModel;
            if (dataContext == null)
                return;
            dataContext.RootTask.SetParents();
            treeViewTasks.SelectItem(dataContext.Project.SelectedTreeTask);
            treeViewTasks.PreviewKeyDown += TreeViewTasksOnPreviewKeyDown;
        }

        private void TreeViewTasksOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            OnTreeViewKeyDown(sender, e);
        }

        private void treeViewTasks_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void OnTreeViewKeyDown(object sender, KeyEventArgs e)
        {
            var keyState = GetKeyState(e.Key);
            Debug.WriteLine("treeViewTasks_KeyDown");
            var treeView = sender as TreeListView;
            if (treeView == null)
                return;
            var dataContext = treeView.DataContext as MainViewModel;
            if (dataContext == null)
                return;
            var task = treeView.SelectedItem as TaskViewModel;
            if (task == null)
                task = dataContext.RootTask;
            var expandItem = (Action<TaskViewModel>)(t =>
           {
               var treeViewItem = treeView.ItemContainerGenerator.ContainerFromItem(task) as TreeViewItem;
               if (treeViewItem == null)
                   return;
               treeViewItem.IsExpanded = true;
           });
            var deleteMessageBox = (Func<bool>)(() =>
               MessageBox.Show("Are you sure you would like to delete this?", "Delete", MessageBoxButton.OKCancel) ==
               MessageBoxResult.Cancel);
            var addDelegate = ViewLib.GetAddDelegate(this);
            TaskViewModel.OnTreeViewKeyDown(task, keyState, () => KeyboardState, () => e.Handled = true,
                treeView.SelectItem, expandItem, deleteMessageBox, addDelegate);
        }

        private static TaskViewModel.KeyStates GetKeyState(Key key)
        {
            var keyStates = TaskViewModel.KeyStates.None;
            switch (key)
            {
                case Key.Left:
                    keyStates = TaskViewModel.KeyStates.Left;
                    break;
                case Key.Up:
                    keyStates = TaskViewModel.KeyStates.Up;
                    break;
                case Key.Right:
                    keyStates = TaskViewModel.KeyStates.Right;
                    break;
                case Key.Down:
                    keyStates = TaskViewModel.KeyStates.Down;
                    break;
                case Key.Insert:
                    keyStates = TaskViewModel.KeyStates.Insert;
                    break;
                case Key.Delete:
                    keyStates = TaskViewModel.KeyStates.Delete;
                    break;
            }

            return keyStates;
        }

        private void treeViewTasks_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeListView = sender as TreeListView;
            if (treeListView == null)
                return;
            var dataContext = treeListView.DataContext as MainViewModel;
            if (dataContext == null)
                return;
            var task = treeListView.SelectedItem as TaskViewModel ?? dataContext.RootTask;
            dataContext.SelectTreeTask(task);
        }
    }
}