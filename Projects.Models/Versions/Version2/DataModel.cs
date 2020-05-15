using System;
using System.Collections.Generic;
using System.IO;

namespace ProjectK.Notebook.Models.Versions.Version2
{
    public class DataModel
    {
        public DataModel()
        {
            Tasks = new List<TaskModel>();
        }

        public List<TaskModel> Tasks { get; set; }

        public bool IsSame(DataModel data)
        {
            if (data.Tasks.Count != Tasks.Count)
                return false;

            for (int i = 0; i < Tasks.Count; i++)
            {
                var a = Tasks[i];
                var b = data.Tasks[i];
                if (!a.IsSame(b))
                    return false;

            }

            return true;
        }

        public void Clear()
        {
            Tasks.Clear();
        }
    }
}