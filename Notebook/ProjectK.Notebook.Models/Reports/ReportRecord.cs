using System;

namespace ProjectK.Notebook.Models.Reports;

public class ReportRecord
{
    public ReportRecord()
    {
        Level = 1;
        Duration = new TimeSpan();
    }

    public int Level { get; set; }
    public string Type { get; set; }
    public string Text { get; set; }
    public TimeSpan Duration { get; set; }

    public override string ToString()
    {
        return $"Level={Level,2}, Type={Type,5}, Duration={Duration}";
    }
}