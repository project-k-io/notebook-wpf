using ProjectK.Notebook.Domain.Interfaces;

namespace ProjectK.Notebook.Domain.Extensions
{
    public static class NodeExtensions
    {
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
    }
}