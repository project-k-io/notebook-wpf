using System;
using System.Collections;
using System.Collections.Generic;

namespace ProjectK.Notebook.Models;

public static class ModelRules
{
    public static readonly List<string> GlobalContextList = new()
    {
        "Notebook",
        "Company",
        "Contact",
        "Node",
        "Task",
        "Note",
        "Time Tracker",
        "Year",
        "Month",
        "Day",
        "Week"
    };

    public static string GetSubNodeContext(string context)
    {
        return context switch
        {
            "Time Tracker" => "Year",
            "Year" => "Month",
            "Month" => "Week",
            "Week" => "Day",
            "Day" => "Task",
            "Task" => "Task",
            _ => "Node"
        };
    }

    public static bool GetSubNodeContext(string parentContext, out string context)
    {
        context = GetSubNodeContext(parentContext);
        return !string.IsNullOrEmpty(context);
    }


    public static string FixTypes(string title)
    {
        var upper = title.ToUpper();
        var type = "";
        if (upper.Contains("LUNCH") || upper.Contains("BREAKFAST"))
            type = "Lunch";
        else if (upper.Contains("TASK") || upper.Contains("CODE REVIEW") || title.Contains("TA") || title.Contains("US"))
            type = "Dev";
        else if (upper.Contains("BUILD"))
            type = "Build";
        else if (upper.Contains("TIME SHEET") || upper.Contains("TIMESHEET") || upper.Contains("EMAIL") ||
                 upper.Contains("PAPER WORKS"))
            type = "Misc";
        else if (upper.Contains("TALKED") || upper.Contains("MEETING") || upper.Contains("SHOWED"))
            type = "Meeting";
        else if (upper.Contains("Trouble"))
            type = "Support";
        return type;
    }

    public static bool IsNodeModelProperty(string n)
    {
        return n == "Id" ||
               n == "ParentId" ||
               n == "Name" ||
               n == "Created" ||
               n == "Context" ||
               n == "Description";
    }

    public static bool IsTaskModelProperty(string n)
    {
        return n == "Type" ||
               n == "SubType" ||
               n == "DataStarted" ||
               n == "DateEnded";
    }


    public static bool IsPersonalType(string type)
    {
        if (string.IsNullOrEmpty(type))
            return false;

        var upper = type.ToUpper();
        return upper.Contains("LUNCH") || upper.Contains("PERSONAL");
    }

    public static bool ContainDate(IList dates, DateTime a)
    {
        foreach (var date in dates)
            if (date is DateTime dateTime)
                if (a.Day == dateTime.Day && a.Month == dateTime.Month && a.Year == dateTime.Year)
                    return true;

        return false;
    }
}