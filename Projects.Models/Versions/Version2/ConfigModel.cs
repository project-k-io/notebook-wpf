using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Projects.Models.Versions.Version2
{
    [XmlRoot("Config")]
    public class ConfigModel
    {
        public ConfigModel()
        {
            App = new AppModel();
            Layout = new LayoutModel();
        }

        public AppModel App { get; set; }

        public LayoutModel Layout { get; set; }

        public class AppModel
        {
            public string RecentFolder { get; set; }
            public string RecentFile { get; set; }
            public Guid LastTreeTaskId { get; set; }
            public Guid LastListTaskId { get; set; }
        }

        public class LayoutModel
        {
            [DefaultValue(100)] public int LayoutNavigatorWidth { get; set; }
            public double MainWindowTop { get; set; }

            [DefaultValue("100")] public double MainWindowLeft { get; set; }

            [DefaultValue("400")] public double MainWindowWidth { get; set; }

            [DefaultValue("300")] public double MainWindowHeight { get; set; }
        }
    }
}