using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ProjectK.View.Helpers.Misc
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
                ? _lastDirection != ListSortDirection.Ascending
                    ? ListSortDirection.Ascending
                    : ListSortDirection.Descending
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