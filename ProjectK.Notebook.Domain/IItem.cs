using System;

namespace ProjectK.Notebook.Domain
{
    public interface IItem
    {
        Guid Id { get; set; }
        Guid ParentId { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string Context { get; set; }
        string Type { get; set; }
        string SubType { get; set; }
        DateTime DateStarted { get; set; }
        DateTime DateEnded { get; set; }
        int Level { get; set; }
    }
}