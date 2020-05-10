using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using ProjectK.Logging;
using ProjectK.Notebook.ViewModels;
using ProjectK.Utils;
using ProjectK.View.Helpers.Misc;

namespace ProjectK.Notebook
{
    public partial class MainWindow : RibbonWindow
    {
        private readonly ILogger _logger = LogManager.GetLogger<MainWindow>();

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
            _logger.LogDebug("MainWindow Loaded()");
            if (!(DataContext is MainViewModel dataContext)) return;

            dataContext.OnDispatcher = ViewLib.GetAddDelegate(this);
            dataContext.Project.SelectedDaysChanged += Project_SelectedDaysChanged;
            InitCommandBindings();
        }

        private void Project_SelectedDaysChanged(object sender, EventArgs e)
        {
        }

        private void InitCommandBindings()
        {
            var commandBindings = new CommandBindingCollection
            {
                new CommandBinding(ApplicationCommands.New, async (s, e) => await Model.NewProjectAsync(), (s, e) => e.CanExecute = !IsModelNull),
                new CommandBinding(ApplicationCommands.Open, async (s, e) => await FileOpenNewFormatAsync(), (s, e) => e.CanExecute = !IsModelNull),
                new CommandBinding(ApplicationCommands.Save, async (s, e) => await FileSaveNewFormatAsync(), (s, e) => e.CanExecute = !IsModelNull),
                new CommandBinding(ApplicationCommands.SaveAs, async (s, e) => await FileSaveAsNewFormatAsync(), (s, e) => e.CanExecute = !IsModelNull),
                new CommandBinding(ApplicationCommands.Close, async (s, e) => await Model.NewProjectAsync(), (s, e) => e.CanExecute = !IsModelNull)
            };
            CommandBindings.AddRange(commandBindings);
        }

        private void FileOpenOldFormat()
        {
            if (!(DataContext is MainViewModel dataContext)) return;

            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = dataContext.Folder,
                FileName = dataContext.RecentFile
            };
            var result = openFileDialog.ShowDialog();
            if (result == false) return;

            dataContext.Folder = Path.GetDirectoryName(openFileDialog.FileName);
            dataContext.RecentFile = openFileDialog.FileName;
            dataContext.FileOpenOldFormat();
        }

        private async Task FileOpenNewFormatAsync()
        {
            if (!(DataContext is MainViewModel model)) return;
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
            if (!(DataContext is MainViewModel model)) return;

            if (Directory.Exists(Model.Folder))
                await model.FileSaveNewFormatAsync();
            else
                await FileSaveAsNewFormatAsync();
        }

        private async Task FileSaveAsNewFormatAsync()
        {
            if (!(DataContext is MainViewModel model)) return;
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
            if (!(DataContext is MainViewModel dataContext)) return;

            if (!(sender is Calendar calendar)) return;

            dataContext.Project.UpdateSelectDayTasks(calendar.SelectedDates);
            dataContext.OnGenerateReportChanged();
        }
    }
}