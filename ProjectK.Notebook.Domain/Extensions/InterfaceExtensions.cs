using ProjectK.Notebook.Domain.Interfaces;

namespace ProjectK.Notebook.Domain.Extensions
{
    public static class InterfaceExtensions
    {
        public static bool IsSame(this INode a, INode b)
        {
            if (a.Id != b.Id) return false;
            if (a.ParentId != b.ParentId) return false;
            if (a.Name != b.Name) return false;
            if (a.Context != b.Context) return false;
            if (a.Created != b.Created) return false;
            return true;
        }

        public static bool IsSame(this INote a, INote b)
        {
            // INode
            if (!((INode)a).IsSame(b)) return false;
            // ITask
            if (a.Text != b.Text) return false;
            return true;
        }

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

    }
}
