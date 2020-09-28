using System;
using ProjectK.Notebook.Domain.Interfaces;

namespace ProjectK.Notebook.Domain
{
    public class NodeModel : INode
    {
        public Guid NodeId { get; set; }
        public Guid ParentId { get; set; }
        public DateTime Created { get; set; }
        public string Context { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public override string ToString()
        {
            return $"{Context}:{Name}:{Created}: {Description}";
        }

        public void Init(Versions.Version2.TaskModel task2)
        {
            NodeId = task2.Id;
            ParentId = task2.ParentId;
            Context = task2.Context;
            Name = task2.Title;
            Description = task2.Description;
        }

        public virtual bool IsSame(NodeModel b)
        {
            if (NodeId != b.NodeId) return false;
            if (ParentId != b.ParentId) return false;
            if (Name != b.Name) return false;
            if (Description != b.Description) return false;
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
            Description = b.Description;
        }

        public NodeModel Copy()
        {
            return new NodeModel(this);
        }
    }
}