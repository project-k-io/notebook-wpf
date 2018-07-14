﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Projects.Models;
using Projects.ViewModels;
using Vibor.Logging;

namespace Projects.Views
{
    public partial class ReportView : UserControl, IComponentConnector
    {
        private static readonly ILog Logger = LogManager.GetLogger("Converter");

        public ReportView()
        {
            InitializeComponent();
            Loaded += ReportView_Loaded;
        }

        private void ReportView_Loaded(object sender, RoutedEventArgs e)
        {
            var dataContext = DataContext as MainViewModel;
            if (dataContext == null)
                return;
            dataContext.GenerateReportChanged += viewModel_GenerateReportChanged;
        }

        private void viewModel_GenerateReportChanged(object sender, EventArgs e)
        {
            GenerateReportA();
        }

        public void GenerateReportA()
        {
            try
            {
                var dataContext = DataContext as MainViewModel;
                if (dataContext == null)
                    return;
                var project = dataContext.Project;
                var maxDelta = 40.0 / 5.0 * project.GetSelectedDays().Count;
                var sb = new StringBuilder();
                var report = GenerateReport(project.SelectedTaskList)
                    .GenerateReport(maxDelta, dataContext.UseTimeOptimization);
                var selectedTask = project.SelectedTask;
                if (selectedTask != null && selectedTask.Context == "Week")
                    AddHeader(selectedTask, sb);
                sb.Append(report);
                if (project.SelectedTask != null && project.SelectedTask.Context == "Week")
                {
                    var dateStarted = project.SelectedTask.SubTasks.LastOrDefault().DateStarted;
                    File.WriteAllText(
                        string.Format("Alan Kharebov Worksheet {0}-{1:00}-{2:00}.txt", dateStarted.Year,
                            dateStarted.Month, dateStarted.Day), dataContext.Report);
                }

                dataContext.Report = sb.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public void AddHeader(TaskViewModel t, StringBuilder sb)
        {
            var dateStarted1 = t.SubTasks.FirstOrDefault().DateStarted;
            var dateStarted2 = t.SubTasks.LastOrDefault().DateStarted;
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
            sb.AppendFormat("                    INVOICE #{0}{1:00}{2:00}                                ",
                dateStarted2.Year, dateStarted2.Month, dateStarted2.Day);
            sb.AppendLine();
            sb.AppendLine();
        }

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
                var reportRecord1 = new ReportRecord {Type = key1, Text = key1};
                reportModule.Records.Add(reportRecord1);
                foreach (var keyValuePair2 in keyValuePair1.Value)
                {
                    var key2 = keyValuePair2.Key;
                    var taskViewModelList = keyValuePair2.Value;
                    var timeSpan = new TimeSpan();
                    foreach (var taskViewModel in taskViewModelList)
                        timeSpan += taskViewModel.Duration;
                    var reportRecord2 = new ReportRecord {Level = 2, Text = key2, Duration = timeSpan};
                    reportRecord2.Type = key1;
                    reportRecord1.Duration += reportRecord2.Duration;
                    reportModule.Records.Add(reportRecord2);
                }

                reportModule.TotalRecord.Duration += reportRecord1.Duration;
            }

            return reportModule;
        }
    }
}