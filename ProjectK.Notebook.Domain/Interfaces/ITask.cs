using System;

namespace ProjectK.Notebook.Domain.Interfaces
{
    public interface ITask : IItem
    {
        string SubType { get; set; }
        DateTime DateStarted { get; set; }
        DateTime DateEnded { get; set; }
    }
}