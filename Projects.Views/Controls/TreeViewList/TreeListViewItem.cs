// Decompiled with JetBrains decompiler
// Type: Projects.Views.Controls.TreeViewList.TreeListViewItem
// Assembly: Projects.Views, Version=1.1.8.29122, Culture=neutral, PublicKeyToken=null
// MVID: BCB19E50-AB69-4CA9-9CF4-1A9C4DEAF8F2
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.Views.dll

using System.Windows;
using System.Windows.Controls;

namespace Projects.Views.Controls.TreeViewList
{
    public class TreeListViewItem : TreeViewItem
    {
        private int _level = -1;

        public int Level
        {
            get
            {
                if (_level == -1)
                {
                    var treeListViewItem = ItemsControlFromItemContainer(this) as TreeListViewItem;
                    _level = treeListViewItem != null ? treeListViewItem.Level + 1 : 0;
                }

                return _level;
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }
    }
}