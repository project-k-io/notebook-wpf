using System;
using System.Collections.Generic;

namespace ProjectK.Notebook.Settings
{
    public class AppSettings
    {
        public AppSettings()
        {
            Connections.Add("AlanDatabase", "Data Source=D:\\db\\alan_notebooks.db");
            Connections.Add("TestDatabase", "Data Source=D:\\db\\test_notebooks.db");
        }

        public WindowSettings Window { get; set; } = new WindowSettings();
        public LayoutSettings Layout { get; set; } = new LayoutSettings();
        public Dictionary<string, string> Connections { get; set; } = new Dictionary<string, string>();
        public Guid LastListTaskId { get; set; }
        public Guid LastTreeTaskId { get; set; }
    }
}