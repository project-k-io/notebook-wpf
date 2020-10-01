using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Domain;
using ProjectK.Notebook.ViewModels.Enums;
using ProjectK.Notebook.ViewModels.Interfaces;
using ProjectK.Notebook.ViewModels.Services;
using ProjectK.Utils;

namespace ProjectK.Notebook.ViewModels.Extensions
{
    public static class NodeViewModelExtensions
    {
        private static readonly ILogger Logger = LogManager.GetLogger<TaskViewModel>();

        public static void Execute(this NodeViewModel model, Action<NodeViewModel> action)
        {
            action(model);
            foreach (var node in model.Nodes)
            {
                node.Execute(action);
            }
        }

        public static void UpAction(this NodeViewModel model, Action<NodeViewModel> action)
        {
            if (model.Parent == null)
                return;

            action(model.Parent);
            model.Parent.UpAction(action);
        }

        public static List<dynamic> GetModels(this IList<NodeViewModel> nodes)
        {
            var models = new List<dynamic>();
            foreach (var node in nodes)
            {
                node.Execute(n => models.Add(n.Model));
            }
            return models;
        }
    }


}
