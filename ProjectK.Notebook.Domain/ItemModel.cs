using System;
using ProjectK.Notebook.Domain.Interfaces;

namespace ProjectK.Notebook.Domain
{
    public class ItemModel : IItem
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public string Context { get; set; }
        public string Description { get; set; }

        public bool IsSame(ItemModel b)
        {
            if (Id != b.Id) return false;
            if (ParentId != b.ParentId) return false;
            if (Name != b.Name) return false;
            if (Context != b.Context) return false;
            if (Created != b.Created) return false;
            if (Description != b.Description) return false;
            return true;
        }

        public ItemModel()
        {
        }


        public ItemModel(INode b)
        {
            Id = b.Id;
            Name = b.Name;
            Context = b.Context;
            Created = b.Created;
            ParentId = b.ParentId;
            Description = b.Description;
        }
#if AK
        public override string ToString()
        {
            return $"{Context}:{Name}:{Created}: {Description}";
        }

        public void Import(Versions.Version2.TaskModel task2)
        {
        }


        public ItemModel()
        {
        }

        public ItemModel Copy()
        {
            return new ItemModel(this);
        }
#endif
    }
}