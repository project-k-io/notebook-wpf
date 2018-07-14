using System;

namespace Projects.ViewModels
{
    internal class ActivityViewModel
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Type { get; set; }
        public string Task { get; set; }
        public string Action { get; set; }
    }
}