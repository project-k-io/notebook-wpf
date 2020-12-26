﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectK.Notebook.Domain
{
    public static class ModesRulesHelper
    {
        public static readonly List<string> GlobalContextList = new List<string>
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
    }
}
