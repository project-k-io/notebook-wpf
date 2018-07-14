// Decompiled with JetBrains decompiler
// Type: Vibor.Generic.ViewModels.RoutedEventHelper
// Assembly: Vibor.Generic.ViewModels, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 18134161-73B0-45D8-9612-67C25563536B
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Generic.ViewModels.dll

using System;
using System.Windows;

namespace Vibor.Generic.ViewModels
{
  public static class RoutedEventHelper
  {
    public static void RaiseEvent(DependencyObject target, RoutedEventArgs args)
    {
      if (target is UIElement)
      {
        (target as UIElement).RaiseEvent(args);
      }
      else
      {
        if (!(target is ContentElement))
          return;
        (target as ContentElement).RaiseEvent(args);
      }
    }

    internal static void AddHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler)
    {
      UIElement uiElement = element as UIElement;
      if (uiElement != null)
        uiElement.AddHandler(routedEvent, handler);
      else
        (element as ContentElement)?.AddHandler(routedEvent, handler);
    }

    internal static void RemoveHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler)
    {
      UIElement uiElement = element as UIElement;
      if (uiElement != null)
        uiElement.RemoveHandler(routedEvent, handler);
      else
        (element as ContentElement)?.RemoveHandler(routedEvent, handler);
    }
  }
}
