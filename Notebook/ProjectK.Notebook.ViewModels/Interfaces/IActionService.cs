using ProjectK.Notebook.ViewModels.Enums;
using System;

namespace ProjectK.Notebook.ViewModels.Interfaces
{
    public interface IActionService
    {
        Func<KeyboardStates> GetState { get; set; }
        Action Handled { get; set; }
        Action<NodeViewModel> SelectItem { get; set; }
        Action<NodeViewModel> ExpandItem { get; set; }
        Func<bool> DeleteMessageBox { get; set; }
        Action<Action> Dispatcher { get; set; }
    }
}