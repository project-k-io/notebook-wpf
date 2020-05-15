using System;

namespace ProjectK.Notebook.Models.Versions.Version2
{
    public class TaskModel
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public int Rating { get; set; }
        public DateTime DateStarted { get; set; }
        public DateTime DateEnded { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Context { get; set; }
        public static TaskModel Copy(TaskModel a)
        {
            var b = new TaskModel
            {
                Id = a.Id,
                ParentId = a.ParentId,
                Rating = a.Rating,
                DateStarted = a.DateStarted,
                DateEnded = a.DateEnded,
                Type = a.Type,
                SubType = a.SubType,
                Title = a.Title,
                Description = a.Description,
                Context = a.Context
            };
            return b;
        }


        public static TaskModel NetTask()
        {
            return new TaskModel { Id = Guid.NewGuid(), DateStarted = DateTime.Now };
        }

        public override string ToString()
        {
            return $"{Context}:{Type}:{Title}:{DateStarted}:{DateEnded}";
        }

        public bool IsSame(TaskModel m)
        {
            if (Id != m.Id) return false;
            if (ParentId != m.ParentId) return false;
            if (Rating != m.Rating) return false;
            if (DateStarted != m.DateStarted) return false;
            if (DateEnded != m.DateEnded) return false;
            if (Type != m.Type) return false;
            if (SubType != m.SubType) return false;
            if (Title != m.Title) return false;
            if (Description != m.Description) return false;
            if (Context != m.Context) return false;
            return true;
        }
    }
}