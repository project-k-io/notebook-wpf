using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectK.Notebook.Domain.Interfaces;

namespace ProjectK.Notebook.Domain
{
    // [Table("Nodes")]
    public class NodeModel :  ItemModel, INode
    {
        [Key]
        public int ItemId { get; set; }

        // Foreign Key
        [ForeignKey("NotebookModel")]
        public int NotebookId { get; set; }
        public virtual NotebookModel NotebookModel { get; set; }
    }
}