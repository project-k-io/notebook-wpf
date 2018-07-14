// Decompiled with JetBrains decompiler
// Type: Projects.Models.ReportRecord
// Assembly: Projects.Models, Version=1.1.8.29073, Culture=neutral, PublicKeyToken=null
// MVID: 40F42789-FE28-4D3C-8B74-0B7FD98A36C8
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.Models.dll

using System;

namespace Projects.Models
{
  public class ReportRecord
  {
    public int Level { get; set; }

    public string Type { get; set; }

    public string Text { get; set; }

    public TimeSpan Duration { get; set; }

    public ReportRecord()
    {
      this.Level = 1;
      this.Duration = new TimeSpan();
    }

    public override string ToString()
    {
      return string.Format("Level={0,2}, Type={1,5}, Duration={2}",  this.Level,  this.Type,  this.Duration);
    }
  }
}
