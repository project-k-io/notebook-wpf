using System;

namespace ProjectK.Notebook.Domain
{
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