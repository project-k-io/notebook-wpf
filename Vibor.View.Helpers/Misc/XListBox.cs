﻿using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Vibor.View.Helpers.Misc
{
    internal class XListBox
    {
        public static int GetCurrentIndex(ListBox listBox, Func<Point, Point> getPosition)
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

        public static ListBoxItem GetCurrentListBoxItem(ListBox listBox, Func<Point, Point> getPosition)
        {
            for (var index = 0; index < listBox.Items.Count; ++index)
            {
                var listBoxItem = GetListBoxItem(listBox, index);
                if (IsMouseOverTarget(listBoxItem, getPosition))
                    return listBoxItem;
            }

            return null;
        }

        private static ListBoxItem GetListBoxItem(ListBox listBox, int index)
        {
            if (listBox.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return null;
            return listBox.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
        }

        private static bool IsMouseOverTarget(IInputElement target, Func<Point, Point> getPosition)
        {
            return target == null ? false : false;
        }

        public static ObservableCollection<T> GetDraggableIems<T>(ListBox listBox, MouseEventArgs e) where T : class
        {
            return null;
        }

        public static void Drop<T>(ListBox listBox, object sender, DragEventArgs e)
        {
        }
    }
}