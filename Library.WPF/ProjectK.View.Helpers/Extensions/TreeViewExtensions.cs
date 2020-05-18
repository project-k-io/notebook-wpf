using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ProjectK.View.Helpers.Extensions
{
    public static class TreeViewExtensions
    {
        public static void ExpandAll(this TreeView treeView)
        {
            treeView.ExpandSubContainers();
        }

        private static void ExpandSubContainers(this ItemsControl parentContainer)
        {
            foreach (var obj in parentContainer.Items)
                if (parentContainer.ItemContainerGenerator.ContainerFromItem(obj) is TreeViewItem currentContainer && currentContainer.Items.Count > 0)
                {
                    currentContainer.IsExpanded = true;
                    if (currentContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                    {
                        var eh = (EventHandler) null;
                        eh = (param0, param1) =>
                        {
                            if (currentContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                                return;
                            ExpandSubContainers(currentContainer);
                            currentContainer.ItemContainerGenerator.StatusChanged -= eh;
                        };
                        currentContainer.ItemContainerGenerator.StatusChanged += eh;
                    }
                    else
                    {
                        ExpandSubContainers(currentContainer);
                    }
                }
        }

        public static void SelectItem(this TreeView treeView, object item)
        {
            treeView.ExpandAndSelectItem(item);
        }

        private static bool ExpandAndSelectItem(this ItemsControl itemsControl, object itemToSelect)
        {
            foreach (var obj in itemsControl.Items)
                if (obj == itemToSelect && itemsControl.ItemContainerGenerator.ContainerFromItem(obj) is TreeViewItem treeViewItem)
                {
                    treeViewItem.IsSelected = true;
                    treeViewItem.BringIntoView();
                    treeViewItem.Focus();
                    return true;
                }

            foreach (var obj in itemsControl.Items)
            {
                if (!(itemsControl.ItemContainerGenerator.ContainerFromItem(obj) is TreeViewItem currentContainer) || currentContainer.Items.Count <= 0)
                    continue;

                var isExpanded = currentContainer.IsExpanded;
                currentContainer.IsExpanded = true;
                if (currentContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                {
                    var eh = (EventHandler) null;
                    eh = (param0, param1) =>
                    {
                        if (currentContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                            return;
                        if (!ExpandAndSelectItem(currentContainer, itemToSelect))
                            currentContainer.IsExpanded = false;
                        currentContainer.ItemContainerGenerator.StatusChanged -= eh;
                    };
                    currentContainer.ItemContainerGenerator.StatusChanged += eh;
                }
                else
                {
                    if (ExpandAndSelectItem(currentContainer, itemToSelect))
                        return true;
                    currentContainer.IsExpanded = isExpanded;
                }
            }

            return false;
        }
    }
}