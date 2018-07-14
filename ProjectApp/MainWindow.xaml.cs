// Decompiled with JetBrains decompiler
// Type: ProjectApp.MainWindow
// Assembly: ProjectApp, Version=1.1.8.29131, Culture=neutral, PublicKeyToken=null
// MVID: A7331AD2-AF8A-4A84-BF9D-60C36001D1E0
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\ProjectApp.exe

using System;
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
using Projects.ViewModels;
using Vibor.View.Helpers.Misc;

namespace ProjectApp
{
    public partial class MainWindow : RibbonWindow, IComponentConnector
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainView_Loaded;
        }

        private bool IsModelNull => Model == null;

        private MainViewModel Model => DataContext as MainViewModel;

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            var dataContext = DataContext as MainViewModel;
            if (dataContext == null) return;

            dataContext.Dispatcher = ViewLib.GetAddDelegate(this, DispatcherPriority.Background);
            dataContext.Project.SelectedDaysChanged += Project_SelectedDaysChanged;
        }

        private void Project_SelectedDaysChanged(object sender, EventArgs e)
        {
        }

        private void Edit()
        {
            if (IsModelNull) return;

            Process.Start("notepad.exe", Model.Folder);
        }

        private async void CommandBinding_OnOpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            await FileOpenNewFormatAsync();
        }

        private void CommandBinding_OnOpenCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsModelNull;
        }

        private async void CommandBinding_OnSaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            await FileSaveNewFormatAsync();
        }

        private void CommandBinding_OnSaveCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsModelNull;
        }

        private async void CommandBinding_OnSaveAsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            await FileSaveAsNewFormatAsync();
        }

        private void CommandBinding_OnSaveAsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsModelNull;
        }

        private void CommandBinding_OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void CommandBinding_OnCanCloseExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsModelNull;
        }

        private void CommandBinding_OnClearExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Model.Project.Clear();
        }

        private void CommandBinding_OnCanClearExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsModelNull;
        }

        private void CommandBinding_OnEditExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Edit();
        }

        private void CommandBinding_OnCanEditExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsModelNull;
        }

        private void CommandBinding_OnFixTimeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Model.Project.FixTime();
        }

        private void CommandBinding_OnCanFixTimeExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsModelNull;
        }

        private void CommandBinding_OnExtractContextExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Model.Project.ExtractContext();
        }

        private void CommandBinding_OnCanExtractContextExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsModelNull;
        }

        private void CommandBinding_OnFixContextExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Model.Project.FixContext();
        }

        private void CommandBinding_OnCanFixContextExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsModelNull;
        }

        private void CommandBinding_OnFixTitlesExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Model.Project.FixTitles();
        }

        private void CommandBinding_OnCanFixTitlesExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsModelNull;
        }

        private void CommandBinding_OnFixTypesExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Model.Project.FixTypes();
        }

        private void CommandBinding_OnCanFixTypesExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsModelNull;
        }

        private void CommandBinding_OnCanNewExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsModelNull;
        }

        private async void CommandBinding_OnNewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            await Model.NewProjectAsync();
        }

        private void CommandBinding_OnCanCopyTaskExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsModelNull;
        }

        private void CommandBinding_OnCopyTaskExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Model.CopyTask();
        }

        private void CommandBinding_OnCanContinueTaskExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !IsModelNull;
        }

        private void CommandBinding_OnContinueTaskExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Model.ContinueTask();
        }

        private void FileOpenOldFormat()
        {
            var dataContext = DataContext as MainViewModel;
            if (dataContext == null) return;

            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = dataContext.Folder;
            openFileDialog.FileName = dataContext.RecentFile;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

            dataContext.Folder = Path.GetDirectoryName(openFileDialog.FileName);
            dataContext.RecentFile = openFileDialog.FileName;
            dataContext.FileOpenOldFormat();
        }

        private async Task FileOpenNewFormatAsync()
        {
            var model = DataContext as MainViewModel;
            if (model == null) return;

            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = model.Folder;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

            model.Folder = dialog.SelectedPath;
            await model.FileOpenNewFormatAsync();
        }

        private async Task FileSaveNewFormatAsync()
        {
            var model = DataContext as MainViewModel;
            if (model == null) return;

            if (Directory.Exists(Model.Folder))
                await model.FileSaveNewFormatAsync();
            else
                await FileSaveAsNewFormatAsync();
        }

        private async Task FileSaveAsNewFormatAsync()
        {
            var model = DataContext as MainViewModel;
            if (model == null) return;

            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = model.Folder;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

            model.Folder = dialog.SelectedPath;
            await model.FileSaveNewFormatAsync();
        }

        private void Calendar_OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            var addedItems = e.AddedItems;
            var dataContext = DataContext as MainViewModel;
            if (dataContext == null) return;

            var calendar = sender as Calendar;
            if (calendar == null) return;

            dataContext.Project.UpdateSelectDayTaks(calendar.SelectedDates);
            dataContext.OnGenerateReportChanged();
        }
    }
}