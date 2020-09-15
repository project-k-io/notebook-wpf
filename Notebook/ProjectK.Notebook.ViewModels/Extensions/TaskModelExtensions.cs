using System;
using ProjectK.Notebook.Domain;

namespace ProjectK.Notebook.ViewModels.Extensions
{
    public static class TaskModelExtensions
    {

        public static void Init(this TaskModel task, Models.Versions.Version2.TaskModel task2)
        {
            task.Id = task2.Id;
            task.Name = task2.Title;
            task.Data.Init(task2);
        }

        public static bool IsSame(this TaskModel a, TaskModel b)
        {
            if (a.Id != b.Id) return false;
            if (a.Name != b.Name) return false;
            if (!a.Data.IsSame(b.Data)) return false;
            return true;
        }

        public static TaskModel Copy(this TaskModel a)
        {
            return new TaskModel
            {
                Id = a.Id,
                Name = a.Name,
                Data = a.Data.Copy(),
            };
        }
    }
}
