using ProjectK.Notebook.Models.Interfaces;

namespace ProjectK.Notebook.Models.Extensions;

public static class TaskExtensions
{
    public static bool IsSame(this ITask a, ITask b)
    {
        // INode
        if (!((INode)a).IsSame(b)) return false;
        // ITask
        if (a.DateEnded != b.DateEnded) return false;
        if (a.DateStarted != b.DateStarted) return false;
        if (a.SubType != b.SubType) return false;
        if (a.Type != b.Type) return false;
        if (a.Rating != b.Rating) return false;
        return true;
    }

    public static void Init(this ITask a, ITask b)
    {
        // INode
        ((INode)a).Init(b);
        // ITask 
        a.DateEnded = b.DateEnded;
        a.DateStarted = b.DateStarted;
        a.SubType = b.SubType;
        a.Type = b.Type;
        a.Rating = b.Rating;
    }

    public static string Text(this ITask a)
    {
        return $"{((INode)a).Text()} | {a.DateStarted} | {a.DateEnded} | | {a.Type} | {a.SubType} | {a.Rating}";
    }
}