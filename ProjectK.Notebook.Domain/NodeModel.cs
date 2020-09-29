using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectK.Notebook.Domain.Interfaces;

namespace ProjectK.Notebook.Domain
{
    public class NodeModel : INode
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public string Context { get; set; }

        public bool IsSame(NodeModel b)
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


        public NodeModel(INode b)
        {
            Id = b.Id;
            Name = b.Name;
            Context = b.Context;
            Created = b.Created;
            ParentId = b.ParentId;
        }
#if AK
        public override string ToString()
        {
            return $"{Context}:{Name}:{Created}: {Description}";
        }

        public void Import(Versions.Version2.TaskModel task2)
        {
        }


        public NodeModel()
        {
        }

        public NodeModel Copy()
        {
            return new NodeModel(this);
        }
#endif
    }
}