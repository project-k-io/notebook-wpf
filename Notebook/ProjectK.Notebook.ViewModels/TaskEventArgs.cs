using System;

namespace ProjectK.Notebook.ViewModels
{
    public class TaskEventArgs : EventArgs
    {
        public NodeViewModel Task { get; set; }
    }
}