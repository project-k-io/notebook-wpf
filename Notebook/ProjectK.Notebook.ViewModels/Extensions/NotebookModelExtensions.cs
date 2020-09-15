using System.Diagnostics;
using ProjectK.Notebook.Domain;

namespace ProjectK.Notebook.ViewModels.Extensions
{
    public static class NotebookModelExtensions
    {
        public static void Init(this NotebookModel notebook, ProjectK.Notebook.Models.Versions.Version2.DataModel model)
        {
            foreach (var task2 in model.Tasks)
            {
                var task = new TaskModel();
                task.Init(task2);
                notebook.Tasks.Add(task);
            }
        }

        public static bool IsSame(this NotebookModel source, NotebookModel target)
        {
            if (target.Tasks.Count != source.Tasks.Count)
                return false;

            for (var i = 0; i < source.Tasks.Count; i++)
            {
                var a = source.Tasks[i];
                var b = target.Tasks[i];
                if (b.Name == "XXX")
                    Debug.WriteLine("XXX");
                if (!a.IsSame(b))
                    return false;
            }

            return true;
        }

        public static NotebookModel Copy(this NotebookModel notebook)
        {
            var model = new NotebookModel();
            foreach (var task in notebook.Tasks)
                model.Tasks.Add(task.Copy());

            return model;
        }

        public static void CopyFrom(this NotebookModel target, NotebookModel source)
        {
            target.Tasks.Clear();
            foreach (var task in source.Tasks)
                target.Tasks.Add(task.Copy());
        }

        public static void Clear(this NotebookModel notebook)
        {
            notebook.Tasks.Clear();
        }

    }
}
