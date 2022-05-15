using System;

namespace ProjectK.Notebook.Models.Reports;

public class ExcelCsvRecord
{
    public DateTime Day { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Type { get; set; }
    public string Type1 { get; set; }
    public string Task { get; set; }
    public string Type2 { get; set; }
    public string SubTask { get; set; }

    public override string ToString()
    {
        return $"{Day}, {Start}, {End}, {Type}, {Type1}, {Task}, {Type2}, {SubTask}";
    }

    public bool TryParse(string line)
    {
        var strArray = line.Split(',');

        if (!DateTime.TryParse(strArray[1], out var result))
            return false;

        Day = result;
        if (!DateTime.TryParse(strArray[2], out result))
            return false;

        Start = result;
        if (!DateTime.TryParse(strArray[3], out result))
            return false;

        End = result;
        Type = strArray[9];
        Type1 = strArray[10];
        Task = strArray[11];
        Type2 = strArray[12];
        SubTask = strArray[13];
        return true;
    }
}