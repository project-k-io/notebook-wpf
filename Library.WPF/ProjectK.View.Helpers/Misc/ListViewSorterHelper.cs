using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Extensions.Logging;
using ProjectK.Extensions.Logging;

namespace ProjectK.Toolkit.Wpf.Helpers.Misc;

public class ListViewSorterHelper
{
    private static readonly ILogger Logger = LogManager.GetLogger<ListViewSorterHelper>();
    private GridViewColumnHeader _lastColumn; // last column clicked

    private ListSortDirection _lastDirection = ListSortDirection.Ascending;

    public void Clicked(FrameworkElement parent, object sender, RoutedEventArgs e)
    {
        if (!(sender is ListView listView))
            return;

        if (!(e.OriginalSource is GridViewColumnHeader column) || column.Role == GridViewColumnHeaderRole.Padding)
            return;

        var direction = column == _lastColumn
            ? _lastDirection != ListSortDirection.Ascending ? ListSortDirection.Ascending :
            ListSortDirection.Descending
            : ListSortDirection.Ascending;

        var header = column.Column.Header as string;
        Sort(listView, header, direction);

        column.Column.HeaderTemplate = direction != ListSortDirection.Ascending
            ? parent.Resources["HeaderTemplateArrowDown"] as DataTemplate
            : parent.Resources["HeaderTemplateArrowUp"] as DataTemplate;

        if (_lastColumn != null && _lastColumn != column)
            _lastColumn.Column.HeaderTemplate = null;

        _lastColumn = column;
        _lastDirection = direction;
    }

    private static void Sort(ItemsControl control, string sortBy, ListSortDirection direction)
    {
        try
        {
            var view = CollectionViewSource.GetDefaultView(control.ItemsSource);
            view.SortDescriptions.Clear();
            var sortDescription = new SortDescription(sortBy, direction);
            view.SortDescriptions.Add(sortDescription);
            view.DeferRefresh();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }
}