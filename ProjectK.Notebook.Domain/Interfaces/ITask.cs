using System;

namespace ProjectK.Notebook.Domain.Interfaces
{
    public interface ITask : INode
    {
        string Type { get; set; }
        string SubType { get; set; }
        DateTime DateStarted { get; set; }
        DateTime DateEnded { get; set; }
        public int Rating { get; set; }
    }
}