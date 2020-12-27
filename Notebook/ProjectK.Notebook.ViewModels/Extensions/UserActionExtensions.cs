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