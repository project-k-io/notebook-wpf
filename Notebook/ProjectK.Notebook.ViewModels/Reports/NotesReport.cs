using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Models;
using ProjectK.Utils;
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
            const int Max = 80;
            if(node == null)
                return;

            var description = string.IsNullOrEmpty(node.Description) ? "" : node.Description;

            if (string.IsNullOrEmpty(description))
            {
                sb.Append(new string(' ', offset));
                sb.AppendLine(node.Title);
            }
            else
            {
                if (description.Length > Max)
                {
                    var  lines =  StringHelper.ConvertTextInMultipleLines(description, Max);
                    foreach (var line in lines)
                    {
                        sb.Append(new string(' ', offset));
                        sb.AppendLine(line);
                    }
                }
                else
                {
                    sb.Append(new string(' ', offset));
                    sb.AppendLine(description);
                }
            }

            foreach (var subTask in node.SubTasks)
            {
                GenerateReport(subTask, sb, offset + 2);
            }
        }
    }
}
