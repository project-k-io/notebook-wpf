using System;
using ProjectK.Notebook.Domain.Interfaces;

namespace ProjectK.Notebook.Domain
{
    public class Node : INode
    {
        public Guid NodeId { get; set; }
        public Guid ParentId { get; set; }
        public string Name { get; set; }
        public string Context { get; set; }
        public DateTime Created { get; set; }


        public virtual bool IsSame(Node b)
        {
            if (NodeId != b.NodeId) return false;
            if (ParentId != b.ParentId) return false;
            if (Name != b.Name) return false;
            if (Context != b.Context) return false;
            if (Created != b.Created) return false;
            return true;
        }

        public Node()
        {
        }
        public Node(Node b)
        {
            NodeId = b.NodeId;
            Name = b.Name;
            Context = b.Context;
            Created = b.Created;
            ParentId = b.ParentId;
        }

        public Node Copy()
        {
            return new Node(this);
        }
    }
}