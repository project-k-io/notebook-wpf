using System;

namespace ProjectK.Notebook.Domain.Interfaces
{
    public interface ITask : IItem
    {
        string Description { get; set; }
        string Type { get; set; }
        string SubType { get; set; }
        DateTime DateStarted { get; set; }
        DateTime DateEnded { get; set; }
    }
}