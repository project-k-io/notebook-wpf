using System;
using ProjectK.Notebook.Domain.Interfaces;

namespace ProjectK.Notebook.Domain
{ 
    public class NodeModel : IItem 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ParentId { get; set; }
        public string Context { get; set; }
        
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public string Type { get; set; }

        public virtual bool IsSame(NodeModel b)
        {
            if (Id != b.Id) return false;
            if (Name != b.Name) return false;
            if (ParentId != b.ParentId) return false;
            if (Type != b.Type) return false;
            if (Description != b.Description) return false;
            if (Context != b.Context) return false;
            return true;
        }

        public NodeModel()
        {
        }
        public NodeModel(NodeModel b)
        {
            Id = b.Id;
            Name = b.Name;
            ParentId = b.ParentId;
            Type = b.Type;
            Description = b.Description;
            Context = b.Context;
        }

        public virtual NodeModel Copy()
        {
            return new NodeModel(this);
        }

    }
}