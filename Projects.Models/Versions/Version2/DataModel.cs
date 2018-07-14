// Decompiled with JetBrains decompiler
// Type: Projects.Models.Versions.Version2.DataModel
// Assembly: Projects.Models, Version=1.1.8.29073, Culture=neutral, PublicKeyToken=null
// MVID: 40F42789-FE28-4D3C-8B74-0B7FD98A36C8
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.Models.dll

using System.Collections.Generic;
using System.IO;

namespace Projects.Models.Versions.Version2
{
    public class DataModel
    {
        public DataModel()
        {
            Tasks = new List<TaskModel>();
        }

        public List<TaskModel> Tasks { get; set; }

        public static string GetTasksFileName(string folderName)
        {
            return Directory.Exists(folderName) ? Path.Combine(folderName, "tasks.xml") : "tasks.xml";
        }
    }
}