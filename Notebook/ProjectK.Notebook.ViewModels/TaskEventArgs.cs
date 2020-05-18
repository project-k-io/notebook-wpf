using System;

namespace ProjectK.Notebook.ViewModels
{
    public class TaskEventArgs : EventArgs
    {
        public TaskViewModel Task { get; set; }
    }
}