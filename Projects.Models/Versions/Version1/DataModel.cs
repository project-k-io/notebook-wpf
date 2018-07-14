// Decompiled with JetBrains decompiler
// Type: Projects.Models.Versions.Version1.DataModel
// Assembly: Projects.Models, Version=1.1.8.29073, Culture=neutral, PublicKeyToken=null
// MVID: 40F42789-FE28-4D3C-8B74-0B7FD98A36C8
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.Models.dll

using System.Xml.Serialization;
using Vibor.Generic.Models;

namespace Projects.Models.Versions.Version1
{
  [XmlRoot("ProjectViewModel")]
  public class DataModel
  {
    public TaskModel RootTask { get; set; }

    public static DataModel ReadFromFile(string file)
    {
      return XFile2.ReadFromXmlFile<DataModel>(file);
    }
  }
}
