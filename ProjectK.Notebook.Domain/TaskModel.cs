using System;
using System.Text.Json.Serialization;

namespace ProjectK.Notebook.Domain
{
    public class TaskModel : NodeModel<TaskModelBase>
    {

        public override string ToString()
        {
            return $"{Data.Context}:{Data.Type}:{Name}:{Data.DateStarted}:{Data.DateEnded}";
        }

        public static TaskModel NetTask()
        {
            var task = new TaskModel
            {
                Id = Guid.NewGuid(), 
                Data =
                {
                    DateStarted = DateTime.Now
                }
            };
            return task;
        }
    }

    public class TaskModelBase
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
    }
}