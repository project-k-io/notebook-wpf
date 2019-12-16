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
        }

        public AppModel App { get; set; }


        public class AppModel
        {
            public string RecentFolder { get; set; }
            public string RecentFile { get; set; }
            public Guid LastTreeTaskId { get; set; }
            public Guid LastListTaskId { get; set; }
        }

    }
}