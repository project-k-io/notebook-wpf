using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ProjectK.Notebook.Models.Reports;

public class ReportModule
{
    public ReportModule()
    {
        Records = new List<ReportRecord>();
        TotalRecord = new ReportRecord
        {
            Text = "Total"
        };
    }

    public List<ReportRecord> Records { get; set; }

    public ReportRecord TotalRecord { get; set; }

    public static void AppendHoursAndMinutes(StringBuilder sb, TimeSpan ts)
    {
        sb.AppendFormat("{0:00}:{1:00}", ts.TotalHours, ts.Minutes);
    }

    public static void AppendRecord(StringBuilder sb, ReportRecord r, int maxWidth, string tab)
    {
        sb.Append(r.Text);
        var num = string.IsNullOrEmpty(r.Text) ? 0 : r.Text.Length;
        if (num < maxWidth)
        {
            var str = new string(' ', maxWidth - num);
            sb.Append(str);
        }

        sb.Append(tab);
        AppendHoursAndMinutes(sb, r.Duration);
        sb.AppendLine();
    }

    public string GenerateReport(double maxDelta, bool useTimeOptimization)
    {
        var num = 60;
        SetDevDelta(useTimeOptimization ? FindDevDelta(maxDelta) : 1.0);
        var maxWidth = num + 5;
        var tab1 = new string(' ', 5);
        var str = new string(' ', 5);
        var tab2 = new string(' ', 10);
        var sb = new StringBuilder();
        foreach (var record in Records)
            if (record.Level == 2)
                record.Text = str + record.Text;
        sb.AppendLine("REPORT");
        sb.AppendLine();
        foreach (var record in Records)
            if (record.Level == 1)
            {
                sb.Append(str);
                AppendRecord(sb, record, maxWidth, tab1);
            }

        sb.AppendLine();
        AppendRecord(sb, TotalRecord, maxWidth, tab2);
        sb.AppendLine();
        sb.AppendLine("DETAILED REPORT");
        foreach (var record in Records)
        {
            if (record.Level == 1)
                sb.AppendLine();
            AppendRecord(sb, record, maxWidth, tab2);
        }

        sb.AppendLine();
        AppendRecord(sb, TotalRecord, maxWidth, tab2);
        return sb.ToString();
    }

    private static bool IsDev(ReportRecord r)
    {
        return r.Level == 2 && r.Type == "Dev";
    }

    private TimeSpan CalculateDev(double delta)
    {
        var timeSpan = new TimeSpan();
        foreach (var record in Records)
            if (record.Level == 2)
            {
                if (record.Type == "Dev")
                    timeSpan += TimeSpan.FromMinutes(record.Duration.TotalMinutes * delta);
                else
                    timeSpan += TimeSpan.FromMinutes(record.Duration.TotalMinutes);
            }

        return timeSpan;
    }

    private void SetDevDelta(double delta)
    {
        var timeSpan = new TimeSpan();
        foreach (var record in Records)
            if (IsDev(record))
            {
                record.Duration = TimeSpan.FromMinutes(record.Duration.TotalMinutes * delta);
                timeSpan += record.Duration;
            }

        foreach (var record in Records)
            if (record.Level == 1 && record.Type == "Dev")
                record.Duration = timeSpan;
        var zero = TimeSpan.Zero;
        foreach (var record in Records)
            if (record.Level == 2)
                zero += record.Duration;
        TotalRecord.Duration = zero;
    }

    private double FindDevDelta(double maxDelta)
    {
        var delta = 1.0;
        while (delta < 20.0)
        {
            var dev = CalculateDev(delta);
            Debug.WriteLine(dev.TotalHours);
            if (dev.TotalHours > maxDelta)
                return delta;
            delta += 0.1;
        }

        return 1.0;
    }
}