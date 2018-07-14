using System.Windows;
using System.Windows.Controls;
using Projects.Views.Helpers;

namespace Projects.Views.Controls.TreeViewList
{
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

        public void SelectItem(object item)
        {
            TreeViewHelper.SelectItem(this, item);
        }

        public void ExpandAll()
        {
            TreeViewHelper.ExpandAll(this);
        }
    }
}