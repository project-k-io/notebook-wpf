using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using ProjectK.Logging;
using ProjectK.Notebook.ViewModels;

namespace ProjectK.Notebook
{
    class MainViewModel: NotebookViewModel
    {
        private readonly ILogger _logger = LogManager.GetLogger<MainViewModel>();

        public CommandBindingCollection CreateCommandBindings()
        {
            var commandBindings = new CommandBindingCollection
            {
                new CommandBinding(ApplicationCommands.New, async (s, e) => await NewProjectAsync(), (s, e) => e.CanExecute = true),
                new CommandBinding(ApplicationCommands.Open, async (s, e) => await OpenFile(), (s, e) => e.CanExecute = true),
                new CommandBinding(ApplicationCommands.Save, async (s, e) => await UserSaveFileAsync(), (s, e) => e.CanExecute = true),
                new CommandBinding(ApplicationCommands.SaveAs, async (s, e) => await SaveFileAsAsync(), (s, e) => e.CanExecute = true),
                new CommandBinding(ApplicationCommands.Close, async (s, e) => await NewProjectAsync(), (s, e) => e.CanExecute = true)
            };
            return commandBindings;
        }


        private void UI_FileOpenOldFormat()
        {
            _logger.LogDebug("OpenFile()");
            var dialog = new OpenFileDialog();
            var r = SetFileDialog(dialog, DataFile);
            if (!r.ok)
                return;

            DataFile = r.fileName;
            FileOpenOldFormat();
        }


        private async Task OpenFile()
        {
            _logger.LogDebug("OpenFile()");
            var dialog = new OpenFileDialog();
            var r = SetFileDialog(dialog, DataFile);
            if (!r.ok)
                return;

            DataFile = r.fileName;
            await OpenFileNewFormatAsync();
        }

        private async Task UserSaveFileAsync()
        {
            if (File.Exists(DataFile))
                await SaveFileAsync();
            else
                await SaveFileAsAsync();
        }

        private async Task SaveFileAsAsync()
        {
            var dialog = new SaveFileDialog();
            var r = SetFileDialog(dialog, DataFile);
            if (!r.ok)
                return;

            DataFile = r.fileName;
            await SaveFileAsync();
        }

        public (string fileName, bool ok) SetFileDialog(FileDialog dialog, string path)
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

    }
}
