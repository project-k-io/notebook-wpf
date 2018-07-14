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
    internal ListView listViewTasks;
    internal Button buttonTest;
    private bool _contentLoaded;

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
      this.listViewTasks.SelectedItem = (object) dataContext.Project.SelectedTask;
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

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Projects.Views;component/taskslistview.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.listViewTasks = (ListView) target;
          this.listViewTasks.SelectionChanged += new SelectionChangedEventHandler(this.listViewTasks_SelectionChanged);
          this.listViewTasks.AddHandler(ButtonBase.ClickEvent, (Delegate) new RoutedEventHandler(this.ListViewTasks_OnClick));
          break;
        case 2:
          this.buttonTest = (Button) target;
          this.buttonTest.Click += new RoutedEventHandler(this.buttonTest_Click);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
