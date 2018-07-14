// Decompiled with JetBrains decompiler
// Type: Projects.Views.ReportView
// Assembly: Projects.Views, Version=1.1.8.29122, Culture=neutral, PublicKeyToken=null
// MVID: BCB19E50-AB69-4CA9-9CF4-1A9C4DEAF8F2
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.Views.dll

using log4net;
using Projects.Models;
using Projects.ViewModels;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Projects.Views
{
  public partial class ReportView : UserControl, IComponentConnector
  {
    private static readonly ILog Logger = LogManager.GetLogger("Converter");
    private bool _contentLoaded;

    public ReportView()
    {
      this.InitializeComponent();
      this.Loaded += new RoutedEventHandler(this.ReportView_Loaded);
    }

    private void ReportView_Loaded(object sender, RoutedEventArgs e)
    {
      MainViewModel dataContext = this.DataContext as MainViewModel;
      if (dataContext == null)
        return;
      dataContext.GenerateReportChanged += new EventHandler<EventArgs>(this.viewModel_GenerateReportChanged);
    }

    private void viewModel_GenerateReportChanged(object sender, EventArgs e)
    {
      this.GenerateReportA();
    }

    public void GenerateReportA()
    {
      try
      {
        MainViewModel dataContext = this.DataContext as MainViewModel;
        if (dataContext == null)
          return;
        ProjectViewModel project = dataContext.Project;
        double maxDelta = 40.0 / 5.0 * (double) project.GetSelectedDays().Count;
        StringBuilder sb = new StringBuilder();
        string report = ReportView.GenerateReport((IList<TaskViewModel>) project.SelectedTaskList).GenerateReport(maxDelta, dataContext.UseTimeOptimization);
        TaskViewModel selectedTask = project.SelectedTask;
        if (selectedTask != null && selectedTask.Context == "Week")
          this.AddHeader(selectedTask, sb);
        sb.Append(report);
        if (project.SelectedTask != null && project.SelectedTask.Context == "Week")
        {
          DateTime dateStarted = project.SelectedTask.SubTasks.LastOrDefault<TaskViewModel>().DateStarted;
          File.WriteAllText(string.Format("Alan Kharebov Worksheet {0}-{1:00}-{2:00}.txt", (object) dateStarted.Year, (object) dateStarted.Month, (object) dateStarted.Day), dataContext.Report);
        }
        dataContext.Report = sb.ToString();
      }
      catch (Exception ex)
      {
        ReportView.Logger.Error((object) ex);
      }
    }

    public void AddHeader(TaskViewModel t, StringBuilder sb)
    {
      DateTime dateStarted1 = t.SubTasks.FirstOrDefault<TaskViewModel>().DateStarted;
      DateTime dateStarted2 = t.SubTasks.LastOrDefault<TaskViewModel>().DateStarted;
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
      sb.AppendFormat("                    INVOICE #{0}{1:00}{2:00}                                ", (object) dateStarted2.Year, (object) dateStarted2.Month, (object) dateStarted2.Day);
      sb.AppendLine();
      sb.AppendLine();
    }

    public static ReportModule GenerateReport(IList<TaskViewModel> list)
    {
      SortedList<string, SortedList<string, List<TaskViewModel>>> sortedList1 = new SortedList<string, SortedList<string, List<TaskViewModel>>>();
      foreach (TaskViewModel taskViewModel in (IEnumerable<TaskViewModel>) list)
      {
        if (!(taskViewModel.Context != "Task") && !string.IsNullOrEmpty(taskViewModel.Type) && !taskViewModel.IsSubTypeSleep)
        {
          if (!sortedList1.ContainsKey(taskViewModel.Type))
            sortedList1.Add(taskViewModel.Type, new SortedList<string, List<TaskViewModel>>());
          SortedList<string, List<TaskViewModel>> sortedList2 = sortedList1[taskViewModel.Type];
          if (!sortedList2.ContainsKey(taskViewModel.Title))
            sortedList2.Add(taskViewModel.Title, new List<TaskViewModel>());
          sortedList2[taskViewModel.Title].Add(taskViewModel);
        }
      }
      ReportModule reportModule = new ReportModule();
      foreach (KeyValuePair<string, SortedList<string, List<TaskViewModel>>> keyValuePair1 in sortedList1)
      {
        string key1 = keyValuePair1.Key;
        ReportRecord reportRecord1 = new ReportRecord() { Type = key1, Text = key1 };
        reportModule.Records.Add(reportRecord1);
        foreach (KeyValuePair<string, List<TaskViewModel>> keyValuePair2 in keyValuePair1.Value)
        {
          string key2 = keyValuePair2.Key;
          List<TaskViewModel> taskViewModelList = keyValuePair2.Value;
          TimeSpan timeSpan = new TimeSpan();
          foreach (TaskViewModel taskViewModel in taskViewModelList)
            timeSpan += taskViewModel.Duration;
          ReportRecord reportRecord2 = new ReportRecord() { Level = 2, Text = key2, Duration = timeSpan };
          reportRecord2.Type = key1;
          reportRecord1.Duration += reportRecord2.Duration;
          reportModule.Records.Add(reportRecord2);
        }
        reportModule.TotalRecord.Duration += reportRecord1.Duration;
      }
      return reportModule;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Projects.Views;component/reportview.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      this._contentLoaded = true;
    }
  }
}
