using ProjectK.Notebook.Domain.Interfaces;

namespace ProjectK.Notebook.Domain.Extensions
{
    public static class NoteExtensions
    {
        public static bool IsSame(this INote a, INote b)
        {
            // INode
            if (!((INode) a).IsSame(b)) return false;
            // INote
            if (a.Text != b.Text) return false;
            return true;
        }

        public static void Init(this INote a, INote b)
        {
            // INode
            ((INode) a).Init(b);
            // INote 
            a.Text = b.Text;
        }
    }
}