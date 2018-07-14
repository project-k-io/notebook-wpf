// Decompiled with JetBrains decompiler
// Type: Projects.Models.Versions.Version2.TaskModel
// Assembly: Projects.Models, Version=1.1.8.29073, Culture=neutral, PublicKeyToken=null
// MVID: 40F42789-FE28-4D3C-8B74-0B7FD98A36C8
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.Models.dll

using System;

namespace Projects.Models.Versions.Version2
{
  public class TaskModel
  {
    public Guid Id { get; set; }

    public Guid ParentId { get; set; }

    public int Rating { get; set; }

    public DateTime DateStarted { get; set; }

    public DateTime DateEnded { get; set; }

    public string Type { get; set; }

    public string SubType { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string Context { get; set; }

    public static TaskModel NetTask()
    {
      return new TaskModel() { Id = Guid.NewGuid(), DateStarted = DateTime.Now };
    }

    public override string ToString()
    {
      return string.Format("{0}:{1}:{2}:{3}:{4}", (object) this.Context, (object) this.Type, (object) this.Title, (object) this.DateStarted, (object) this.DateEnded);
    }
  }
}
