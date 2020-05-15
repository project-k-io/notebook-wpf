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
    public partial class MainWindow : Window
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
                new CommandBinding(ApplicationCommands.Save, async (s, e) => await SaveFileAsync(), (s, e) => e.CanExecute = !IsModelNull),
                new CommandBinding(ApplicationCommands.SaveAs, async (s, e) => await SaveFileAsAsync(), (s, e) => e.CanExecute = !IsModelNull),
                new CommandBinding(ApplicationCommands.Close, async (s, e) => await Model.NewProjectAsync(), (s, e) => e.CanExecute = !IsModelNull)
            };
            CommandBindings.AddRange(commandBindings);
        }

        public static (string fileName, bool ok) SetFileDialog(FileDialog dialog, string path)
        {
            var directoryName = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directoryName) && Directory.Exists(directoryName))
            {
                dialog.InitialDirectory = directoryName;
            }

            var fileName = Path.GetFileNameWithoutExtension(path);
            if (!string.IsNullOrEmpty(fileName))
            {
                dialog.FileName = fileName;
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

            var dialog = new OpenFileDialog();
            var r = SetFileDialog(dialog, model.DataFile);
            if (!r.ok)
                return;

            model.DataFile = r.fileName;

            model.FileOpenOldFormat();
        }


        private async Task OpenFile()
        {
            _logger.LogDebug("OpenFile()");
            if (!(DataContext is MainViewModel model)) return;

            var dialog = new OpenFileDialog();
            var r = SetFileDialog(dialog, model.DataFile);
            if (!r.ok)
                return;

            model.DataFile = r.fileName;
            await model.OpenFileNewFormatAsync();
        }

        private async Task SaveFileAsync()
        {
            if (!(DataContext is MainViewModel model)) return;

            if (File.Exists(Model.DataFile))
                await model.FileSaveNewFormatAsync();
            else
                await SaveFileAsAsync();
        }

        private async Task SaveFileAsAsync()
        {
            if (!(DataContext is MainViewModel model)) return;

            var dialog = new SaveFileDialog();
            var r = SetFileDialog(dialog, model.DataFile);
            if (!r.ok)
                return;

            model.DataFile = r.fileName;
            await model.FileSaveNewFormatAsync();
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