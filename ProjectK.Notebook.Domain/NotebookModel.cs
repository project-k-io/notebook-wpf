﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ProjectK.Notebook.Domain.Extensions;
using ProjectK.Notebook.Domain.Interfaces;
using ProjectK.Utils.Extensions;

namespace ProjectK.Notebook.Domain
{
    public class NotebookModel : INode
    {
        [Key] public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string Name { get; set; }
        public string Context { get; set; }
        public DateTime Created { get; set; }
        public string Description { get; set; }

        // Notebook
        public bool NonRoot { get; set; }

        public virtual IList<NodeModel> Nodes { get; set; } = new ObservableCollection<NodeModel>();
        public virtual IList<NoteModel> Notes { get; set; } = new ObservableCollection<NoteModel>();
        public virtual IList<TaskModel> Tasks { get; set; } = new ObservableCollection<TaskModel>();

        public void Clear()
        {
            Nodes.Clear();
            Notes.Clear();
            Tasks.Clear();
        }


        public bool IsSame(NotebookModel target)
        {
            if (!Notes.IsSame(target.Notes, (a, b) => a.IsSame(b)))
                return false;

            if (!Tasks.IsSame(target.Tasks, (a, b) => a.IsSame(b)))
                return false;

            return true;
        }
#if AK
        public NotebookModel Copy()
        {
            var model = new NotebookModel();
            model.Notes.Copy(Notes, a => a.Copy());
            model.Tasks.Copy(Tasks, a => a.Copy());
            return model;
        }
        public void CopyFrom(NotebookModel source)
        {
            Notes.Clear();
            Notes.Copy(source.Notes, a => a.Copy());
            Tasks.Clear();
            Tasks.Copy(source.Tasks, a => a.Copy());
        }

#endif
        public List<INode> GetItems()
        {
            var items = new List<INode>();

            var nodes = Nodes.Cast<NodeModel>();
            var notes = Notes.Cast<NoteModel>();
            var tasks = Tasks.Cast<TaskModel>();

            items.AddRange(nodes);
            items.AddRange(notes);
            items.AddRange(tasks);

            return items;
        }
    }
}