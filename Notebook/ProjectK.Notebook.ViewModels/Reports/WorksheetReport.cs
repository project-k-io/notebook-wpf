using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Models;
using ProjectK.Notebook.Models.Reports;
using ProjectK.Notebook.ViewModels.Extensions;
using ProjectK.Utils.Extensions;

namespace ProjectK.Notebook.ViewModels.Reports
{
    public class WorksheetReport
    {
        private static readonly ILogger Logger = LogManager.GetLogger<WorksheetReport>();

        private static ReportModule GenerateReport(IList<NodeViewModel> nodes)
        {
            var sortedList = new SortedList<string, SortedList<string, List<NodeViewModel>>>();
            foreach (var node in nodes)
                if (node.Model is TaskModel task)
                    if (!string.IsNullOrEmpty(task.Type) && !task.IsSubTypeSleep())
                    {
                        if (!sortedList.ContainsKey(task.Type))
                            sortedList.Add(task.Type, new SortedList<string, List<NodeViewModel>>());

                        var sortedList2 = sortedList[task.Type];
                        if (!sortedList2.ContainsKey(node.Name))
                            sortedList2.Add(node.Name, new List<NodeViewModel>());
                        sortedList2[node.Name].Add(node);
                    }

            var reportModule = new ReportModule();
            foreach (var kv1 in sortedList)
            {
                var key1 = kv1.Key;
                var record = new ReportRecord {Type = key1, Text = key1};
                reportModule.Records.Add(record);
                foreach (var kv2 in kv1.Value)
                {
                    var key2 = kv2.Key;
                    var nodes2 = kv2.Value;
                    var timeSpan = new TimeSpan();
                    foreach (var node2 in nodes2)
                        if (node2.Model is TaskModel task2)
                            timeSpan += task2.Duration;

                    var record2 = new ReportRecord {Level = 2, Text = key2, Duration = timeSpan, Type = key1};
                    record.Duration += record2.Duration;
                    reportModule.Records.Add(record2);
                }

                reportModule.TotalRecord.Duration += record.Duration;
            }

            return reportModule;
        }

        private static void AddHeader(NodeViewModel t, StringBuilder sb, ILogger logger)
        {
            logger.LogDebug("GenerateReport()");
            if (t.Nodes.IsNullOrEmpty())
                return;

            var firstNode = t.Nodes.FirstOrDefault();
            var lastNode = t.Nodes.LastOrDefault();

            if (!(firstNode?.Model is TaskModel firstTask))
                return;

            if (!(lastNode?.Model is TaskModel lastTask))
                return;

            var dateStarted1 = firstTask.DateStarted;
            var dateStarted2 = lastTask.DateStarted;
            sb.AppendLine("                       Alan Kharebov                                  ");
            sb.AppendLine();
            sb.AppendLine("                        Worksheet                                     ");
            sb.AppendLine();
            sb.Append("                 From ");
            sb.Append(dateStarted1.ToShortDateString());
            sb.Append(" to ");
            sb.Append(dateStarted2.ToShortDateString());
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendFormat("                    INVOICE #{0}{1:00}{2:00}                                ", dateStarted2.Year, dateStarted2.Month, dateStarted2.Day);
            sb.AppendLine();
            sb.AppendLine();
        }

        public static void GenerateReport(MainViewModel model)
        {
            Logger.LogDebug("GenerateReport()");
            try
            {
                var maxDelta = 40.0 / 5.0 * model.GetSelectedDays().Count;

                var sb = new StringBuilder();
                var report = GenerateReport(model.SelectedNodeList).GenerateReport(maxDelta, model.UseTimeOptimization);

                if (!(model.SelectedNode is NodeViewModel selectedNode))
                    return;

                if (selectedNode.Context == "Week")
                    AddHeader(selectedNode, sb, Logger);

                sb.Append(report);

                if (selectedNode.Context == "Week")
                {
                    var nodes = selectedNode.Nodes;
                    var lastNode = (NodeViewModel) nodes.LastOrDefault();
                    if (lastNode != null)
                    {
                        var dateStarted = lastNode.Model is TaskModel lastTask ? lastTask.DateStarted : DateTime.Now;
                        File.WriteAllText($"Alan Kharebov Worksheet {dateStarted.Year}-{dateStarted.Month:00}-{dateStarted.Day:00}.txt", model.TextReport);
                    }
                }

                model.TextReport = sb.ToString();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
    }
}