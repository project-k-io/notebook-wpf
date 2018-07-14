using System;

namespace Projects.ViewModels
{
    public class TaskEventArgs : EventArgs
    {
        public TaskViewModel Task { get; set; }
    }
}