using System;
using System.Collections.Generic;
using ProjectK.Notebook.Models.Interfaces;

namespace ProjectK.Notebook.Models.Extensions;

public static class NodeExtensions
{
    public static void AddToList<T>(this ICollection<T> list, T task) where T : ITreeNode<T>
    {
        list.Add(task);
        foreach (var subTask in task.Nodes)
            AddToList(list, subTask);
    }

    public static bool IsSame(this INode a, INode b)
    {
        if (a.Id != b.Id) return false;
        if (a.ParentId != b.ParentId) return false;
        if (a.Name != b.Name) return false;
        if (a.Context != b.Context) return false;
        if (a.Created != b.Created) return false;
        if (a.Description != b.Description) return false;
        return true;
    }

    public static void Init(this INode a, INode b)
    {
        a.Id = b.Id;
        a.ParentId = b.ParentId;
        a.Name = b.Name;
        a.Created = b.Created;
        a.Context = b.Context;
    }

    public static string Text(this INode a)
    {
        return $"{a.Id} | {a.ParentId} | {a.Name} | {a.Context} | {a.Created} |  {a.Description}";
    }


    public static INode CreateModel(this INode parentNode, Guid notebookId)
    {
        var context = ModelRules.GetSubNodeContext(parentNode.Context);
        if (string.IsNullOrEmpty(context))
            context = "Node";

        INode model;
        // Create Model
        if (context == "Task")
            model = new TaskModel // CreateModel
            {
                DateStarted = DateTime.Now,
                DateEnded = DateTime.Now,
                NotebookId = notebookId
            };
        else
            model = new NodeModel
            {
                Created = DateTime.Now,
                NotebookId = notebookId
            };

        model.Id = Guid.NewGuid();
        model.Context = context;
        return model;
    }

    public static void FixTypes(this INode model, string name)
    {
        if (model is TaskModel task)
            if (string.IsNullOrEmpty(task.Type))
                task.Type = ModelRules.FixTypes(name);
    }
}