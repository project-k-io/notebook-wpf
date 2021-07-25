using ProjectK.Notebook.ViewModels.Enums;
using ProjectK.Notebook.ViewModels.Interfaces;
using System;

namespace ProjectK.Notebook.ViewModels.Services
{
    public class ActionService : IActionService
    {
        public Func<KeyboardStates> GetState { get; set; }
        public Action Handled { get; set; }
        public Action<NodeViewModel> SelectItem { get; set; }
        public Action<NodeViewModel> ExpandItem { get; set; }
        public Func<bool> DeleteMessageBox { get; set; }
        public Action<Action> Dispatcher { get; set; }
    }
}