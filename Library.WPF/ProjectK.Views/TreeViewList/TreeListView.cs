using System.Windows;
using System.Windows.Controls;

namespace ProjectK.Toolkit.Wpf.Controls.TreeViewList;

public class TreeListView : TreeView
{
    protected override DependencyObject GetContainerForItemOverride()
    {
        return new TreeListViewItem();
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is TreeListViewItem;
    }
}