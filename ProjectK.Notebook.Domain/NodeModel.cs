using System;

namespace ProjectK.Notebook.Domain
{ 
    public class NodeModel<T> where T : new()
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public T Data { get; set; } = new T();
    }
}