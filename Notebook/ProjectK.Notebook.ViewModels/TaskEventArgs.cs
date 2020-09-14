using System;

namespace ProjectK.Notebook.ViewModels
{
    public class TaskEventArgs : EventArgs
    {
        public NodeViewModel Node { get; set; }
    }
}