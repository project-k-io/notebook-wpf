using System;

namespace ProjectK.Notebook.Models.Interfaces;

public interface IItem : INode
{
    Guid NotebookId { get; set; }
}