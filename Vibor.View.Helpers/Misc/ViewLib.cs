using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Vibor.Helpers;
using Microsoft.Extensions.Logging;

namespace Vibor.View.Helpers.Misc
{
    public class ViewLib
    {
        private static readonly ILogger Log = LogManager.GetLogger<ViewLib>();

        public static bool IsInDesignMode => (bool) DesignerProperties.IsInDesignModeProperty
            .GetMetadata(typeof(DependencyObject)).DefaultValue;

        public static int WaitCursorCount
        {
            get
            {
                if (!Application.Current.Properties.Contains("CN")) Application.Current.Properties.Add("CN", 0);

                return (int) Application.Current.Properties["CN"];
            }
            set
            {
                if (!Application.Current.Properties.Contains("CN")) Application.Current.Properties.Add("CN", value);

                if (value < 0) value = 0;

                Application.Current.Properties["CN"] = value;
                Application.Current.MainWindow.Cursor = value > 0 ? Cursors.AppStarting : Cursors.Arrow;
            }
        }

        public static void CreateListViewColumns(GridView gridView, string[] bindings, string[] headers,
            double[] widths)
        {
            if (gridView == null) return;

            if (!XList.IsNullOrEmpty(gridView.Columns)) gridView.Columns.Clear();

            for (var index = 0; index < bindings.Length; ++index)
                AddColumnWithBinding(gridView, bindings[index], headers[index], widths[index]);
        }

        public static void AddColumnWithBinding(GridView gv, string path, string header, double width)
        {
            var gridViewColumn =
                new GridViewColumn {DisplayMemberBinding = new Binding(path), Header = header, Width = width};
            gv.Columns.Add(gridViewColumn);
        }

        public static void BeginInvokeSimple(FrameworkElement frameworkElement, Action del, DispatcherPriority priority)
        {
            var action = (Action) (() =>
            {
                try
                {
                    ++WaitCursorCount;
                    del();
                }
                catch (Exception ex)
                {
                    Log.LogError(ex);
                }
                finally
                {
                    --WaitCursorCount;
                }
            });
            frameworkElement.Dispatcher.BeginInvoke(priority, action);
        }

        public static void BeginInvoke(FrameworkElement frameworkElement, Action del1, DispatcherPriority priority)
        {
            ((Action) (() => BeginInvokeSimple(frameworkElement, del1, priority))).BeginInvoke(null, null);
        }

        public static void DisconnectFromParent(FrameworkElement player)
        {
            if (player.Parent == null) return;

            if (player.Parent is ContentControl)
            {
                var parent = player.Parent as ContentControl;
                if (parent == null || parent.Content == null) return;

                parent.Content = null;
            }
            else if (player.Parent is Panel)
            {
                var parent = player.Parent as Panel;
                if (parent == null || !parent.Children.Contains(player)) return;

                parent.Children.Remove(player);
            }
            else
            {
                if (!(player.Parent is Decorator)) return;

                var parent = player.Parent as Decorator;
                if (parent == null || parent.Child == null) return;

                parent.Child = null;
            }
        }

        public static void ShowPhoto(Image image, Uri uri)
        {
            image.Source = null;
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = uri;
            bitmapImage.EndInit();
            image.Stretch = Stretch.Uniform;
            image.Source = bitmapImage;
        }

        public static void ShowPhoto(Image image, string path, UriKind uriKind)
        {
            image.Source = null;
            var uri = new Uri(path, uriKind);
            ShowPhoto(image, uri);
        }

        public static void ShowPhoto(Image image, string path)
        {
            ShowPhoto(image, path, UriKind.Relative);
        }

        public static void Sort(IEnumerable itemsSource, string sortBy, ListSortDirection direction)
        {
            if (itemsSource == null) return;

            var defaultView = CollectionViewSource.GetDefaultView(itemsSource);
            defaultView.SortDescriptions.Clear();
            var sortDescription = new SortDescription(sortBy, direction);
            defaultView.SortDescriptions.Add(sortDescription);
            defaultView.Refresh();
        }

        public static void ShowStatusMessageItemsCount(IList items)
        {
        }


        public static Action<Action> GetAddAndShowDelegate(ListView lv)
        {
            return a => lv.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Action) (() =>
            {
                a();
                ScrollToEnd(lv);
            }));
        }

        public static void ScrollToEnd(ListView lv)
        {
            if (lv.Items.Count <= 0) return;

            lv.ScrollIntoView(lv.Items[lv.Items.Count - 1]);
        }

        public static Action<Action> GetAddDelegate(FrameworkElement lv,
            DispatcherPriority priority = DispatcherPriority.Background)
        {
            return a => lv.Dispatcher.BeginInvoke(priority, a);
        }
    }
}