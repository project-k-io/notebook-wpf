using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectK.Notebook.Domain.Interfaces;

namespace ProjectK.Notebook.Domain
{
    public class NoteModel : ItemModel, INote
    {
        [Key]
        public int ItemId { get; set; }

        // INote
        public string Text { get; set; }

        // Foreign Key
        [ForeignKey("NotebookModel")]
        public int NotebookId { get; set; }
        public virtual NotebookModel NotebookModel { get; set; }

#if AK
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
#endif
    }
}