using System;
using ProjectK.Notebook.Domain.Interfaces;

namespace ProjectK.Notebook.Domain
{
    public class NodeModel : INode
    {
        public Guid NodeId { get; set; }
        public Guid ParentId { get; set; }
        public string Name { get; set; }
        public string Context { get; set; }
        public DateTime Created { get; set; }


        public virtual bool IsSame(NodeModel b)
        {
            if (NodeId != b.NodeId) return false;
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
            NodeId = b.NodeId;
            Name = b.Name;
            Context = b.Context;
            Created = b.Created;
            ParentId = b.ParentId;
        }

        public NodeModel Copy()
        {
            return new NodeModel(this);
        }
    }
}