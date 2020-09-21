using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Domain.Reports;
using ProjectK.Utils.Extensions;

namespace ProjectK.Notebook.ViewModels.Reports
{
    public  class WorksheetReport
    {
        private static readonly ILogger Logger = LogManager.GetLogger<WorksheetReport>();

        private  ReportModule GenerateReport(IList<NodeViewModel> nodes)
        {
            var sortedList = new SortedList<string, SortedList<string, List<NodeViewModel>>>();
            foreach (var node in nodes)

                if (node.Context == "TaskModel")
                {
                    if (node is TaskViewModel task)
                    {
                        if (!string.IsNullOrEmpty(task.Type) && !task.IsSubTypeSleep)
                        {
                            if (!sortedList.ContainsKey(task.Type))
                                sortedList.Add(task.Type, new SortedList<string, List<NodeViewModel>>());

                            var sortedList2 = sortedList[task.Type];
                            if (!sortedList2.ContainsKey(node.Title))
                                sortedList2.Add(node.Title, new List<NodeViewModel>());
                            sortedList2[node.Title].Add(node);
                        }
                    }
                }

            var reportModule = new ReportModule();
            foreach (var kv1 in sortedList)
            {
                var key1 = kv1.Key;
                var record = new ReportRecord { Type = key1, Text = key1 };
                reportModule.Records.Add(record);
                foreach (var kv2 in kv1.Value)
                {
                    var key2 = kv2.Key;
                    var nodes2 = kv2.Value;
                    var timeSpan = new TimeSpan();
                    foreach (var node2 in nodes2)
                    {
                        if (node2 is TaskViewModel task2)
                            timeSpan += task2.Duration;
                    }

                    var record2 = new ReportRecord { Level = 2, Text = key2, Duration = timeSpan };
                    record2.Type = key1;
                    record.Duration += record2.Duration;
                    reportModule.Records.Add(record2);
                }

                reportModule.TotalRecord.Duration += record.Duration;
            }

            return reportModule;
        }

        private  void AddHeader(NodeViewModel t, StringBuilder sb, ILogger logger)
        {
            logger.LogDebug("GenerateReport()");
            if (t.Nodes.IsNullOrEmpty())
                return;

            var firstTask = (TaskViewModel) t.Nodes.FirstOrDefault();
            var lastTask = (TaskViewModel)t.Nodes.LastOrDefault();

            var dateStarted1 = firstTask?.DateStarted;
            var dateStarted2 = lastTask?.DateStarted;
            sb.AppendLine("                       Alan Kharebov                                  ");
            sb.AppendLine();
            sb.AppendLine("                        Worksheet                                     ");
            sb.AppendLine();
            sb.Append("                 From ");
            sb.Append(dateStarted1?.ToShortDateString());
            sb.Append(" to ");
            sb.Append(dateStarted2?.ToShortDateString());
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendFormat("                    INVOICE #{0}{1:00}{2:00}                                ", dateStarted2?.Year, dateStarted2?.Month, dateStarted2?.Day);
            sb.AppendLine();
            sb.AppendLine();
        }

        public  void GenerateReport(MainViewModel model)
        {
            Logger.LogDebug("GenerateReport()");
            try
            {
                var notebook = model.SelectedNotebook;
                if(notebook == null)
                    return;


                var maxDelta = 40.0 / 5.0 * notebook.GetSelectedDays().Count;

                var sb = new StringBuilder();
                var report = GenerateReport(notebook.SelectedNodeList).GenerateReport(maxDelta, model.UseTimeOptimization);
                var selectedTask = notebook.SelectedNode;

                if (selectedTask != null && selectedTask.Context == "Week")
                    AddHeader(selectedTask, sb, Logger);

                sb.Append(report);
                if (notebook.SelectedNode != null && notebook.SelectedNode.Context == "Week")
                {
                    var nodes = notebook.SelectedNode.Nodes;
                    var lastNode = (NodeViewModel)nodes.LastOrDefault();
                    if (lastNode != null)
                    {
                        var dateStarted = lastNode is TaskViewModel lastTask ? lastTask.DateStarted : DateTime.Now;
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
