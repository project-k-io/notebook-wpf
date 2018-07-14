// Decompiled with JetBrains decompiler
// Type: ProjectApp.MainWindow
// Assembly: ProjectApp, Version=1.1.8.29131, Culture=neutral, PublicKeyToken=null
// MVID: A7331AD2-AF8A-4A84-BF9D-60C36001D1E0
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\ProjectApp.exe

using Projects.ViewModels;
using Projects.Views;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using Vibor.View.Helpers.Misc;

namespace ProjectApp
{
  public partial class MainWindow : RibbonWindow, IComponentConnector
  {
    internal System.Windows.Controls.Ribbon.Ribbon Ribbon;
    internal TasksTreeView tasksTreeView;
    private bool _contentLoaded;

    public MainWindow()
    {
      this.InitializeComponent();
      this.Loaded += new RoutedEventHandler(this.MainView_Loaded);
    }

    private void MainView_Loaded(object sender, RoutedEventArgs e)
    {
      MainViewModel dataContext = this.DataContext as MainViewModel;
      if (dataContext == null)
        return;
      dataContext.Dispatcher = ViewLib.GetAddDelegate((FrameworkElement) this, DispatcherPriority.Background);
      dataContext.Project.SelectedDaysChanged += new EventHandler(this.Project_SelectedDaysChanged);
    }

    private void Project_SelectedDaysChanged(object sender, EventArgs e)
    {
    }

    private void Edit()
    {
      if (this.IsModelNull)
        return;
      Process.Start("notepad.exe", this.Model.Folder);
    }

    private bool IsModelNull
    {
      get
      {
        return this.Model == null;
      }
    }

    private MainViewModel Model
    {
      get
      {
        return this.DataContext as MainViewModel;
      }
    }

    private async void CommandBinding_OnOpenExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      await this.FileOpenNewFormatAsync();
    }

