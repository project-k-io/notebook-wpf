using System.Windows.Controls;

namespace ProjectK.View.Helpers.Extensions;

public static class TreeViewExtensions
{
    public static void ExpandAll(this TreeView treeView)
    {
        treeView.ExpandSubContainers();
    }


    public static void SelectItem(this TreeView treeView, object item)
    {
        treeView.ExpandAndSelectItem(item);
    }
}