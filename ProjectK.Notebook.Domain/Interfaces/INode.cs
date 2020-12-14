using System;

namespace ProjectK.Notebook.Domain.Interfaces
{
    public interface ICommon<in T>
    {
        bool IsSame(T a);
        bool Init(T a);
    }

    public interface INode
    {
        Guid Id { get; set; }
        Guid ParentId { get; set; }
        string Name { get; set; }
        string Context { get; set; }
        DateTime Created { get; set; }
        string Description { get; set; }
    }
}