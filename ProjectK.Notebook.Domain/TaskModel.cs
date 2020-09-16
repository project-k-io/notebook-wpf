using System;
using ProjectK.Notebook.Domain.Interfaces;

namespace ProjectK.Notebook.Domain
{
    public class TaskModel : NodeModel, ITask
    {
        public int Rating { get; set; }
        public DateTime DateStarted { get; set; }
        public DateTime DateEnded { get; set; }
        public string SubType { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }



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

        public override bool IsSame(NodeModel b)
        {
            return IsSame((TaskModel) b);
        }

        public bool IsSame(TaskModel b)
        {
            if(!base.IsSame(b)) return false;
            if (Rating != b.Rating) return false;
            if (DateStarted != b.DateStarted) return false;
            if (DateEnded != b.DateEnded) return false;
            if (SubType != b.SubType) return false;
            if (Type != b.Type) return false;
            if (Description != b.Description) return false;
            return true;
        }

        public TaskModel()
        {
        }
        public TaskModel(TaskModel b) : base(b)
        {
            Rating = b.Rating;
            DateStarted = b.DateStarted;
            DateEnded = b.DateEnded;
            SubType = b.SubType;
            Type = b.Type;
            Description = b.Description;
        }

        public override NodeModel Copy()
        {
            return new TaskModel(this);
        }
    }
}