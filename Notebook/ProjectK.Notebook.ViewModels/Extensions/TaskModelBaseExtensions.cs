using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectK.Notebook.Domain;

namespace ProjectK.Notebook.ViewModels.Extensions
{
    public static class TaskModelBaseExtensions
    {
        public static void Init(this TaskModelBase task, Models.Versions.Version2.TaskModel task2)
        {
            task.ParentId = task2.ParentId;
            task.Rating = task2.Rating;
            task.DateStarted = task2.DateStarted;
            task.DateEnded = task2.DateEnded;
            task.Type = task2.Type;
            task.SubType = task2.SubType;
            task.Description = task2.Description;
            task.Context = task2.Context;
        }
        public static bool IsSame(this TaskModelBase a, TaskModelBase b)
        {
            if (a.ParentId != b.ParentId) return false;
            if (a.Rating != b.Rating) return false;
            if (a.DateStarted != b.DateStarted) return false;
            if (a.DateEnded != b.DateEnded) return false;
            if (a.Type != b.Type) return false;
            if (a.SubType != b.SubType) return false;
            if (a.Description != b.Description) return false;
            if (a.Context != b.Context) return false;
            return true;
        }

        public static TaskModelBase Copy(this TaskModelBase a)
        {
            return new TaskModelBase
            {
                ParentId = a.ParentId,
                Rating = a.Rating,
                DateStarted = a.DateStarted,
                DateEnded = a.DateEnded,
                Type = a.Type,
                SubType = a.SubType,
                Description = a.Description,
                Context = a.Context
            };
        }

    }
}
