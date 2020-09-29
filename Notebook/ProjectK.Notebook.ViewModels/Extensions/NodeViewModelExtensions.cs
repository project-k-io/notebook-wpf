using System;
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

        public static void Execute(this NodeViewModel model,  Action<NodeViewModel> action)
        {
            action(model);
            foreach (var node in model.Nodes)
            {
                node.Execute(action);
            }
        }

        public static void UpAction(this NodeViewModel model, Action<NodeViewModel> action)
        {
            if(model.Parent == null)
                return;

            action(model.Parent);
            model.Parent.UpAction(action);
        }

    }


}
