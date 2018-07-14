// Decompiled with JetBrains decompiler
// Type: Vibor.View.Helpers.Misc.ViewLib
// Assembly: Vibor.View.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 66A7410A-B003-4703-9434-CC2ABBB24103
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.View.Helpers.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Vibor.Helpers;

namespace Vibor.View.Helpers.Misc
{
  public class ViewLib
  {
    private static readonly ILog Log = XLogger.GetLogger();

    public static bool IsInDesignMode
    {
      get
      {
        return (bool) DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof (DependencyObject)).DefaultValue;
      }
    }

    public static int WaitCursorCount
    {
      get
      {
        if (!Application.Current.Properties.Contains((object) "CN"))
          Application.Current.Properties.Add((object) "CN", (object) 0);
        return (int) Application.Current.Properties[(object) "CN"];
      }
      set
      {
        if (!Application.Current.Properties.Contains((object) "CN"))
          Application.Current.Properties.Add((object) "CN", (object) value);
        if (value < 0)
          value = 0;
        Application.Current.Properties[(object) "CN"] = (object) value;
        Application.Current.MainWindow.Cursor = value > 0 ? Cursors.AppStarting : Cursors.Arrow;
      }
    }

    public static void CreateListViewColumns(GridView gridView, string[] bindings, string[] headers, double[] widths)
    {
      if (gridView == null)
        return;
      if (!XList.IsNullOrEmpty<GridViewColumn>((ICollection<GridViewColumn>) gridView.Columns))
        gridView.Columns.Clear();
      for (int index = 0; index < bindings.Length; ++index)
        ViewLib.AddColumnWithBinding(gridView, bindings[index], headers[index], widths[index]);
    }

    public static void AddColumnWithBinding(GridView gv, string path, string header, double width)
    {
      GridViewColumn gridViewColumn = new GridViewColumn() { DisplayMemberBinding = (BindingBase) new Binding(path), Header = (object) header, Width = width };
      gv.Columns.Add(gridViewColumn);
    }

    public static void BeginInvokeSimple(FrameworkElement frameworkElement, Action del, DispatcherPriority priority)
    {
      Action action = (Action) (() =>
      {
        try
        {
          ++ViewLib.WaitCursorCount;
          del();
        }
        catch (Exception ex)
        {
          ViewLib.Log.Error(ex);
        }
        finally
        {
          --ViewLib.WaitCursorCount;
        }
      });
      frameworkElement.Dispatcher.BeginInvoke(priority, (Delegate) action);
    }

    public static void BeginInvoke(FrameworkElement frameworkElement, Action del1, DispatcherPriority priority)
    {
      ((Action) (() => ViewLib.BeginInvokeSimple(frameworkElement, del1, priority))).BeginInvoke((AsyncCallback) null, (object) null);
    }

    public static void DisconnectFromParent(FrameworkElement player)
    {
      if (player.Parent == null)
        return;
      if (player.Parent is ContentControl)
      {
        ContentControl parent = player.Parent as ContentControl;
        if (parent == null || parent.Content == null)
          return;
        parent.Content = (object) null;
      }
      else if (player.Parent is Panel)
      {
        Panel parent = player.Parent as Panel;
        if (parent == null || !parent.Children.Contains((UIElement) player))
          return;
        parent.Children.Remove((UIElement) player);
      }
      else
      {
        if (!(player.Parent is Decorator))
          return;
        Decorator parent = player.Parent as Decorator;
        if (parent == null || parent.Child == null)
          return;
        parent.Child = (UIElement) null;
      }
    }

    public static void ShowPhoto(Image image, Uri uri)
    {
      image.Source = (ImageSource) null;
      BitmapImage bitmapImage = new BitmapImage();
      bitmapImage.BeginInit();
      bitmapImage.UriSource = uri;
      bitmapImage.EndInit();
      image.Stretch = Stretch.Uniform;
      image.Source = (ImageSource) bitmapImage;
    }

    public static void ShowPhoto(Image image, string path, UriKind uriKind)
    {
      image.Source = (ImageSource) null;
      Uri uri = new Uri(path, uriKind);
      ViewLib.ShowPhoto(image, uri);
    }

    public static void ShowPhoto(Image image, string path)
    {
      ViewLib.ShowPhoto(image, path, UriKind.Relative);
    }

    public static void Sort(IEnumerable itemsSource, string sortBy, ListSortDirection direction)
    {
      if (itemsSource == null)
        return;
      ICollectionView defaultView = CollectionViewSource.GetDefaultView((object) itemsSource);
      defaultView.SortDescriptions.Clear();
      SortDescription sortDescription = new SortDescription(sortBy, direction);
      defaultView.SortDescriptions.Add(sortDescription);
      defaultView.Refresh();
    }

    public static void ShowStatusMessageItemsCount(IList items)
    {
    }

    public static Action<Action> GetAddAndShowDelegate(ListView lv)
    {
      return (Action<Action>) (a => lv.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Delegate) (() =>
      {
        a();
        ViewLib.ScrollToEnd(lv);
      })));
    }

    public static void ScrollToEnd(ListView lv)
    {
      if (lv.Items.Count <= 0)
        return;
      lv.ScrollIntoView(lv.Items[lv.Items.Count - 1]);
    }

    public static Action<Action> GetAddDelegate(FrameworkElement lv, DispatcherPriority priority = DispatcherPriority.Background)
    {
      return (Action<Action>) (a => lv.Dispatcher.BeginInvoke(priority, (Delegate) a));
    }
  }
}
