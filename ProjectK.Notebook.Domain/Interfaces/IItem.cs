using System;

namespace ProjectK.Notebook.Domain.Interfaces
{
    public interface IItem
    {
        Guid Id { get; set; }
        Guid ParentId { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string Context { get; set; }
        DateTime Created { get; set; }
        string Type { get; set; }
    }
}