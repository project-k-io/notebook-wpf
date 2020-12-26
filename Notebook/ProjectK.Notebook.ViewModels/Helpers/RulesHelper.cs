using System;
using ProjectK.Notebook.Domain.Extensions;
using ProjectK.Notebook.Domain.Interfaces;

namespace ProjectK.Notebook.ViewModels.Helpers
{
    public static class RulesHelper
    {
        public static string GetSubNodeTitle(this NodeViewModel parent, INode node)
        {
            var title = string.Empty;
            switch (parent.Context)
            {
                case "Time Tracker":
                    title = node.Created.ToString("yyyy");
                    break;
                case "Year":
                    title = node.Created.ToString("MMMM");
                    break;
                case "Month":
                    var i = parent.Nodes.Count;
                    title = "Week" + (i + 1);
                    break;
                case "Week":
                    title = node.Created.DayOfWeek.ToString();
                    break;
            }

            return title;
        }

        public static INode CreateModel(this NodeViewModel parentNode, Guid notebookId)
        {
            var model = parentNode.Model.CreateModel(notebookId);
            var title = parentNode.GetSubNodeTitle(model);

            if (!string.IsNullOrEmpty(title))
                model.Name = title;
            else
                model.Name = model.Context;

            return model;
        }
    }
}