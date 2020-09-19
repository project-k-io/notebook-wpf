namespace ProjectK.Notebook.Domain
{
    public class NoteModel : NodeModel
    {
        public string Text { get; set; }

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