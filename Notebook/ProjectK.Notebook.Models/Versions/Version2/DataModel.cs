using System.Collections.Generic;
using System.Diagnostics;

namespace ProjectK.Notebook.Models.Versions.Version2;

public class DataModel
{
    public int Id { get; set; }
    public string Name { get; set; }

    public virtual List<TaskModel> Tasks { get; set; } = new();

    public bool IsSame(DataModel notebook)
    {
        if (notebook.Tasks.Count != Tasks.Count)
            return false;

        for (var i = 0; i < Tasks.Count; i++)
        {
            var a = Tasks[i];
            var b = notebook.Tasks[i];
            if (b.Title == "XXX")
                Debug.WriteLine("XXX");
            if (!a.IsSame(b))
                return false;
        }

        return true;
    }

    public void Clear()
    {
        Tasks.Clear();
    }
}