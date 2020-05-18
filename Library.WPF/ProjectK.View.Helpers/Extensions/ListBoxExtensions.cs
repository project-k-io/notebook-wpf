using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ProjectK.View.Helpers.Extensions
{
    public static class ListBoxExtensions
    {
        public static int GetCurrentIndex(this ListBox listBox, Func<Point, Point> getPosition)
        {
            var num = -1;
            for (var index = 0; index < listBox.Items.Count; ++index)
                if (IsMouseOverTarget(GetListBoxItem(listBox, index), getPosition))
                {
                    num = index;
                    break;
                }

            return num;
        }

        public static ListBoxItem GetCurrentListBoxItem(this ListBox listBox, Func<Point, Point> getPosition)
        {
            for (var index = 0; index < listBox.Items.Count; ++index)
            {
                var listBoxItem = GetListBoxItem(listBox, index);
                if (IsMouseOverTarget(listBoxItem, getPosition))
                    return listBoxItem;
            }

            return null;
        }

        private static ListBoxItem GetListBoxItem(this ListBox listBox, int index)
        {
            if (listBox.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return null;
            return listBox.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
        }

        private static bool IsMouseOverTarget(IInputElement target, Func<Point, Point> getPosition)
        {
            return target != null;
        }

        public static ObservableCollection<T> GetDraggableItems<T>(this ListBox listBox, MouseEventArgs e) where T : class
        {
            return null;
        }
    }
}