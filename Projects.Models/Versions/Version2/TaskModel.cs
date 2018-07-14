using System;

namespace Projects.Models.Versions.Version2
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
        public static TaskModel NetTask()
        {
            return new TaskModel {Id = Guid.NewGuid(), DateStarted = DateTime.Now};
        }

        public override string ToString()
        {
            return $"{Context}:{Type}:{Title}:{DateStarted}:{DateEnded}";
        }
    }
}