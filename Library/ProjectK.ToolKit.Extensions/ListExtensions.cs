using System;
using System.Collections.Generic;

namespace ProjectK.ToolKit.Utils;

public static class ListExtensions
{
    public static bool IsValidIndex<T>(this ICollection<T> a, int ii)
    {
        if (a == null)
            return false;

        return ii >= 0 && ii < a.Count;
    }

    public static bool IsNullOrEmpty<T>(this ICollection<T> a)
    {
        return !a.IsValidIndex(0);
    }

    public static void AddRange<T>(this ICollection<T> target, ICollection<T> source)
    {
        foreach (var item in source) target.Add(item);
    }


    public static bool IsSame<T>(this IList<T> listA, IList<T> listB, Func<T, T, bool> isSame)
    {
        if (listA.Count != listB.Count)
            return false;

        for (var i = 0; i < listA.Count; i++)
        {
            var a = listB[i];
            var b = listA[i];

            if (!isSame(a, b))
                return false;
        }

        return true;
    }

    public static void Copy<T>(this IList<T> listA, IList<T> listB, Func<T, T> copy)
    {
        foreach (var nodeB in listB)
            listA.Add(copy(nodeB));
    }
}