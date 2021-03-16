using System;
using System.Collections.Generic;

namespace ProjectK.Notebook.WinApp.Models
{
    public class AppSettings
    {
        public AppSettings()
        {
            Connections.Add("AlanDatabase", "Sample Source=D:\\db\\alan_notebooks.db");
            Connections.Add("TestDatabase", "Sample Source=D:\\db\\test_notebooks.db");
        }
        public LayoutSettingsModel Layout { get; set; } = new();
        public Dictionary<string, string> Connections { get; set; } = new();
        public Guid LastListTaskId { get; set; }
        public Guid LastTreeTaskId { get; set; }
    }
}