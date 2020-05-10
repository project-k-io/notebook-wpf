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
    }
}