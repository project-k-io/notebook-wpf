using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectK.Notebook.Domain
{
    public class NoteModel : NodeModel
    {
        [Key]
        public int Id { get; set; }

        public string Text { get; set; }

        [ForeignKey("NotebookModel")]
        public int NotebookId { get; set; }
        public virtual NotebookModel NotebookModel { get; set; }

        public NoteModel()
        {
        }
        public NoteModel(NoteModel b) : base(b)
        {
            Text = b.Text;
        }

        public new NoteModel Copy()
        {
            return new NoteModel(this);
        }
    }
}