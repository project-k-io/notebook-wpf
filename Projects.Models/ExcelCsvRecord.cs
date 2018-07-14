// Decompiled with JetBrains decompiler
// Type: Projects.Models.ExcelCsvRecord
// Assembly: Projects.Models, Version=1.1.8.29073, Culture=neutral, PublicKeyToken=null
// MVID: 40F42789-FE28-4D3C-8B74-0B7FD98A36C8
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.Models.dll

using System;

namespace Projects.Models
{
  public class ExcelCsvRecord
  {
    public DateTime Day { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public string Type { get; set; }

    public string Type1 { get; set; }

    public string Task { get; set; }

    public string Type2 { get; set; }

    public string SubTask { get; set; }

    public override string ToString()
    {
      return string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}",  this.Day,  this.Start,  this.End,  this.Type,  this.Type1,  this.Task,  this.Type2,  this.SubTask);
    }

    public bool TryParse(string line)
    {
      string[] strArray = line.Split(',');
      DateTime result;
      if (!DateTime.TryParse(strArray[1], out result))
        return false;
      this.Day = result;
      if (!DateTime.TryParse(strArray[2], out result))
        return false;
      this.Start = result;
      if (!DateTime.TryParse(strArray[3], out result))
        return false;
      this.End = result;
      this.Type = strArray[9];
      this.Type1 = strArray[10];
      this.Task = strArray[11];
      this.Type2 = strArray[12];
      this.SubTask = strArray[13];
      return true;
    }
  }
}
