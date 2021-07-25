using ProjectK.Notebook.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace ProjectK.Notebook.ViewModels.Extensions
{
    public static class NodeViewModelExtensions
    {
        public static void Execute(this NodeViewModel model, Action<NodeViewModel> action)
        {
            action(model);
            foreach (var node in model.Nodes) node.Execute(action);
        }

        public static void UpAction(this NodeViewModel model, Action<NodeViewModel> action)
        {
            if (model.Parent == null)
                return;

            action(model.Parent);
            model.Parent.UpAction(action);
        }

        public static List<INode> GetModels(this IList<NodeViewModel> nodes)
        {
            var models = new List<INode>();
            foreach (var node in nodes) node.Execute(n => models.Add(n.Model));
            return models;
        }
    }
}