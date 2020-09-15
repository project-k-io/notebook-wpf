using System.Collections.Generic;
using System.Diagnostics;

namespace ProjectK.Notebook.Domain
{
    public class NotebookModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<TaskModel> Tasks { get; set; } = new List<TaskModel>();
    }
}