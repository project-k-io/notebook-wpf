using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Models;
using LoggerExtensions = Microsoft.Extensions.Logging.LoggerExtensions;

namespace ProjectK.Notebook.ViewModels.Reports
{
    public class NotesReport
    {
        private static readonly ILogger Logger = LogManager.GetLogger<NotesReport>();

        public void GenerateReport(MainViewModel model)
        {
            Logger.LogDebug("GenerateReport()");
            try
            {
                var project = model.Notebook;

                var sb = new StringBuilder();
                GenerateReport(model.Notebook.SelectedTask, sb, 0);
                model.Report = sb.ToString();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void GenerateReport(TaskViewModel node, StringBuilder sb, int offset)
        {
            if(node == null)
                return;

            sb.Append(new string(' ', offset * 2));

            if(string.IsNullOrEmpty(node.Description))
                sb.AppendLine(node.Title);
            else
                sb.AppendLine(node.Description);

            foreach (var subTask in node.SubTasks)
            {
                GenerateReport(subTask, sb, offset + 1);
            }
        }
    }
}
