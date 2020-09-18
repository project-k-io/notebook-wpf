using System;
using ProjectK.Notebook.Domain.Interfaces;

namespace ProjectK.Notebook.Domain
{
    public class BaseModel : INode
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string Name { get; set; }
        public string Context { get; set; }
        public DateTime Created { get; set; }

    }

    public class NodeModel : BaseModel
    {
        public virtual bool IsSame(NodeModel b)
        {
            if (Id != b.Id) return false;
            if (ParentId != b.ParentId) return false;
            if (Name != b.Name) return false;
            if (Context != b.Context) return false;
            if (Created != b.Created) return false;
            return true;
        }

        public NodeModel()
        {
        }
        public NodeModel(NodeModel b)
        {
            Id = b.Id;
            Name = b.Name;
            Context = b.Context;
            Created = b.Created;
            ParentId = b.ParentId;
        }

        public virtual NodeModel Copy()
        {
            return new NodeModel(this);
        }

    }
}