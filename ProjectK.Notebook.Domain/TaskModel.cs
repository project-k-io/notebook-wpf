using System;

namespace ProjectK.Notebook.Domain
{
    public class TaskModel : NodeModel
    {
        public int Level = 0;
        public Guid ParentId { get; set; }
        public int Rating { get; set; }
        public DateTime DateStarted { get; set; }
        public DateTime DateEnded { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public string Description { get; set; }
        public string Context { get; set; }


        public override string ToString()
        {
            return $"{Context}:{Type}:{Name}:{DateStarted}:{DateEnded}";
        }

        public static TaskModel NewTask()
        {
            var task = new TaskModel
            {
                Id = Guid.NewGuid(),
                DateStarted = DateTime.Now
            };
            return task;
        }

        public void Init(Versions.Version2.TaskModel  task2)
        {
            Id = task2.Id;
            Name = task2.Title;
            ParentId = task2.ParentId;
            Rating = task2.Rating;
            DateStarted = task2.DateStarted;
            DateEnded = task2.DateEnded;
            Type = task2.Type;
            SubType = task2.SubType;
            Description = task2.Description;
            Context = task2.Context;
        }

        public bool IsSame(TaskModel b)
        {
            if (Id != b.Id) return false;
            if (Name != b.Name) return false;
            if (ParentId != b.ParentId) return false;
            if (Rating != b.Rating) return false;
            if (DateStarted != b.DateStarted) return false;
            if (DateEnded != b.DateEnded) return false;
            if (Type != b.Type) return false;
            if (SubType != b.SubType) return false;
            if (Description != b.Description) return false;
            if (Context != b.Context) return false;
            return true;
        }

        public TaskModel Copy()
        {
            return new TaskModel
            {
                Id = Id,
                Name = Name,
                ParentId = ParentId,
                Rating = Rating,
                DateStarted = DateStarted,
                DateEnded = DateEnded,
                Type = Type,
                SubType = SubType,
                Description = Description,
                Context = Context
            };
        }
    }
}