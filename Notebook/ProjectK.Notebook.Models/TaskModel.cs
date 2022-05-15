using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using ProjectK.Notebook.Models.Extensions;
using ProjectK.Notebook.Models.Interfaces;

namespace ProjectK.Notebook.Models;

public class TaskModel : IItem
{
    [XmlIgnore] public virtual NotebookModel NotebookModel { get; set; }

    [XmlIgnore]
    public TimeSpan Duration
    {
        get
        {
            if (DateStarted == DateTime.MinValue || DateEnded == DateTime.MinValue)
                return TimeSpan.Zero;

            return DateEnded - DateStarted;
        }
    }

    // ITask
    public string Type { get; set; }
    public string SubType { get; set; }
    public int Rating { get; set; }
    public DateTime DateStarted { get; set; }

    public DateTime DateEnded { get; set; }

    // Foreign Key
    [ForeignKey("NotebookModel")] public Guid NotebookId { get; set; }

    // Primary Key
    [Key] public Guid Id { get; set; }

    // INode
    public Guid ParentId { get; set; }
    public string Name { get; set; }
    public string Context { get; set; }
    public DateTime Created { get; set; }
    public string Description { get; set; }

    public bool IsSame(TaskModel b)
    {
        return ((ITask)this).IsSame(b);
    }

    public void Init(Versions.Version2.TaskModel task2)
    {
        Id = task2.Id;
        ParentId = task2.ParentId;
        Context = task2.Context;
        Name = task2.Title;
        Description = task2.Description;
        Rating = task2.Rating;
        DateStarted = task2.DateStarted;
        DateEnded = task2.DateEnded;
        Type = task2.Type;
        SubType = task2.SubType;
    }

    public override string ToString()
    {
        return this.Text();
    }

    public TaskModel Copy()
    {
        throw new NotImplementedException();
    }
}