    private void CommandBinding_OnOpenCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = !this.IsModelNull;
    }

    private async void CommandBinding_OnSaveExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      await this.FileSaveNewFormatAsync();
    }

    private void CommandBinding_OnSaveCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = !this.IsModelNull;
    }

    private async void CommandBinding_OnSaveAsExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      await this.FileSaveAsNewFormatAsync();
    }

    private void CommandBinding_OnSaveAsCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = !this.IsModelNull;
    }

    private void CommandBinding_OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
    {
    }

    private void CommandBinding_OnCanCloseExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = !this.IsModelNull;
    }

    private void CommandBinding_OnClearExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      this.Model.Project.Clear();
    }

    private void CommandBinding_OnCanClearExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = !this.IsModelNull;
    }

    private void CommandBinding_OnEditExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      this.Edit();
    }

    private void CommandBinding_OnCanEditExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = !this.IsModelNull;
    }

    private void CommandBinding_OnFixTimeExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      this.Model.Project.FixTime();
    }

    private void CommandBinding_OnCanFixTimeExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = !this.IsModelNull;
    }

    private void CommandBinding_OnExtractContextExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      this.Model.Project.ExtractContext();
    }

    private void CommandBinding_OnCanExtractContextExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = !this.IsModelNull;
    }

    private void CommandBinding_OnFixContextExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      this.Model.Project.FixContext();
    }

    private void CommandBinding_OnCanFixContextExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = !this.IsModelNull;
    }

    private void CommandBinding_OnFixTitlesExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      this.Model.Project.FixTitles();
    }

    private void CommandBinding_OnCanFixTitlesExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = !this.IsModelNull;
    }

    private void CommandBinding_OnFixTypesExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      this.Model.Project.FixTypes();
    }

    private void CommandBinding_OnCanFixTypesExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = !this.IsModelNull;
    }

    private void CommandBinding_OnCanNewExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = !this.IsModelNull;
    }

    private async void CommandBinding_OnNewExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      await this.Model.NewProjectAsync();
    }

    private void CommandBinding_OnCanCopyTaskExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = !this.IsModelNull;
    }

    private void CommandBinding_OnCopyTaskExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      this.Model.CopyTask();
    }

    private void CommandBinding_OnCanContinueTaskExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = !this.IsModelNull;
    }

    private void CommandBinding_OnContinueTaskExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      this.Model.ContinueTask();
    }

    private void FileOpenOldFormat()
    {
      MainViewModel dataContext = this.DataContext as MainViewModel;
      if (dataContext == null)
        return;
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.InitialDirectory = dataContext.Folder;
      openFileDialog.FileName = dataContext.RecentFile;
      if (openFileDialog.ShowDialog() == DialogResult.Cancel)
        return;
      dataContext.Folder = Path.GetDirectoryName(openFileDialog.FileName);
      dataContext.RecentFile = openFileDialog.FileName;
      dataContext.FileOpenOldFormat();
    }

    private async Task FileOpenNewFormatAsync()
    {
      MainViewModel model = this.DataContext as MainViewModel;
      if (model == null)
        return;
      FolderBrowserDialog dialog = new FolderBrowserDialog();
      dialog.SelectedPath = model.Folder;
      if (dialog.ShowDialog() == DialogResult.Cancel)
        return;
      model.Folder = dialog.SelectedPath;
      await model.FileOpenNewFormatAsync();
    }

    private async Task FileSaveNewFormatAsync()
    {
      MainViewModel model = this.DataContext as MainViewModel;
      if (model == null)
        return;
      if (Directory.Exists(this.Model.Folder))
        await model.FileSaveNewFormatAsync();
      else
        await this.FileSaveAsNewFormatAsync();
    }

    private async Task FileSaveAsNewFormatAsync()
    {
      MainViewModel model = this.DataContext as MainViewModel;
      if (model == null)
        return;
      FolderBrowserDialog dialog = new FolderBrowserDialog();
      dialog.SelectedPath = model.Folder;
      if (dialog.ShowDialog() == DialogResult.Cancel)
        return;
      model.Folder = dialog.SelectedPath;
      await model.FileSaveNewFormatAsync();
    }

    private void Calendar_OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
    {
      IList addedItems = e.AddedItems;
      MainViewModel dataContext = this.DataContext as MainViewModel;
      if (dataContext == null)
        return;
      Calendar calendar = sender as Calendar;
      if (calendar == null)
        return;
      dataContext.Project.UpdateSelectDayTaks((IList) calendar.SelectedDates);
      dataContext.OnGenerateReportChanged();
    }

    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      System.Windows.Application.LoadComponent((object) this, new Uri("/ProjectApp;component/mainwindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((CommandBinding) target).CanExecute += new CanExecuteRoutedEventHandler(this.CommandBinding_OnCanNewExecute);
          ((CommandBinding) target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OnNewExecuted);
          break;
        case 2:
          ((CommandBinding) target).CanExecute += new CanExecuteRoutedEventHandler(this.CommandBinding_OnOpenCanExecute);
          ((CommandBinding) target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OnOpenExecuted);
          break;
        case 3:
          ((CommandBinding) target).CanExecute += new CanExecuteRoutedEventHandler(this.CommandBinding_OnSaveCanExecute);
          ((CommandBinding) target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OnSaveExecuted);
          break;
        case 4:
          ((CommandBinding) target).CanExecute += new CanExecuteRoutedEventHandler(this.CommandBinding_OnSaveAsCanExecute);
          ((CommandBinding) target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OnSaveAsExecuted);
          break;
        case 5:
          ((CommandBinding) target).CanExecute += new CanExecuteRoutedEventHandler(this.CommandBinding_OnCanCloseExecute);
          ((CommandBinding) target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OnCloseExecuted);
          break;
        case 6:
          ((CommandBinding) target).CanExecute += new CanExecuteRoutedEventHandler(this.CommandBinding_OnCanClearExecute);
          ((CommandBinding) target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OnClearExecuted);
          break;
        case 7:
          ((CommandBinding) target).CanExecute += new CanExecuteRoutedEventHandler(this.CommandBinding_OnCanEditExecute);
          ((CommandBinding) target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OnEditExecuted);
          break;
        case 8:
          ((CommandBinding) target).CanExecute += new CanExecuteRoutedEventHandler(this.CommandBinding_OnCanFixTimeExecute);
          ((CommandBinding) target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OnFixTimeExecuted);
          break;
        case 9:
          ((CommandBinding) target).CanExecute += new CanExecuteRoutedEventHandler(this.CommandBinding_OnCanExtractContextExecute);
          ((CommandBinding) target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OnExtractContextExecuted);
          break;
        case 10:
          ((CommandBinding) target).CanExecute += new CanExecuteRoutedEventHandler(this.CommandBinding_OnCanFixContextExecute);
          ((CommandBinding) target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OnFixContextExecuted);
          break;
        case 11:
          ((CommandBinding) target).CanExecute += new CanExecuteRoutedEventHandler(this.CommandBinding_OnCanFixTitlesExecute);
          ((CommandBinding) target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OnFixTitlesExecuted);
          break;
        case 12:
          ((CommandBinding) target).CanExecute += new CanExecuteRoutedEventHandler(this.CommandBinding_OnCanFixTypesExecute);
          ((CommandBinding) target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OnFixTypesExecuted);
          break;
        case 13:
          ((CommandBinding) target).CanExecute += new CanExecuteRoutedEventHandler(this.CommandBinding_OnCanCopyTaskExecute);
          ((CommandBinding) target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OnCopyTaskExecuted);
          break;
        case 14:
          ((CommandBinding) target).CanExecute += new CanExecuteRoutedEventHandler(this.CommandBinding_OnCanContinueTaskExecute);
          ((CommandBinding) target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OnContinueTaskExecuted);
          break;
        case 15:
          this.Ribbon = (System.Windows.Controls.Ribbon.Ribbon) target;
          break;
        case 16:
          this.tasksTreeView = (TasksTreeView) target;
          break;
        case 17:
          ((Calendar) target).SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(this.Calendar_OnSelectedDatesChanged);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
