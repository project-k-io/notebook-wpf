using System.Collections.Generic;
using ProjectK.Notebook.Domain.Interfaces;

namespace ProjectK.Notebook.ViewModels.Helpers
{
    public static class RulesHelper
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

        public static string GetSubNodeTitle(NodeViewModel parent, INode node)
        {
            var title = string.Empty;
            switch (parent.Context)
            {
                case "Time Tracker":
                    title = node.Created.ToString("yyyy");
                    break;
                case "Year":
                    title = node.Created.ToString("MMMM");
                    break;
                case "Month":
                    var i = parent.Nodes.Count;
                    title = "Week" + (i + 1);
                    break;
                case "Week":
                    title = node.Created.DayOfWeek.ToString();
                    break;
            }

            return title;
        }


        public static bool GetSubNodeContext(string parentContext, out string context)
        {
            context = GetSubNodeContext(parentContext);
            return !string.IsNullOrEmpty(context);
        }
    }
}