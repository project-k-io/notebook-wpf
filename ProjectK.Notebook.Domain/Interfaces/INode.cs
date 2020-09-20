using System;

namespace ProjectK.Notebook.Domain.Interfaces
{
    public interface INode
    {
        Guid NodeId { get; set; }
        Guid ParentId { get; set; }
        string Name { get; set; }
        DateTime Created { get; set; }
        string Context { get; set; }
    }
}