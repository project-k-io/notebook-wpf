using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using ProjectK.Logging;
using ProjectK.Notebook.Domain;
using ProjectK.Notebook.Domain.Versions.Version2;
// using ProjectK.NotebookModel.Models.Versions.Version2;
using ProjectK.Notebook.ViewModels.Enums;
using ProjectK.Utils;
using Task = System.Threading.Tasks.Task;

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
            try
            {
                Logger.LogDebug($"{Tag} | OpenFileAsync()");
                var dialog = new OpenFileDialog();
                var r = dialog.SetFileDialog(mainViewModel.SelectedNotebook?.Title);
                if (!r.ok)
                    return;

                var path = r.fileName;
                Logger.LogDebug($"OpenFileAsync | {Path.GetDirectoryName(path)} | {Path.GetFileName(path)} ");

                var dataModel = await FileHelper.ReadFromFileAsync<DataModel>(path);

                var notebook = new NotebookModel { Name = path};
                // Populate Notebook model from DataModel
                notebook.Init(dataModel);
                mainViewModel.ImportNotebook(notebook);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
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

        public static void UserAction_ShowReport(this MainViewModel model, ReportTypes reportType)
        {
            Logger.LogDebug($"Show Report: {reportType}");
            model.OnGenerateReportChanged();
        }

    }
}
