// Decompiled with JetBrains decompiler
// Type: Vibor.View.Helpers.Misc.XListView
// Assembly: Vibor.View.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 66A7410A-B003-4703-9434-CC2ABBB24103
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.View.Helpers.dll

using Microsoft.Win32;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Vibor.View.Helpers.Misc
{
  public class XListView
  {
    public static void SortAndUpdateTemplate(ListView listView, GridViewColumn gridViewColumn, string sortBy, ListSortDirection direction, ref GridViewColumn lastColumnClicked, ref ListSortDirection lastDirection)
    {
      ViewLib.Sort(listView.ItemsSource, sortBy, direction);
      gridViewColumn.HeaderTemplate = direction != ListSortDirection.Ascending ? listView.FindResource((object) "HeaderTemplateArrowDown") as DataTemplate : listView.FindResource((object) "HeaderTemplateArrowUp") as DataTemplate;
      if (lastColumnClicked != null && lastColumnClicked != gridViewColumn)
        lastColumnClicked.HeaderTemplate = (DataTemplate) null;
      lastColumnClicked = gridViewColumn;
      lastDirection = direction;
    }

    public static void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e, ref GridViewColumn lastColumnClicked, ref ListSortDirection lastDirection)
    {
      ListView listView = sender as ListView;
      GridViewColumnHeader originalSource = e.OriginalSource as GridViewColumnHeader;
      if (originalSource == null || originalSource.Role == GridViewColumnHeaderRole.Padding)
        return;
      ListSortDirection direction = originalSource.Column == lastColumnClicked ? (lastDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending) : ListSortDirection.Ascending;
      SortableGridViewColumn column = originalSource.Column as SortableGridViewColumn;
      if (column == null)
        return;
      string fieldName = column.FieldName;
      if (string.IsNullOrEmpty(fieldName))
        fieldName = column.Header.ToString();
      if (string.IsNullOrEmpty(fieldName))
        return;
      XListView.SortAndUpdateTemplate(listView, originalSource.Column, fieldName, direction, ref lastColumnClicked, ref lastDirection);
    }

    public static void ListViewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      ListView listView = sender as ListView;
      if (listView == null)
        return;
      GridView view = listView.View as GridView;
      if (view == null)
        return;
      ContextMenu contextMenu = new ContextMenu();
      foreach (GridViewColumn column in (Collection<GridViewColumn>) view.Columns)
      {
        ListViewGridViewColumnPair gridViewColumnPair = new ListViewGridViewColumnPair() { ListView = listView, Column = column };
        CheckBox checkBox1 = new CheckBox();
        checkBox1.Content = column.Header;
        checkBox1.Tag = (object) gridViewColumnPair;
        checkBox1.IsChecked = new bool?(column.Width != 0.0);
        CheckBox checkBox2 = checkBox1;
        contextMenu.Items.Add((object) checkBox2);
        checkBox2.Checked += new RoutedEventHandler(XListView.CheckBoxChecked);
        checkBox2.Unchecked += new RoutedEventHandler(XListView.CheckBoxChecked);
      }
      listView.ContextMenu = contextMenu;
    }

    public static void ReadColumnSettings(ListView lv)
    {
      if (lv == null)
        return;
      string name = lv.Name;
      if (string.IsNullOrEmpty(name))
        return;
      RegistryKey subKey = Registry.CurrentUser.CreateSubKey(name);
      if (subKey == null)
        return;
      GridView view = lv.View as GridView;
      if (view == null)
        return;
      double num = 0.0;
      foreach (GridViewColumn column in (Collection<GridViewColumn>) view.Columns)
      {
        string header = column.Header as string;
        string s = subKey.GetValue(header, (object) 48) as string;
        int result = 0;
        if (int.TryParse(s, out result))
          column.Width = (double) result;
        num += column.Width;
      }
      lv.Width = num + 48.0;
    }

    public static void SaveColumnSettings(ListView lv)
    {
      string name = lv.Name;
      if (string.IsNullOrEmpty(name))
        return;
      RegistryKey subKey = Registry.CurrentUser.CreateSubKey(name);
      if (subKey == null)
        return;
      GridView view = lv.View as GridView;
      if (view == null)
        return;
      foreach (GridViewColumn column in (Collection<GridViewColumn>) view.Columns)
      {
        string header = column.Header as string;
        subKey.SetValue(header, (object) column.Width);
      }
    }

    private static void CheckBoxUnchecked(object sender, RoutedEventArgs e)
    {
      CheckBox checkBox = sender as CheckBox;
      if (checkBox == null)
        return;
      GridViewColumn tag = checkBox.Tag as GridViewColumn;
      if (tag == null)
        return;
      tag.Width = 0.0;
    }

    private static void CheckBoxChecked(object sender, RoutedEventArgs e)
    {
      CheckBox checkBox1 = sender as CheckBox;
      if (checkBox1 == null)
        return;
      ListViewGridViewColumnPair tag = checkBox1.Tag as ListViewGridViewColumnPair;
      if (tag == null)
        return;
      ListView listView = tag.ListView;
      if (listView == null)
        return;
      ContextMenu contextMenu = listView.ContextMenu;
      if (contextMenu == null)
        return;
      int num1 = 0;
      foreach (object obj in (IEnumerable) contextMenu.Items)
      {
        CheckBox checkBox2 = obj as CheckBox;
        if (checkBox2 == null)
          return;
        if (checkBox2.IsChecked.GetValueOrDefault())
          ++num1;
      }
      if (num1 == 0)
        return;
      GridViewColumn column1 = tag.Column;
      if (column1 == null)
        return;
      column1.Width = column1.Width != 0.0 ? 0.0 : double.NaN;
      GridView view = listView.View as GridView;
      if (view == null)
        return;
      double num2 = 0.0;
      foreach (GridViewColumn column2 in (Collection<GridViewColumn>) view.Columns)
      {
        if (double.IsNaN(column2.Width))
        {
          if (column2.ActualWidth == 0.0)
          {
            column2.Width = 48.0;
            num2 += column2.Width;
          }
          else
            num2 += column2.ActualWidth;
        }
        else
          num2 += column2.Width;
      }
      listView.Width = num2 + 48.0;
      XListView.SaveColumnSettings(listView);
    }

    private void RefreshSort()
    {
    }

    public static void Refresh(ListView listView)
    {
      CollectionViewSource.GetDefaultView((object) listView.ItemsSource).Refresh();
    }

    public static void CopyCmdExecuted(object target, ExecutedRoutedEventArgs e)
    {
      ListView originalSource = e.OriginalSource as ListView;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (object selectedItem in (IEnumerable) originalSource.SelectedItems)
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
