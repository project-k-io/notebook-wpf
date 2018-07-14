// Decompiled with JetBrains decompiler
// Type: Vibor.View.Helpers.Misc.XListBox
// Assembly: Vibor.View.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 66A7410A-B003-4703-9434-CC2ABBB24103
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.View.Helpers.dll

using System;
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
      int num = -1;
      for (int index = 0; index < listBox.Items.Count; ++index)
      {
        if (XListBox.IsMouseOverTarget((IInputElement) XListBox.GetListBoxItem(listBox, index), getPosition))
        {
          num = index;
          break;
        }
      }
      return num;
    }

    public static ListBoxItem GetCurrentListBoxItem(ListBox listBox, Func<Point, Point> getPosition)
    {
      for (int index = 0; index < listBox.Items.Count; ++index)
      {
        ListBoxItem listBoxItem = XListBox.GetListBoxItem(listBox, index);
        if (XListBox.IsMouseOverTarget((IInputElement) listBoxItem, getPosition))
          return listBoxItem;
      }
      return (ListBoxItem) null;
    }

    private static ListBoxItem GetListBoxItem(ListBox listBox, int index)
    {
      if (listBox.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
        return (ListBoxItem) null;
      return listBox.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
    }

    private static bool IsMouseOverTarget(IInputElement target, Func<Point, Point> getPosition)
    {
      return target == null ? false : false;
    }

    public static ObservableCollection<T> GetDraggableIems<T>(ListBox listBox, MouseEventArgs e) where T : class
    {
      return (ObservableCollection<T>) null;
    }

    public static void Drop<T>(ListBox listBox, object sender, DragEventArgs e)
    {
    }
  }
}
