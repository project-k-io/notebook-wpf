using System.Collections.Generic;
using System.Diagnostics;

namespace ProjectK.Notebook.Domain
{
    public class NotebookModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<TaskModel> Tasks { get; set; } = new List<TaskModel>();


        public  void Init(ProjectK.Notebook.Domain.Versions.Version2.DataModel model)
        {
            foreach (var task2 in model.Tasks)
            {
                var task = new TaskModel();
                task.Init(task2);
                Tasks.Add(task);
            }
        }

        public  bool IsSame(NotebookModel target)
        {
            if (target.Tasks.Count != Tasks.Count)
                return false;

            for (var i = 0; i < Tasks.Count; i++)
            {
                var a = Tasks[i];
                var b = target.Tasks[i];
                if (b.Name == "XXX")
                    Debug.WriteLine("XXX");
                if (!a.IsSame(b))
                    return false;
            }

            return true;
        }

        public  NotebookModel Copy()
        {
            var model = new NotebookModel();
            foreach (var task in Tasks)
                model.Tasks.Add(task.Copy());

            return model;
        }

        public  void CopyFrom(NotebookModel source)
        {
            Tasks.Clear();
            foreach (var task in source.Tasks)
                Tasks.Add(task.Copy());
        }

        public  void Clear()
        {
            Tasks.Clear();
        }

    }
}