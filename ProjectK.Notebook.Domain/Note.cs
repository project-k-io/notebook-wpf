using System.ComponentModel.DataAnnotations;

namespace ProjectK.Notebook.Domain
{
    public class Note : Node
    {
        [Key]
        public int NoteId { get; set; }

        public string Text { get; set; }
        public int NotebookId { get; set; }
        public virtual Notebook Notebook { get; set; }

        public Note()
        {
        }
        public Note(Note b) : base(b)
        {
            Text = b.Text;
        }

        public new Note Copy()
        {
            return new Note(this);
        }
    }
}