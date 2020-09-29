using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectK.Notebook.Domain.Interfaces;

namespace ProjectK.Notebook.Domain
{
    public class NoteModel : INote
    {
        [Key]
        public int ItemId { get; set; }


        // INode
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public string Context { get; set; }

        public bool IsSame(INode b)
        {
            throw new NotImplementedException();
        }

        // INote
        public string Text { get; set; }
        public bool IsSame(INote b)
        {
            throw new NotImplementedException();
        }

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