using System;
using System.Collections;
using System.Collections.Generic;
using ProjectK.Notebook.Models;
using ProjectK.Notebook.Models.Extensions;
using ProjectK.Notebook.Models.Interfaces;

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


        public static void AddToList2(ICollection<NodeViewModel> list, NodeViewModel node, IList dates)
        {
            if (node.Model is TaskModel task)
                if (ModelRules.ContainDate(dates, task.DateStarted))
                    list.Add(node);

            foreach (var subTask in node.Nodes)
                AddToList2(list, subTask, dates);
        }

    }
}