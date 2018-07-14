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
            var lv = sender as ListView;
            if (lv == null)
                return;
            var originalSource = e.OriginalSource as GridViewColumnHeader;
            if (originalSource == null || originalSource.Role == GridViewColumnHeaderRole.Padding)
                return;
            var direction = originalSource == _lastHeaderClicked
                ? (_lastDirection != ListSortDirection.Ascending
                    ? ListSortDirection.Ascending
                    : ListSortDirection.Descending)
                : ListSortDirection.Ascending;
            var header = originalSource.Column.Header as string;
            Sort(lv, header, direction);
            originalSource.Column.HeaderTemplate = direction != ListSortDirection.Ascending
                ? parent.Resources["HeaderTemplateArrowDown"] as DataTemplate
                : parent.Resources["HeaderTemplateArrowUp"] as DataTemplate;
            if (_lastHeaderClicked != null && _lastHeaderClicked != originalSource)
                _lastHeaderClicked.Column.HeaderTemplate = null;
            _lastHeaderClicked = originalSource;
            _lastDirection = direction;
        }

        private void Sort(ListView lv, string sortBy, ListSortDirection direction)
        {
            try
            {
                var defaultView = CollectionViewSource.GetDefaultView(lv.ItemsSource);
                defaultView.SortDescriptions.Clear();
                var sortDescription = new SortDescription(sortBy, direction);
                defaultView.SortDescriptions.Add(sortDescription);
                defaultView.Refresh();
            }
            catch (Exception ex)
            {
            }
        }
    }
}