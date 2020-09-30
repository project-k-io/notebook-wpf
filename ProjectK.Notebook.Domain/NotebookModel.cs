using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ProjectK.Notebook.Domain.Interfaces;
using ProjectK.Utils.Extensions;

namespace ProjectK.Notebook.Domain
{
    public class NotebookModel: ItemModel, INode
    {
        [Key]
        public int ItemId { get; set; }
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

        public void Import(Versions.Version2.DataModel model)
        {
            foreach (var task2 in model.Tasks)
            {
                var task = new TaskModel();
                task.Init(task2);
                Tasks.Add(task);
            }
        }

        public void AddNode(dynamic model)
        {
            if (model is TaskModel task)
                Tasks.Add(task);
            else if (model is NoteModel note)
                Notes.Add(note);
            else if (model is NodeModel node)
                Nodes.Add(node);
        }


#if AK


        public  bool IsSame(NotebookModel target)
        {
            if (!Notes.IsSame(target.Notes, (a, b) => a.IsSame(b)))
                return false;

            if (!Tasks.IsSame(target.Tasks, (a, b) => a.IsSame(b)))
                return false;

            return true;
        }
        public  NotebookModel Copy()
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
        public List<ItemModel> GetItems()
        {
            var items = new List<ItemModel>();

            var nodes = Nodes.Cast<NodeModel>();
            var notes = Notes.Cast<ItemModel>();
            var tasks = Tasks.Cast<TaskModel>();

            items.AddRange(nodes);
            items.AddRange(notes);
            items.AddRange(tasks);

            return items;
        }
    }
}