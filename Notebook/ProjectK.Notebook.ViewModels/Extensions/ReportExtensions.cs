using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Models;
using ProjectK.Utils.Extensions;

namespace ProjectK.Notebook.ViewModels.Extensions
{
    public static class ReportExtensions
    {
        // private static readonly ILogger Logger = LogManager.GetLogger<ReportExtensions>();

#if AK
                if (!(DataContext is DomainViewModel dataContext))
                    return;


#endif
        public static ReportModule GenerateReport(IList<TaskViewModel> list)
        {
            var sortedList1 = new SortedList<string, SortedList<string, List<TaskViewModel>>>();
            foreach (var taskViewModel in list)
                if (!(taskViewModel.Context != "Task") && !string.IsNullOrEmpty(taskViewModel.Type) &&
                    !taskViewModel.IsSubTypeSleep)
                {
                    if (!sortedList1.ContainsKey(taskViewModel.Type))
                        sortedList1.Add(taskViewModel.Type, new SortedList<string, List<TaskViewModel>>());
                    var sortedList2 = sortedList1[taskViewModel.Type];
                    if (!sortedList2.ContainsKey(taskViewModel.Title))
                        sortedList2.Add(taskViewModel.Title, new List<TaskViewModel>());
                    sortedList2[taskViewModel.Title].Add(taskViewModel);
                }

            var reportModule = new ReportModule();
            foreach (var keyValuePair1 in sortedList1)
            {
                var key1 = keyValuePair1.Key;
                var reportRecord1 = new ReportRecord { Type = key1, Text = key1 };
                reportModule.Records.Add(reportRecord1);
                foreach (var keyValuePair2 in keyValuePair1.Value)
                {
                    var key2 = keyValuePair2.Key;
                    var taskViewModelList = keyValuePair2.Value;
                    var timeSpan = new TimeSpan();
                    foreach (var taskViewModel in taskViewModelList)
                        timeSpan += taskViewModel.Duration;
                    var reportRecord2 = new ReportRecord { Level = 2, Text = key2, Duration = timeSpan };
                    reportRecord2.Type = key1;
                    reportRecord1.Duration += reportRecord2.Duration;
                    reportModule.Records.Add(reportRecord2);
                }

                reportModule.TotalRecord.Duration += reportRecord1.Duration;
            }

            return reportModule;
        }

        private static void AddHeader(TaskViewModel t, StringBuilder sb, ILogger logger)
        {
            logger.LogDebug("GenerateReport()");
            if (t.SubTasks.IsNullOrEmpty())
                return;

            var firstTask = t.SubTasks.FirstOrDefault();
            var lastTask = t.SubTasks.LastOrDefault();

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

        public static void GenerateReport(this MainViewModel dataContext, ILogger logger)
        {
            logger.LogDebug("GenerateReport()");
            try
            {
                var project = dataContext.Notebook;
                var maxDelta = 40.0 / 5.0 * project.GetSelectedDays().Count;

                var sb = new StringBuilder();
                var report = GenerateReport(project.SelectedTaskList).GenerateReport(maxDelta, dataContext.UseTimeOptimization);
                var selectedTask = project.SelectedTask;

                if (selectedTask != null && selectedTask.Context == "Week")
                    AddHeader(selectedTask, sb, logger);

                sb.Append(report);
                if (project.SelectedTask != null && project.SelectedTask.Context == "Week")
                {
                    var subTasks = project.SelectedTask.SubTasks;
                    var lastTask = subTasks.LastOrDefault();

                    if (lastTask != null)
                    {
                        var dateStarted = lastTask.DateStarted;
                        File.WriteAllText($"Alan Kharebov Worksheet {dateStarted.Year}-{dateStarted.Month:00}-{dateStarted.Day:00}.txt", dataContext.Report);
                    }
                }

                dataContext.Report = sb.ToString();
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
            }
        }
    }
}
