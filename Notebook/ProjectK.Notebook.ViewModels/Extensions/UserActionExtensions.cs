using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.ViewModels.Enums;

// using ProjectK.NotebookModel.Models.Versions.Version2;

namespace ProjectK.Notebook.ViewModels.Extensions
{
    public static class UserActionExtensions
    {
        private const string Tag = "UserAction";
        private static readonly ILogger Logger = LogManager.GetLogger<MainViewModel>();

        public static void UserAction_Edit(this MainViewModel model)
        {
#if AK
            var notebook = model.SelectedNotebook;
            if (notebook == null)
                return;

            var path = notebook.Title;
            Logger.LogDebug($"{Tag} | Edit | {Path.GetFileName(path)} | {Path.GetDirectoryName(path)}");

            Process.Start("explorer", path);
#endif
        }

        public static void UserAction_Clear(this MainViewModel model)
        {
#if AK
            var notebook = model.SelectedNotebook;
            if (notebook == null)
                return;

            Logger.LogDebug($"{Tag} | Clear");
            notebook.Clear();
#endif
        }


        public static async Task UserAction_ExportSelectedAllAsText(this MainViewModel model)
        {
            Logger.LogDebug($"{Tag} | ExportSelectedAllAsText()");

            var notebook = model.SelectedNode;
            if (notebook == null)
                return;

            await model.ExportSelectedAllAsText(model.TextReport);
        }

        public static async Task UserAction_ExportSelectedAllAsJson(this MainViewModel model)
        {
            Logger.LogDebug($"{Tag} | ExportSelectedAllAsJson()");


            await model.ExportSelectedAllAsJson();
        }

        public static void UserAction_ShowReport(this MainViewModel model, ReportTypes reportType)
        {
            Logger.LogDebug($"Show Report: {reportType}");
            model.OnGenerateReportChanged();
        }
    }
}