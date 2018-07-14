// Decompiled with JetBrains decompiler
// Type: Projects.Models.ReportModule
// Assembly: Projects.Models, Version=1.1.8.29073, Culture=neutral, PublicKeyToken=null
// MVID: 40F42789-FE28-4D3C-8B74-0B7FD98A36C8
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.Models.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Projects.Models
{
  public class ReportModule
  {
    public List<ReportRecord> Records { get; set; }

    public ReportRecord TotalRecord { get; set; }

    public ReportModule()
    {
      this.Records = new List<ReportRecord>();
      this.TotalRecord = new ReportRecord()
      {
        Text = "Total"
      };
    }

    public static void AppendHoursAndMinutes(StringBuilder sb, TimeSpan ts)
    {
      sb.AppendFormat("{0:00}:{1:00}", (object) ts.TotalHours, (object) ts.Minutes);
    }

    public static void AppendRecord(StringBuilder sb, ReportRecord r, int maxWidth, string tab)
    {
      sb.Append(r.Text);
      int num = string.IsNullOrEmpty(r.Text) ? 0 : r.Text.Length;
      if (num < maxWidth)
      {
        string str = new string(' ', maxWidth - num);
        sb.Append(str);
      }
      sb.Append(tab);
      ReportModule.AppendHoursAndMinutes(sb, r.Duration);
      sb.AppendLine();
    }

    public string GenerateReport(double maxDelta, bool useTimeOptimization)
    {
      int num = 60;
      this.SetDevDelta(useTimeOptimization ? this.FindDevDelta(maxDelta) : 1.0);
      int maxWidth = num + 5;
      string tab1 = new string(' ', 5);
      string str = new string(' ', 5);
      string tab2 = new string(' ', 10);
      StringBuilder sb = new StringBuilder();
      foreach (ReportRecord record in this.Records)
      {
        if (record.Level == 2)
          record.Text = str + record.Text;
      }
      sb.AppendLine("REPORT");
      sb.AppendLine();
      foreach (ReportRecord record in this.Records)
      {
        if (record.Level == 1)
        {
          sb.Append(str);
          ReportModule.AppendRecord(sb, record, maxWidth, tab1);
        }
      }
      sb.AppendLine();
      ReportModule.AppendRecord(sb, this.TotalRecord, maxWidth, tab2);
      sb.AppendLine();
      sb.AppendLine("DETAILED REPORT");
      foreach (ReportRecord record in this.Records)
      {
        if (record.Level == 1)
          sb.AppendLine();
        ReportModule.AppendRecord(sb, record, maxWidth, tab2);
      }
      sb.AppendLine();
      ReportModule.AppendRecord(sb, this.TotalRecord, maxWidth, tab2);
      return sb.ToString();
    }

    private bool IsDev(ReportRecord r)
    {
      return r.Level == 2 && r.Type == "Dev";
    }

    private TimeSpan CalculateDev(double delta)
    {
      TimeSpan timeSpan = new TimeSpan();
      foreach (ReportRecord record in this.Records)
      {
        if (record.Level == 2)
        {
          if (record.Type == "Dev")
            timeSpan += TimeSpan.FromMinutes(record.Duration.TotalMinutes * delta);
          else
            timeSpan += TimeSpan.FromMinutes(record.Duration.TotalMinutes);
        }
      }
      return timeSpan;
    }

    private void SetDevDelta(double delta)
    {
      TimeSpan timeSpan = new TimeSpan();
      foreach (ReportRecord record in this.Records)
      {
        if (this.IsDev(record))
        {
          record.Duration = TimeSpan.FromMinutes(record.Duration.TotalMinutes * delta);
          timeSpan += record.Duration;
        }
      }
      foreach (ReportRecord record in this.Records)
      {
        if (record.Level == 1 && record.Type == "Dev")
          record.Duration = timeSpan;
      }
      TimeSpan zero = TimeSpan.Zero;
      foreach (ReportRecord record in this.Records)
      {
        if (record.Level == 2)
          zero += record.Duration;
      }
      this.TotalRecord.Duration = zero;
    }

    private double FindDevDelta(double maxDelta)
    {
      double delta = 1.0;
      while (delta < 20.0)
      {
        TimeSpan dev = this.CalculateDev(delta);
        Debug.WriteLine((object) dev.TotalHours);
        if (dev.TotalHours > maxDelta)
          return delta;
        delta += 0.1;
      }
      return 1.0;
    }
  }
}
