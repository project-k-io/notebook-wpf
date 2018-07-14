// Decompiled with JetBrains decompiler
// Type: Vibor.View.Helpers.Misc.ListViewSorterHelper
// Assembly: Vibor.View.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 66A7410A-B003-4703-9434-CC2ABBB24103
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.View.Helpers.dll

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Vibor.View.Helpers.Misc
{
  public class ListViewSorterHelper
  {
    private ListSortDirection _lastDirection = ListSortDirection.Ascending;
    private GridViewColumnHeader _lastHeaderClicked;

    public void Clicked(FrameworkElement parent, object sender, RoutedEventArgs e)
    {
      ListView lv = sender as ListView;
      if (lv == null)
        return;
      GridViewColumnHeader originalSource = e.OriginalSource as GridViewColumnHeader;
      if (originalSource == null || originalSource.Role == GridViewColumnHeaderRole.Padding)
        return;
      ListSortDirection direction = originalSource == this._lastHeaderClicked ? (this._lastDirection != ListSortDirection.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending) : ListSortDirection.Ascending;
      string header = originalSource.Column.Header as string;
      this.Sort(lv, header, direction);
      originalSource.Column.HeaderTemplate = direction != ListSortDirection.Ascending ? parent.Resources[(object) "HeaderTemplateArrowDown"] as DataTemplate : parent.Resources[(object) "HeaderTemplateArrowUp"] as DataTemplate;
      if (this._lastHeaderClicked != null && this._lastHeaderClicked != originalSource)
        this._lastHeaderClicked.Column.HeaderTemplate = (DataTemplate) null;
      this._lastHeaderClicked = originalSource;
      this._lastDirection = direction;
    }

    private void Sort(ListView lv, string sortBy, ListSortDirection direction)
    {
      try
      {
        ICollectionView defaultView = CollectionViewSource.GetDefaultView((object) lv.ItemsSource);
        defaultView.SortDescriptions.Clear();
        SortDescription sortDescription = new SortDescription(sortBy, direction);
        defaultView.SortDescriptions.Add(sortDescription);
        defaultView.Refresh();
      }
      catch (Exception ex)
      {
      }
    }
  }
}
