using System;
using System.Text;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Domain;
using ProjectK.Utils;

namespace ProjectK.Notebook.ViewModels.Reports
{
    public class NotesReport
    {
        private static readonly ILogger Logger = LogManager.GetLogger<NotesReport>();

        public string GenerateReport(NodeViewModel node)
        {
            Logger.LogDebug("GenerateReport()");
            try
            {

                var sb = new StringBuilder();
                GenerateReport(node, sb, 0);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return "";
            }
        }

        private const char SpaceChar = ' ';

        private void GenerateReport(NodeViewModel node, StringBuilder sb, int offset)
        {
            const int max = 80;
            if(node == null)
                return;

            if (node.Model is TaskModel task)
            {
                var description = string.IsNullOrEmpty(task.Description) ? "" : task.Description;

                if (string.IsNullOrEmpty(description))
                {
                    sb.Append(new string(SpaceChar, offset));
                    sb.AppendLine(node.Name);
                }
                else
                {
                    if (description.Length > max)
                    {
                        var lines = StringHelper.ConvertTextInMultipleLines(description, max);
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

            }


            foreach (var subTask in node.Nodes)
            {
                GenerateReport(subTask, sb, offset + 2);
            }
        }
    }
}
