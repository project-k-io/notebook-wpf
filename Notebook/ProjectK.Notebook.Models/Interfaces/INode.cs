using System;

namespace ProjectK.Notebook.Models.Interfaces;

public interface INode
{
    Guid Id { get; set; }
    Guid ParentId { get; set; }
    string Name { get; set; }
    string Context { get; set; }
    DateTime Created { get; set; }
    string Description { get; set; }
}