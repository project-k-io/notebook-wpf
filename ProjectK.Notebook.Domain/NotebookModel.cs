using System.Collections.Generic;
using System.Diagnostics;

namespace ProjectK.Notebook.Domain
{
    public class NotebookModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<NodeModel> Nodes { get; set; } = new List<NodeModel>();

        public  void Init(ProjectK.Notebook.Domain.Versions.Version2.DataModel model)
        {
            foreach (var task in model.Tasks)
            {
                var node = new TaskModel();
                node.Init(task);
                Nodes.Add(node);
            }
        }

        public  bool IsSame(NotebookModel target)
        {
            if (target.Nodes.Count != Nodes.Count)
                return false;

            for (var i = 0; i < Nodes.Count; i++)
            {
                var a = Nodes[i];
                var b = target.Nodes[i];
                if (b.Name == "XXX")
                    Debug.WriteLine("XXX");

                if (!a.IsSame(b))
                    return false;
            }

            return true;
        }

        public  NotebookModel Copy()
        {
            var model = new NotebookModel();
            foreach (var node in Nodes)
                model.Nodes.Add(node.Copy());

            return model;
        }

        public  void CopyFrom(NotebookModel source)
        {
            Nodes.Clear();
            foreach (var node in source.Nodes)
                Nodes.Add(node.Copy());
        }

        public  void Clear()
        {
            Nodes.Clear();
        }

    }
}