// Decompiled with JetBrains decompiler
// Type: Projects.Models.Versions.Version2.ConfigModel
// Assembly: Projects.Models, Version=1.1.8.29073, Culture=neutral, PublicKeyToken=null
// MVID: 40F42789-FE28-4D3C-8B74-0B7FD98A36C8
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.Models.dll

using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Projects.Models.Versions.Version2
{
    [XmlRoot("Config")]
    public class ConfigModel
    {
        public ConfigModel.AppModel App { get; set; }

        public ConfigModel.LayoutModel Layout { get; set; }

        public ConfigModel()
        {
            App = new ConfigModel.AppModel();
            Layout = new ConfigModel.LayoutModel();
        }

        public class AppModel
        {
            public string RecentFolder { get; set; }

            public string RecentFile { get; set; }

            public Guid LastTreeTaskId { get; set; }

            public Guid LastListTaskId { get; set; }
        }

        public class LayoutModel
        {
            [DefaultValue(100)]
            public int LayoutNavigatorWidth { get; set; }

            public double MainWindowTop { get; set; }

            [DefaultValue("100")]
            public double MainWindowLeft { get; set; }

            [DefaultValue("400")]
            public double MainWindowWidth { get; set; }

            [DefaultValue("300")]
            public double MainWindowHeight { get; set; }
        }
    }
}
