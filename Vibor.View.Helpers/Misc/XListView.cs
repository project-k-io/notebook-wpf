using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Win32;

namespace ProjectK.View.Helpers.Misc
{
    public class XListView
    {
        public static void SortAndUpdateTemplate(ListView listView, GridViewColumn gridViewColumn, string sortBy,
            ListSortDirection direction, ref GridViewColumn lastColumnClicked, ref ListSortDirection lastDirection)
        {
            ViewLib.Sort(listView.ItemsSource, sortBy, direction);
            gridViewColumn.HeaderTemplate = direction != ListSortDirection.Ascending
                ? listView.FindResource("HeaderTemplateArrowDown") as DataTemplate
                : listView.FindResource("HeaderTemplateArrowUp") as DataTemplate;
            if (lastColumnClicked != null && lastColumnClicked != gridViewColumn)
                lastColumnClicked.HeaderTemplate = null;
            lastColumnClicked = gridViewColumn;
            lastDirection = direction;
        }

        public static void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e,
            ref GridViewColumn lastColumnClicked, ref ListSortDirection lastDirection)
        {
            var listView = sender as ListView;
            if (!(e.OriginalSource is GridViewColumnHeader originalSource) || originalSource.Role == GridViewColumnHeaderRole.Padding)
                return;
            var direction = originalSource.Column == lastColumnClicked
                ? lastDirection == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending
                : ListSortDirection.Ascending;
            if (!(originalSource.Column is SortableGridViewColumn column))
                return;
            var fieldName = column.FieldName;
            if (string.IsNullOrEmpty(fieldName))
                fieldName = column.Header.ToString();
            if (string.IsNullOrEmpty(fieldName))
                return;
            SortAndUpdateTemplate(listView, originalSource.Column, fieldName, direction, ref lastColumnClicked,
                ref lastDirection);
        }

        public static void ListViewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is ListView listView))
                return;
            if (!(listView.View is GridView view))
                return;
            var contextMenu = new ContextMenu();
            foreach (var column in view.Columns)
            {
                var gridViewColumnPair = new ListViewGridViewColumnPair {ListView = listView, Column = column};
                var checkBox1 = new CheckBox
                {
                    Content = column.Header,
                    Tag = gridViewColumnPair,
                    IsChecked = column.Width != 0.0
                };
                var checkBox2 = checkBox1;
                contextMenu.Items.Add(checkBox2);
                checkBox2.Checked += CheckBoxChecked;
                checkBox2.Unchecked += CheckBoxChecked;
            }

            listView.ContextMenu = contextMenu;
        }

        public static void ReadColumnSettings(ListView lv)
        {
            if (lv == null)
                return;
            var name = lv.Name;
            if (string.IsNullOrEmpty(name))
                return;
            var subKey = Registry.CurrentUser.CreateSubKey(name);
            if (subKey == null)
                return;
            if (!(lv.View is GridView view))
                return;
            var num = 0.0;
            foreach (var column in view.Columns)
            {
                var header = column.Header as string;
                var s = subKey.GetValue(header, 48) as string;
                if (int.TryParse(s, out var result))
                    column.Width = result;
                num += column.Width;
            }

            lv.Width = num + 48.0;
        }

        public static void SaveColumnSettings(ListView lv)
        {
            var name = lv.Name;
            if (string.IsNullOrEmpty(name))
                return;
            var subKey = Registry.CurrentUser.CreateSubKey(name);
            if (subKey == null)
                return;
            if (!(lv.View is GridView view))
                return;
            foreach (var column in view.Columns)
            {
                var header = column.Header as string;
                subKey.SetValue(header, column.Width);
            }
        }

        private static void CheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox checkBox))
                return;
            if (!(checkBox.Tag is GridViewColumn tag))
                return;
            tag.Width = 0.0;
        }

        private static void CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox checkBox1))
                return;
            if (!(checkBox1.Tag is ListViewGridViewColumnPair tag))
                return;
            var listView = tag.ListView;
            if (listView == null)
                return;
            var contextMenu = listView.ContextMenu;
            if (contextMenu == null)
                return;
            var num1 = 0;
            foreach (var obj in contextMenu.Items)
            {
                if (!(obj is CheckBox checkBox2))
                    return;
                if (checkBox2.IsChecked.GetValueOrDefault())
                    ++num1;
            }

            if (num1 == 0)
                return;
            var column1 = tag.Column;
            if (column1 == null)
                return;
            column1.Width = column1.Width != 0.0 ? 0.0 : double.NaN;
            if (!(listView.View is GridView view))
                return;
            var num2 = 0.0;
            foreach (var column2 in view.Columns)
                if (double.IsNaN(column2.Width))
                {
                    if (column2.ActualWidth == 0.0)
                    {
                        column2.Width = 48.0;
                        num2 += column2.Width;
                    }
                    else
                    {
                        num2 += column2.ActualWidth;
                    }
                }
                else
                {
                    num2 += column2.Width;
                }

            listView.Width = num2 + 48.0;
            SaveColumnSettings(listView);
        }

        public static void Refresh(ListView listView)
        {
            CollectionViewSource.GetDefaultView(listView.ItemsSource).Refresh();
        }

        public static void CopyCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            var originalSource = e.OriginalSource as ListView;
            var stringBuilder = new StringBuilder();
            foreach (var selectedItem in originalSource.SelectedItems)
                stringBuilder.AppendLine(selectedItem.ToString());
            Clipboard.SetText(stringBuilder.ToString());
        }

        public static void CopyCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((e.OriginalSource as ListView).SelectedItems.Count > 0)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }
    }
}