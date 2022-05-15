using System;

namespace ProjectK.Notebook.ViewModels.Helpers;

public class TaskEventArgs : EventArgs
{
    public NodeViewModel Task { get; set; }
}