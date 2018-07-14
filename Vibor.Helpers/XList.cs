﻿// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.XList
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Vibor.Helpers
{
  public class XList
  {
    public static bool IsValidIndex<T>(ICollection<T> a, int ii)
    {
      if (a == null)
        return false;
      return ii >= 0 && ii < a.Count;
    }

    public static bool IsNullOrEmpty<T>(ICollection<T> a)
    {
      return !XList.IsValidIndex<T>(a, 0);
    }

    public static string ToString(TimeSpan ts)
    {
      return new DateTime(ts.Ticks).ToString("HH:mm:ss");
    }

    public static void SortedListInterator<TKey, TValue>(SortedList<TKey, TValue> sortedList, Action<TKey, TValue> action)
    {
      foreach (KeyValuePair<TKey, TValue> sorted in sortedList)
        action(sorted.Key, sorted.Value);
    }

    public bool Contains(int[] idList, int searchId)
    {
      return ((IEnumerable<int>) idList).Any<int>((Func<int, bool>) (id => id == searchId));
    }
  }
}
