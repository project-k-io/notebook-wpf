using System;
using Microsoft.Extensions.Logging;

namespace ProjectK.ToolKit.ViewModels;

public class OutputRecordViewModel
{
    private const string Format = "{0}\t{1}\t{2}\t{3}\t{4}";

    public int Id { get; set; }
    public LogLevel Type { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Duration { get; set; }
    public string State { get; set; }
    public string Message { get; set; }
    public static string Header => string.Format(Format, "Id", "Type", "Date", "State", "Message");

    public override string ToString()
    {
        return string.Format(Format, Id, Type, Date, State, Message);
    }
}