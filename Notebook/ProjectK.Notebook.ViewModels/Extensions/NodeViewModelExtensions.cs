using System;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Domain;
using ProjectK.Notebook.ViewModels.Enums;
using ProjectK.Notebook.ViewModels.Interfaces;
using ProjectK.Notebook.ViewModels.Services;
using ProjectK.Utils;

namespace ProjectK.Notebook.ViewModels.Extensions
{
    public static class NodeViewModelExtensions
    {
        private static readonly ILogger Logger = LogManager.GetLogger<TaskViewModel>();
        public static void KeyboardAction(this NodeViewModel item, KeyboardKeys keyboardKeys, IActionService service)
        {
            var state = service.GetState();

            // don't show logging ctl, alt, shift or arrow keys
            if (keyboardKeys != KeyboardKeys.None && state != KeyboardStates.None)
                Logger.LogDebug($"KeyboardAction: {keyboardKeys}, {state}");

            switch (keyboardKeys)
            {
                case KeyboardKeys.Insert:
                    NodeViewModel node;
                    switch (state)
                    {
                        case KeyboardStates.IsShiftPressed:
                            node = item.Parent.AddNew();
                            node.Created = DateTime.Now;
                            break;
                        case KeyboardStates.IsControlPressed:
                            var lastSubNode = item.Parent.LastSubNode;
                            node = item.Parent.AddNew();

                            if (lastSubNode != null)
                            {
                                node.Name = item.Name;
                                node.Created = DateTime.Now;
                            }

                            break;
                        default:
                            node = item.AddNew();
                            break;
                    }

                    item.IsSelected = true;
                    service.SelectItem(item);
                    service.ExpandItem(item);
                    service.Handled();
                    Logger.LogDebug($"Added [{node.Name}] to [{item.Name}]");
                    break;
                case KeyboardKeys.Delete:
                    if (service.DeleteMessageBox())
                        break;

                    var parent = item.Parent;
                    if (parent == null)
                        break;

                    var num1 = parent.Nodes.IndexOf(item);
                    service.Dispatcher(() => parent.Remove(item));

                    var parentNode = num1 > 0 ? parent.Nodes[num1 - 1] : parent;
                    if (parentNode == null)
                        break;

                    service.SelectItem(parentNode);
                    service.Handled();
                    break;

                case KeyboardKeys.Left:
                    if (state == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = item.Parent;
                        if (parent1 == null)
                            break;
                        var parent2 = parent1.Parent;
                        if (parent2 == null)
                            break;

                        parent1.Remove(item);

                        var num2 = parent2.Nodes.IndexOf(parent1);
                        parent2.Insert(num2 + 1, item);
                        service.SelectItem(item);
                        service.Handled();
                    }

                    break;
                case KeyboardKeys.Right:
                    if (state == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = item.Parent;
                        if (parent1 == null)
                            break;
                        var num2 = parent1.Nodes.IndexOf(item);
                        if (num2 <= 0)
                            break;

                        var parentNode2 = parent1.Nodes[num2 - 1];
                        if (parentNode2 == null)
                            break;

                        parent1.Remove(item);
                        parentNode2.Add(item);
                        service.SelectItem(item);
                        parent1.IsExpanded = true;
                        item.IsSelected = true;
                        service.Handled();
                    }

                    break;
                case KeyboardKeys.Up:
                    if (state == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = item.Parent;
                        if (parent1 == null)
                            break;
                        var num2 = parent1.Nodes.IndexOf(item);
                        if (num2 <= 0)
                            break;
                        parent1.Remove(item);
                        parent1.Insert(num2 - 1, item);
                        service.SelectItem(item);
                        parent1.IsExpanded = true;
                        item.IsSelected = true;
                        service.Handled();
                    }

                    break;
                case KeyboardKeys.Down:
                    if (state == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = item.Parent;
                        if (parent1 == null)
                            break;
                        var num2 = parent1.Nodes.IndexOf(item);
                        if (num2 >= parent1.Nodes.Count - 1)
                            break;
                        parent1.Remove(item);
                        parent1.Insert(num2 + 1, item);
                        service.SelectItem(item);
                        parent1.IsExpanded = true;
                        item.IsSelected = true;
                        service.Handled();
                    }

                    break;
            }
        }

        public static void Execute(this NodeViewModel model,  Action<NodeViewModel> action)
        {
            action(model);
            foreach (var node in model.Nodes)
            {
                node.Execute(action);
            }
        }

        public static void UpAction(this NodeViewModel model, Action<NodeViewModel> action)
        {
            if(model.Parent == null)
                return;

            action(model.Parent);
            model.Parent.UpAction(action);
        }

    }


}
