using System;

namespace ProjectK.Notebook.Models.Versions.Version2;

public class TaskModel
{
    public int Level = 0;
    public string Title { get; set; }
    public Guid Id { get; set; }
    public Guid ParentId { get; set; }
    public int Rating { get; set; }
    public DateTime DateStarted { get; set; }
    public DateTime DateEnded { get; set; }
    public string Type { get; set; }
    public string SubType { get; set; }
    public string Description { get; set; }
    public string Context { get; set; }

    public override string ToString()
    {
        return $"{Context}:{Type}:{Title}:{DateStarted}:{DateEnded}";
    }

    public bool IsSame(TaskModel m)
    {
        if (Id != m.Id) return false;
        if (ParentId != m.ParentId) return false;
        if (Rating != m.Rating) return false;
        if (DateStarted != m.DateStarted) return false;
        if (DateEnded != m.DateEnded) return false;
        if (Type != m.Type) return false;
        if (SubType != m.SubType) return false;
        if (Title != m.Title) return false;
        if (Description != m.Description) return false;
        if (Context != m.Context) return false;
        return true;
    }
}