using System;
using ProjectK.Notebook.Domain;

namespace ProjectK.Notebook.ViewModels.Extensions
{
    public static class TaskModelExtensions
    {
        public static void Init(this TaskModel task, Models.Versions.Version2.TaskModel task2)
        {
            task.Id = task2.Id;
            task.ParentId = task2.ParentId;
            task.Rating = task2.Rating;
            task.DateStarted = task2.DateStarted;
            task.DateEnded = task2.DateEnded;
            task.Type = task2.Type;
            task.SubType = task2.SubType;
            task.Name = task2.Title;
            task.Description = task2.Description;
            task.Context = task2.Context;
        }

        public static bool IsSame(this TaskModel a, TaskModel b)
        {
            if (a.Id != b.Id) return false;
            if (a.ParentId != b.ParentId) return false;
            if (a.Rating != b.Rating) return false;
            if (a.DateStarted != b.DateStarted) return false;
            if (a.DateEnded != b.DateEnded) return false;
            if (a.Type != b.Type) return false;
            if (a.SubType != b.SubType) return false;
            if (a.Name != b.Name) return false;
            if (a.Description != b.Description) return false;
            if (a.Context != b.Context) return false;
            return true;
        }

        public static TaskModel Copy(this TaskModel a)
        {
            return new TaskModel
            {
                Id = a.Id,
                ParentId = a.ParentId,
                Rating = a.Rating,
                DateStarted = a.DateStarted,
                DateEnded = a.DateEnded,
                Type = a.Type,
                SubType = a.SubType,
                Name = a.Name,
                Description = a.Description,
                Context = a.Context
            };
        }
    }
}
