// Decompiled with JetBrains decompiler
// Type: Projects.Views.TasksListView
// Assembly: Projects.Views, Version=1.1.8.29122, Culture=neutral, PublicKeyToken=null
// MVID: BCB19E50-AB69-4CA9-9CF4-1A9C4DEAF8F2
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.Views.dll

using Projects.ViewModels;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using Vibor.View.Helpers.Misc;

namespace Projects.Views
{
  public partial class TasksListView : UserControl, IComponentConnector
  {
    private readonly ListViewSorterHelper _helper = new ListViewSorterHelper();

    public TasksListView()
    {
      this.InitializeComponent();
      this.Loaded += new RoutedEventHandler(this.TasksListView_Loaded);
    }

    private void TasksListView_Loaded(object sender, RoutedEventArgs e)
    {
      MainViewModel dataContext = this.DataContext as MainViewModel;
      if (dataContext == null)
        return;
      this.listViewTasks.SelectedItem =  dataContext.Project.SelectedTask;
    }

    private void buttonTest_Click(object sender, RoutedEventArgs e)
    {
    }

    private void listViewTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
    }

    private void ListViewTasks_OnClick(object sender, RoutedEventArgs e)
    {
      this._helper.Clicked((FrameworkElement) this, sender, e);
    }
  }
}
