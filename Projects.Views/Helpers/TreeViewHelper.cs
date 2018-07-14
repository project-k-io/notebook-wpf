﻿using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Projects.Views.Helpers
{
    public static class TreeViewHelper
    {
        public static void ExpandAll(this TreeView treeView)
        {
            ExpandSubContainers(treeView);
        }

        private static void ExpandSubContainers(ItemsControl parentContainer)
        {
            foreach (var obj in parentContainer.Items)
            {
                var currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItem;
                if (currentContainer != null && currentContainer.Items.Count > 0)
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
        }

        public static void SelectItem(this TreeView treeView, object item)
        {
            ExpandAndSelectItem(treeView, item);
        }

        private static bool ExpandAndSelectItem(ItemsControl parentContainer, object itemToSelect)
        {
            foreach (var obj in parentContainer.Items)
            {
                var treeViewItem = parentContainer.ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItem;
                if (obj == itemToSelect && treeViewItem != null)
                {
                    treeViewItem.IsSelected = true;
                    treeViewItem.BringIntoView();
                    treeViewItem.Focus();
                    return true;
                }
            }

            foreach (var obj in parentContainer.Items)
            {
                var currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItem;
                if (currentContainer != null && currentContainer.Items.Count > 0)
                {
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
            }

            return false;
        }
    }
}