using System;
using ProjectK.Notebook.Domain.Interfaces;

namespace ProjectK.Notebook.Domain
{ 
    public class NodeModel : IItem 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public string Context { get; set; }
        public DateTime Created { get; set; }

        public Guid ParentId { get; set; }
        public string Type { get; set; }
    }
}