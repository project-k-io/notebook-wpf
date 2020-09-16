using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Domain;
using ProjectK.Notebook.Domain.Reports;
using ProjectK.Utils.Extensions;

namespace ProjectK.Notebook.ViewModels.Reports
{
    public  class WorksheetReport
    {
        private static readonly ILogger Logger = LogManager.GetLogger<WorksheetReport>();

        private  ReportModule GenerateReport(IList<NodeViewModel> list)
        {
            var sortedList = new SortedList<string, SortedList<string, List<NodeViewModel>>>();
            foreach (var item in list)
                if (item.Context == "Task" && !string.IsNullOrEmpty(item.Type)
#if AK
                                           && !item.IsSubTypeSleep
#endif
                    )
                {
                    if (!sortedList.ContainsKey(item.Type))
                        sortedList.Add(item.Type, new SortedList<string, List<NodeViewModel>>());
                    var sortedList2 = sortedList[item.Type];
                    if (!sortedList2.ContainsKey(item.Title))
                        sortedList2.Add(item.Title, new List<NodeViewModel>());
                    sortedList2[item.Title].Add(item);
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
                    var tasks = kv2.Value;
                    var timeSpan = new TimeSpan();
#if AK
                    foreach (var task in tasks)
                        timeSpan += task.Duration;
#endif
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

            var firstTask = (NodeViewModel) t.Nodes.FirstOrDefault();
            var lastTask = (NodeViewModel)t.Nodes.LastOrDefault();

#if AK
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
#endif
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
                var report = GenerateReport(notebook.SelectedTaskList).GenerateReport(maxDelta, model.UseTimeOptimization);
                var selectedTask = notebook.SelectedTask;

                if (selectedTask != null && selectedTask.Context == "Week")
                    AddHeader(selectedTask, sb, Logger);

                sb.Append(report);
                if (notebook.SelectedTask != null && notebook.SelectedTask.Context == "Week")
                {
                    var subTasks = notebook.SelectedTask.Nodes;
                    var lastTask = (NodeViewModel)subTasks.LastOrDefault();
#if AK
                    if (lastTask != null)
                    {
                        var dateStarted = lastTask.DateStarted;
                        File.WriteAllText($"Alan Kharebov Worksheet {dateStarted.Year}-{dateStarted.Month:00}-{dateStarted.Day:00}.txt", model.TextReport);
                    }
#endif
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
