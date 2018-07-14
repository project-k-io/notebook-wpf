// Decompiled with JetBrains decompiler
// Type: Projects.Models.Versions.Version1.TaskModel
// Assembly: Projects.Models, Version=1.1.8.29073, Culture=neutral, PublicKeyToken=null
// MVID: 40F42789-FE28-4D3C-8B74-0B7FD98A36C8
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.Models.dll

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Projects.Models.Versions.Version1
{
  [XmlRoot("TaskViewModel")]
  public class TaskModel
  {
    public int Rating { get; set; }

    [XmlArrayItem("TaskViewModel")]
    public List<TaskModel> SubTasks { get; set; }

    public bool IsSelected { get; set; }

    public bool IsExpanded { get; set; }

    public string Description { get; set; }

    public string Type { get; set; }

    public DateTime DateStarted { get; set; }

    public DateTime DateEnded { get; set; }

    public DateTime TimeStarted { get; set; }

    public DateTime TimeEnded { get; set; }

    public string Title { get; set; }
  }
}
