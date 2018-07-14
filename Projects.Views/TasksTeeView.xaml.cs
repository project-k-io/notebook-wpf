// Decompiled with JetBrains decompiler
// Type: Projects.Views.TasksTreeView
// Assembly: Projects.Views, Version=1.1.8.29122, Culture=neutral, PublicKeyToken=null
// MVID: BCB19E50-AB69-4CA9-9CF4-1A9C4DEAF8F2
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.Views.dll

using Projects.ViewModels;
using Projects.Views.Controls.TreeViewList;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using Vibor.View.Helpers;
using Vibor.View.Helpers.Misc;

namespace Projects.Views
{
  public partial class TasksTreeView : UserControl, IComponentConnector
  {
    private MainViewModel _main = new MainViewModel();

    public TasksTreeView()
    {
      this.InitializeComponent();
      this.Loaded += new RoutedEventHandler(this.TasksTreeView_Loaded);
    }

    private void TasksTreeView_Loaded(object sender, RoutedEventArgs e)
    {
      MainViewModel dataContext = this.DataContext as MainViewModel;
      if (dataContext == null)
        return;
      dataContext.RootTask.SetParents();
      this.treeViewTasks.SelectItem( dataContext.Project.SelectedTreeTask);
      this.treeViewTasks.PreviewKeyDown += new KeyEventHandler(this.TreeViewTasksOnPreviewKeyDown);
    }

    private void TreeViewTasksOnPreviewKeyDown(object sender, KeyEventArgs e)
    {
      this.OnTreeViewKeyDown(sender, e);
    }

    private void treeViewTasks_KeyDown(object sender, KeyEventArgs e)
    {
    }

    private void OnTreeViewKeyDown(object sender, KeyEventArgs e)
    {
      TaskViewModel.KeyStates keyState = TasksTreeView.GetKeyState(e.Key);
      Debug.WriteLine("treeViewTasks_KeyDown");
      TreeListView treeView = sender as TreeListView;
      if (treeView == null)
        return;
      MainViewModel dataContext = treeView.DataContext as MainViewModel;
      if (dataContext == null)
        return;
      TaskViewModel task = treeView.SelectedItem as TaskViewModel;
      if (task == null)
        task = dataContext.RootTask;
      Action<TaskViewModel> expandItem = (Action<TaskViewModel>) (t =>
      {
        TreeViewItem treeViewItem = treeView.ItemContainerGenerator.ContainerFromItem( task) as TreeViewItem;
        if (treeViewItem == null)
          return;
        treeViewItem.IsExpanded = true;
      });
      Func<bool> deleteMessageBox = (Func<bool>) (() => MessageBox.Show("Are you sure you would like to delete this?", "Delete", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel);
      Action<Action> addDelegate = ViewLib.GetAddDelegate((FrameworkElement) this, DispatcherPriority.Background);
      TaskViewModel.OnTreeViewKeyDown(task, keyState, (Func<TaskViewModel.KeyboardStates>) (() => TasksTreeView.KeyboardState), (Action) (() => e.Handled = true), new Action<TaskViewModel>(treeView.SelectItem), expandItem, deleteMessageBox, addDelegate);
    }

    private static TaskViewModel.KeyboardStates KeyboardState
    {
      get
      {
        TaskViewModel.KeyboardStates keyboardStates = TaskViewModel.KeyboardStates.None;
        if (XKeyboard.IsCtrlShiftPressed)
          keyboardStates = TaskViewModel.KeyboardStates.IsCtrlShiftPressed;
        else if (XKeyboard.IsShiftPressed)
          keyboardStates = TaskViewModel.KeyboardStates.IsShiftPressed;
        else if (XKeyboard.IsControlPressed)
          keyboardStates = TaskViewModel.KeyboardStates.IsControlPressed;
        return keyboardStates;
      }
    }

    private static TaskViewModel.KeyStates GetKeyState(Key key)
    {
      TaskViewModel.KeyStates keyStates = TaskViewModel.KeyStates.None;
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
      TreeListView treeListView = sender as TreeListView;
      if (treeListView == null)
        return;
      MainViewModel dataContext = treeListView.DataContext as MainViewModel;
      if (dataContext == null)
        return;
      TaskViewModel task = treeListView.SelectedItem as TaskViewModel ?? dataContext.RootTask;
      dataContext.SelectTreeTask(task);
    }
  }
}
