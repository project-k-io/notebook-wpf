using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ProjectK.Notebook.ViewModels;
using ProjectK.Notebook.ViewModels.Enums;
using ProjectK.Notebook.Views.Controls.TreeViewList;
using ProjectK.View.Helpers;
using ProjectK.View.Helpers.Misc;

namespace ProjectK.Notebook.Views
{
    public partial class TasksTreeView : UserControl
    {
        public TasksTreeView()
        {
            InitializeComponent();
            Loaded += TasksTreeView_Loaded;
        }

        private static KeyboardStates KeyboardState
        {
            get
            {
                var keyboardStates = KeyboardStates.None;
                if (XKeyboard.IsCtrlShiftPressed)
                    keyboardStates = KeyboardStates.IsCtrlShiftPressed;
                else if (XKeyboard.IsShiftPressed)
                    keyboardStates = KeyboardStates.IsShiftPressed;
                else if (XKeyboard.IsControlPressed)
                    keyboardStates = KeyboardStates.IsControlPressed;
                return keyboardStates;
            }
        }

        private void TasksTreeView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is MainViewModel dataContext))
                return;
            dataContext.RootTask.SetParents();
            TreeViewTasks.SelectItem(dataContext.Project.SelectedTreeTask);
            TreeViewTasks.PreviewKeyDown += TreeViewTasksOnPreviewKeyDown;
        }

        private void TreeViewTasksOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            OnTreeViewKeyDown(sender, e);
        }

        private void OnTreeViewKeyDown(object sender, KeyEventArgs e)
        {
            var keyState = GetKeyState(e.Key);
            Debug.WriteLine("treeViewTasks_KeyDown");
            if (!(sender is TreeListView treeView))
                return;
            if (!(treeView.DataContext is MainViewModel dataContext))
                return;
            if (!(treeView.SelectedItem is TaskViewModel task))
                task = dataContext.RootTask;

            void ExpandItem(TaskViewModel t)
            {
                if (!(treeView.ItemContainerGenerator.ContainerFromItem(task) is TreeViewItem treeViewItem))
                    return;

                treeViewItem.IsExpanded = true;
            }

            static bool DeleteMessageBox()
            {
                return MessageBox.Show("Are you sure you would like to delete this?", "Delete", MessageBoxButton.OKCancel) ==
                       MessageBoxResult.Cancel;
            }

            var addDelegate = ViewLib.GetAddDelegate(this);
            task.KeyboardAction(keyState, () => KeyboardState, () => e.Handled = true, treeView.SelectItem, ExpandItem, DeleteMessageBox, addDelegate);
        }

        private static KeyboardKeys GetKeyState(Key key)
        {
            var keyStates = KeyboardKeys.None;
            switch (key)
            {
                case Key.Left:
                    keyStates = KeyboardKeys.Left;
                    break;
                case Key.Up:
                    keyStates = KeyboardKeys.Up;
                    break;
                case Key.Right:
                    keyStates = KeyboardKeys.Right;
                    break;
                case Key.Down:
                    keyStates = KeyboardKeys.Down;
                    break;
                case Key.Insert:
                    keyStates = KeyboardKeys.Insert;
                    break;
                case Key.Delete:
                    keyStates = KeyboardKeys.Delete;
                    break;
            }

            return keyStates;
        }

        private void TreeViewTasks_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!(sender is TreeListView treeListView))
                return;
            if (!(treeListView.DataContext is MainViewModel dataContext))
                return;
            var task = treeListView.SelectedItem as TaskViewModel ?? dataContext.RootTask;
            dataContext.SelectTreeTask(task);
        }
    }
}