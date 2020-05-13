using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;
using System.Windows.Media.Animation;
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
            _logger.LogDebug("Loaded()");
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
                new CommandBinding(ApplicationCommands.Open, async (s, e) => await OpenFile(), (s, e) => e.CanExecute = !IsModelNull),
                new CommandBinding(ApplicationCommands.Save, async (s, e) => await FileSaveNewFormatAsync(), (s, e) => e.CanExecute = !IsModelNull),
                new CommandBinding(ApplicationCommands.SaveAs, async (s, e) => await FileSaveAsNewFormatAsync(), (s, e) => e.CanExecute = !IsModelNull),
                new CommandBinding(ApplicationCommands.Close, async (s, e) => await Model.NewProjectAsync(), (s, e) => e.CanExecute = !IsModelNull)
            };
            CommandBindings.AddRange(commandBindings);
        }

        public static (string fileName, bool ok) OpenFileGeneric(string path)
        {
            var dialog = new OpenFileDialog();
            var directoryName = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directoryName) && Directory.Exists(directoryName))
            {
                dialog.InitialDirectory = directoryName;
            }

            dialog.DefaultExt = ".json";
            dialog.Filter = "Json documents (.json)|*.json" + 
                            "|XML documents(.xml) | *.xml"; 

            var result = dialog.ShowDialog();
            if (result != true)
                return ("", false);

            return (dialog.FileName, true);
        }


        private void FileOpenOldFormat()
        {
            _logger.LogDebug("OpenFile()");
            if (!(DataContext is MainViewModel model)) return;
            var r = OpenFileGeneric(model.DataFile);
            if (!r.ok)
                return;
            
            model.DataFile = r.fileName;

            model.FileOpenOldFormat();
        }


        private async Task OpenFile()
        {
            _logger.LogDebug("OpenFile()");
            if (!(DataContext is MainViewModel model)) return;
            var r = OpenFileGeneric(model.DataFile);
            if (!r.ok)
                return;

            model.DataFile = r.fileName;
            await model.OpenFileNewFormatAsync();
        }

        private async Task FileSaveNewFormatAsync()
        {
            if (!(DataContext is MainViewModel model)) return;

            if (File.Exists(Model.DataFile))
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