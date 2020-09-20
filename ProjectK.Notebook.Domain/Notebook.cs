using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ProjectK.Utils.Extensions;

namespace ProjectK.Notebook.Domain
{
    public class Notebook
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual IList<Note> Notes { get; set; } = new ObservableCollection<Note>();
        public virtual IList<Task> Tasks { get; set; } = new ObservableCollection<Task>();

        public void Init(ProjectK.Notebook.Domain.Versions.Version2.DataModel model)
        {
            foreach (var task2 in model.Tasks)
            {
                var task = new Task();
                task.Init(task2);
                Tasks.Add(task);
            }
        }

        public  bool IsSame(Notebook target)
        {
            if (!Notes.IsSame(target.Notes, (a, b) => a.IsSame(b)))
                return false;

            if (!Tasks.IsSame(target.Tasks, (a, b) => a.IsSame(b)))
                return false;

            return true;
        }

        public  Notebook Copy()
        {
            var model = new Notebook();
            model.Notes.Copy(Notes, a => a.Copy());
            model.Tasks.Copy(Tasks, a => a.Copy());
            return model;
        }
        public void CopyFrom(Notebook source)
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
    }
}