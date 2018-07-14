// Decompiled with JetBrains decompiler
// Type: Projects.Views.Controls.TreeViewList.TreeListView
// Assembly: Projects.Views, Version=1.1.8.29122, Culture=neutral, PublicKeyToken=null
// MVID: BCB19E50-AB69-4CA9-9CF4-1A9C4DEAF8F2
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Projects.Views.dll

using Projects.Views.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace Projects.Views.Controls.TreeViewList
{
  public class TreeListView : TreeView
  {
    protected override DependencyObject GetContainerForItemOverride()
    {
      return (DependencyObject) new TreeListViewItem();
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
