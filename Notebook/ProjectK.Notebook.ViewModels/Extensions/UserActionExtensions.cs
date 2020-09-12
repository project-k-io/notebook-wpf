using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using ProjectK.Logging;
using ProjectK.Notebook.Domain;
using ProjectK.Notebook.ViewModels.Enums;
using ProjectK.Utils;

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

            var path = notebook.Title;
            Logger.LogDebug($"{Tag} | Edit | {Path.GetFileName(path)} | {Path.GetDirectoryName(path)}");

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

        public static async Task UserAction_OpenFileAsync(this MainViewModel mainViewModel)
        {
            Logger.LogDebug("{Tag} | OpenFileAsync()");
            var dialog = new OpenFileDialog();
            var r = dialog.SetFileDialog(mainViewModel.SelectedNotebook?.Title);
            if (!r.ok)
                return;


            // await notebook.OpenFileAsync(r.fileName); // User clicked open file
            var path = r.fileName;
            Logger.LogDebug($"OpenFileAsync | {Path.GetDirectoryName(path)} | {Path.GetFileName(path)} ");
            var model = await FileHelper.ReadFromFileAsync<NotebookModel>(path);
            mainViewModel.AddNewNotebook(model, path);
        }


        public static async Task UserAction_ExportSelectedAllAsText(this MainViewModel model)
        {
            Logger.LogDebug($"{Tag} | ExportSelectedAllAsText()");

            var notebook = model.SelectedNotebook;
            if (notebook == null)
                return;



            await notebook.ExportSelectedAllAsText(model.TextReport);
        }

        public static async Task UserAction_ExportSelectedAllAsJson(this MainViewModel model)
        {
            Logger.LogDebug($"{Tag} | ExportSelectedAllAsJson()");

            var notebook = model.SelectedNotebook;
            if (notebook == null)
                return;

            await notebook.ExportSelectedAllAsJson();
        }

        public static async Task UserAction_ImportToSelectedAsJson(this MainViewModel model)
        {
            Logger.LogDebug($"{Tag} | ImportToSelectedAsJson()");

            var notebook = model.SelectedNotebook;
            if (notebook == null)
                return;

            await notebook.SelectedTask.ImportToSelectedAsJson();
        }

        public static void UserAction_ShowReport(this MainViewModel model, ReportTypes reportType)
        {
            Logger.LogDebug($"Show Report: {reportType}");
            model.OnGenerateReportChanged();
        }

    }
}
