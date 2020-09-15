using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Utils;
using LoggerExtensions = Microsoft.Extensions.Logging.LoggerExtensions;

namespace ProjectK.Notebook.ViewModels.Reports
{
    public class NotesReport
    {
        private static readonly ILogger Logger = LogManager.GetLogger<NotesReport>();

        public string GenerateReport(TaskViewModel task)
        {
            Logger.LogDebug("GenerateReport()");
            try
            {

                var sb = new StringBuilder();
                GenerateReport(task, sb, 0);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return "";
            }
        }

        private const char SpaceChar = ' ';

        private void GenerateReport(TaskViewModel task, StringBuilder sb, int offset)
        {
            const int max = 80;
            if(task == null)
                return;

            var description = string.IsNullOrEmpty(task.Description) ? "" : task.Description;

            if (string.IsNullOrEmpty(description))
            {
                sb.Append(new string(SpaceChar, offset));
                sb.AppendLine(task.Title);
            }
            else
            {
                if (description.Length > max)
                {
                    var  lines =  StringHelper.ConvertTextInMultipleLines(description, max);
                    foreach (var line in lines)
                    {
                        sb.Append(new string(SpaceChar, offset));
                        sb.AppendLine(line);
                    }
                }
                else
                {
                    sb.Append(new string(SpaceChar, offset));
                    sb.AppendLine(description);
                }
            }

            foreach (var subTask in task.Nodes)
            {
                GenerateReport((TaskViewModel)subTask, sb, offset + 2);
            }
        }
    }
}
