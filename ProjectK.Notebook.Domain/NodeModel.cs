using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectK.Notebook.Domain.Interfaces;

namespace ProjectK.Notebook.Domain
{
    [Table("Nodes")]
    public class NodeModel : INode
    {
        [Key]
        public int ItemId { get; set; }

        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public string Context { get; set; }


        // Foreign Key
        [ForeignKey("NotebookModel")]
        public int NotebookId { get; set; }
        public virtual NotebookModel NotebookModel { get; set; }

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

        public void Init(Versions.Version2.TaskModel task2)
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