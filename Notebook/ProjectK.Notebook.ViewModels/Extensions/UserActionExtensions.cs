using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using ProjectK.Logging;

namespace ProjectK.Notebook.ViewModels.Extensions
{
    public static class UserActionExtensions
    {
        private static readonly ILogger Logger = LogManager.GetLogger<MainViewModel>();
        private const string Tag = "UserAction";

        public static void UserAction_Edit(this MainViewModel model)
        {
            var notebook = model.SelectedNotebook;
            if (notebook == null)
                return;

            var path = notebook.DataFile;
            Logger.LogDebug($"{Tag} | Edit | {path}");
            Process.Start("explorer", path);
        }

        public static void UserAction_Clear(this MainViewModel model)
        {
            var notebook = model.SelectedNotebook;
            if (notebook == null)
                return;

            Logger.LogDebug($"{Tag} | Clear");
            notebook.Clear();
        }

        public static async Task UserAction_OpenFileAsync(this MainViewModel model)
        {
            Logger.LogDebug("{Tag} | OpenFileAsync()");
            var dialog = new OpenFileDialog();
            var r = dialog.SetFileDialog(model.SelectedNotebook?.DataFile);
            if (!r.ok)
                return;

            var notebook = new NotebookViewModel();

            await notebook.OpenFileAsync(r.fileName); // User clicked open file
            model.SelectedNotebook = notebook;
            model.Notebooks.Add(notebook);
            model.RootTask.Add(notebook.RootTask);
        }


        private static void UserAction_FileOpenOldFormat(this MainViewModel model)
        {
            Logger.LogDebug("{ Tag} | FileOpenOldFormat()");
            var dialog = new OpenFileDialog();
            var r = dialog.SetFileDialog(model.SelectedNotebook.DataFile);
            if (!r.ok)
                return;

            model.SelectedNotebook.DataFile = r.fileName;
            model.FileOpenOldFormat();
        }


        public static async Task UserAction_SaveFileAsync(this MainViewModel model)
        {
            Logger.LogDebug("{ Tag} | SaveFileAsync()");
            var notebook = model.SelectedNotebook;
            if (notebook == null)
                return;

            if (File.Exists(notebook.DataFile))
                await model.SaveFileAsync(); // User Save
            else
                await model.UserAction_SaveFileAsAsync();
        }

        public static async Task UserAction_SaveFileAsAsync(this MainViewModel model)
        {
            Logger.LogDebug("{ Tag} | SaveFileAsAsync()");
            var notebook = model.SelectedNotebook;
            if (notebook == null)
                return;

            var dialog = new SaveFileDialog();
            var r = dialog.SetFileDialog(notebook.DataFile);
            if (!r.ok)
                return;

            notebook.DataFile = r.fileName;
            await model.SaveFileAsync(); // Save As
        }


    }
}
