using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectK.Notebook.Domain.Interfaces;

namespace ProjectK.Notebook.Domain
{
    public class TaskModel : NodeModel, ITask
    {
        // Primary Key
        [Key]
        public int Id { get; set; }

        // Data
        public string Type { get; set; }
        public string SubType { get; set; }
        public int Rating { get; set; }
        public DateTime DateStarted { get; set; }
        public DateTime DateEnded { get; set; }

        // Foreign Key
        [ForeignKey("NotebookModel")]
        public int NotebookId { get; set; }
        public virtual NotebookModel NotebookModel { get; set; }


        public override string ToString()
        {
            return $"{base.ToString()}:{Type}:{SubType}: {Rating}: {DateStarted}:{DateEnded}";
        }

        public static TaskModel NewTask()
        {
            var task = new TaskModel
            {
                NodeId = Guid.NewGuid(),
                DateStarted = DateTime.Now
            };
            return task;
        }

        public void Init(Versions.Version2.TaskModel task2)
        {
            base.Init(task2);
            Rating = task2.Rating;
            DateStarted = task2.DateStarted;
            DateEnded = task2.DateEnded;
            Type = task2.Type;
            SubType = task2.SubType;
        }


        public bool IsSame(TaskModel b)
        {
            if (!base.IsSame(b)) return false;

            if (Rating != b.Rating) return false;
            if (DateStarted != b.DateStarted) return false;
            if (DateEnded != b.DateEnded) return false;
            if (SubType != b.SubType) return false;
            if (Type != b.Type) return false;
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
        }

        public new TaskModel Copy()
        {
            return new TaskModel(this);
        }
    }
}