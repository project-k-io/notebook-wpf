using System.Collections.Generic;
using System.Diagnostics;

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

            for (var i = 0; i < Tasks.Count; i++)
            {
                var a = Tasks[i];
                var b = data.Tasks[i];
                if (b.Title == "XXX")
                    Debug.WriteLine("XXX");
                if (!a.IsSame(b))
                    return false;
            }

            return true;
        }

        public void Clear()
        {
            Tasks.Clear();
        }

        public DataModel Copy()
        {
            var model = new DataModel();
            foreach (var task in Tasks)
                model.Tasks.Add(task.Copy());

            return model;
        }

        public void Copy(DataModel model)
        {
            Tasks.Clear();
            foreach (var task in model.Tasks)
                Tasks.Add(task.Copy());
        }
    }
}