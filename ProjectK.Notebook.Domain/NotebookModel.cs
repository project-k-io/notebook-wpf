﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ProjectK.Utils.Extensions;

namespace ProjectK.Notebook.Domain
{
    public class NotebookModel: NodeModel
    {
        [Key]
        public int Id { get; set; }

        public virtual IList<NoteModel> Notes { get; set; } = new ObservableCollection<NoteModel>();
        public virtual IList<TaskModel> Tasks { get; set; } = new ObservableCollection<TaskModel>();

        public void Init(Versions.Version2.DataModel model)
        {
            foreach (var task2 in model.Tasks)
            {
                var task = new TaskModel();
                task.Init(task2);
                Tasks.Add(task);
            }
        }

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

        public  void Clear()
        {
            Notes.Clear();
            Tasks.Clear();
        }

        public List<NodeModel> GetNodes()
        {
            var nodes = new List<NodeModel>();
            var notes = Notes.Cast<NodeModel>().ToList();
            var tasks = Tasks.Cast<NodeModel>();
            nodes.AddRange(notes);
            nodes.AddRange(tasks);
            return nodes;
        }

    }
}