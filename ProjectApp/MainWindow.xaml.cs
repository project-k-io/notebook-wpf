using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;
using Microsoft.Win32;
using Projects.ViewModels;
using Vibor.View.Helpers.Misc;

namespace ProjectApp
{
    public partial class MainWindow : RibbonWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainView_Loaded;
        }

        public CommandBindingCollection CommandBindings2 { get; set; }

        private bool IsModelNull => Model == null;

        private MainViewModel Model => DataContext as MainViewModel;

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            var dataContext = DataContext as MainViewModel;
            if (dataContext == null) return;

            dataContext.Dispatcher = ViewLib.GetAddDelegate(this);
            dataContext.Project.SelectedDaysChanged += Project_SelectedDaysChanged;
            CommandBindings.AddRange(CreateCommands());
        }

        private void Project_SelectedDaysChanged(object sender, EventArgs e)
        {
        }

        private CommandBindingCollection CreateCommands()
        {
            var commandBindings = new CommandBindingCollection
            {
                new CommandBinding(ApplicationCommands.New, async (s, e) => await Model.NewProjectAsync(),
                    (s, e) => e.CanExecute = !IsModelNull),
                new CommandBinding(ApplicationCommands.Open, async (s, e) => await FileOpenNewFormatAsync(),
                    (s, e) => e.CanExecute = !IsModelNull),
                new CommandBinding(ApplicationCommands.Save, async (s, e) => await FileSaveNewFormatAsync(),
                    (s, e) => e.CanExecute = !IsModelNull),
                new CommandBinding(ApplicationCommands.SaveAs, async (s, e) => await FileSaveAsNewFormatAsync(),
                    (s, e) => e.CanExecute = !IsModelNull),
                new CommandBinding(ApplicationCommands.Close, async (s, e) => await Model.NewProjectAsync(),
                    (s, e) => e.CanExecute = !IsModelNull),
                new CommandBinding(Commands.Clear, (s, e) => Model.Project.Clear(),
                    (s, e) => e.CanExecute = !IsModelNull),
                new CommandBinding(Commands.Edit, (s, e) => Edit(), (s, e) => e.CanExecute = !IsModelNull),
                new CommandBinding(Commands.FixTime, (s, e) => Model.Project.FixTime(),
                    (s, e) => e.CanExecute = !IsModelNull),
                new CommandBinding(Commands.ExtractContext, (s, e) => Model.Project.FixContext(),
                    (s, e) => e.CanExecute = !IsModelNull),
                new CommandBinding(Commands.FixTitles, (s, e) => Model.Project.FixTitles(),
                    (s, e) => e.CanExecute = !IsModelNull),
                new CommandBinding(Commands.FixTypes, (s, e) => Model.Project.FixTypes(),
                    (s, e) => e.CanExecute = !IsModelNull),
                new CommandBinding(Commands.CopyTask, (s, e) => Model.CopyTask(),
                    (s, e) => e.CanExecute = !IsModelNull),
                new CommandBinding(Commands.ContinueTask, (s, e) => Model.ContinueTask(),
                    (s, e) => e.CanExecute = !IsModelNull)
            };
            return commandBindings;
        }

        private void Edit()
        {
            if (IsModelNull) return;

            Process.Start("notepad.exe", Model.Folder);
        }


        private void FileOpenOldFormat()
        {
            var dataContext = DataContext as MainViewModel;
            if (dataContext == null) return;

            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = dataContext.Folder;
            openFileDialog.FileName = dataContext.RecentFile;
            var result = openFileDialog.ShowDialog();
            if (result == false) return;

            dataContext.Folder = Path.GetDirectoryName(openFileDialog.FileName);
            dataContext.RecentFile = openFileDialog.FileName;
            dataContext.FileOpenOldFormat();
        }

        private async Task FileOpenNewFormatAsync()
        {
            var model = DataContext as MainViewModel;
            if (model == null) return;
#if AK
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = model.Folder;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;
            model.Folder = dialog.SelectedPath;
            await model.FileOpenNewFormatAsync();
#endif
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
#if AK
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = model.Folder;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

            model.Folder = dialog.SelectedPath;
            await model.FileSaveNewFormatAsync();
#endif
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