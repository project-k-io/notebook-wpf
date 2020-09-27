using System;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Domain;
using ProjectK.Notebook.ViewModels.Enums;

namespace ProjectK.Notebook.ViewModels.Extensions
{


    public static class NodeViewModelExtension
    {

        private static readonly ILogger _logger = LogManager.GetLogger<TaskViewModel>();
        public static void KeyboardAction(
            this NodeViewModel item,
            KeyboardKeys keyboardKeys,
            Func<KeyboardStates> getState,
            Action handled,
            Action<NodeViewModel> selectItem, Action<NodeViewModel> expandItem,
            Func<bool> deleteMessageBox,
            Action<Action> dispatcher)
        {
            var state = getState();

            // don't show logging ctl, alt, shift or arrow keys
            if (keyboardKeys != KeyboardKeys.None && state != KeyboardStates.None)
                _logger.LogDebug($"KeyboardAction: {keyboardKeys}, {state}");

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
#if AK // AddNew
                                node.Type = item.Type;
#endif
                                node.Name = item.Name;
                                node.Created = DateTime.Now;
                            }

                            break;
                        default:
                            node = item.AddNew();
                            break;
                    }

                    item.IsSelected = true;
                    selectItem(item);
                    expandItem(item);
                    handled();
                    _logger.LogDebug($"Added [{node.Name}] to [{item.Name}]");
                    break;
                case KeyboardKeys.Delete:
                    if (deleteMessageBox())
                        break;
                    var parent = item.Parent;
                    if (parent == null)
                        break;

                    var num1 = parent.Nodes.IndexOf(item);
                    dispatcher(() => parent.Nodes.Remove(item));

                    var parentNode = num1 > 0 ? parent.Nodes[num1 - 1] : parent;
                    if (parentNode == null)
                        break;

                    selectItem(parentNode);
                    handled();
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
                        parent1.Nodes.Remove(item);
                        var num2 = parent2.Nodes.IndexOf(parent1);
                        parent2.Insert(num2 + 1, item);
                        selectItem(item);
                        handled();
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

                        parent1.Nodes.Remove(item);
                        parentNode2.Add(item);
                        selectItem(item);
                        parent1.IsExpanded = true;
                        item.IsSelected = true;
                        handled();
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
                        parent1.Nodes.Remove(item);
                        parent1.Insert(num2 - 1, item);
                        selectItem(item);
                        parent1.IsExpanded = true;
                        item.IsSelected = true;
                        handled();
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
                        parent1.Nodes.Remove(item);
                        parent1.Insert(num2 + 1, item);
                        selectItem(item);
                        parent1.IsExpanded = true;
                        item.IsSelected = true;
                        handled();
                    }

                    break;
            }
        }

    }
}
