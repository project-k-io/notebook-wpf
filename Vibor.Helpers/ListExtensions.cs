using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectK.Utils
{
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
            return !IsValidIndex(a, 0);
        }

        public static string ToString(this TimeSpan ts)
        {
            return new DateTime(ts.Ticks).ToString("HH:mm:ss");
        }

        public static void SortedListIterator<TKey, TValue>(this SortedList<TKey, TValue> sortedList,
            Action<TKey, TValue> action)
        {
            foreach (var sorted in sortedList)
                action(sorted.Key, sorted.Value);
        }

        public static bool Contains(this int[] idList, int searchId)
        {
            return idList.Any(id => id == searchId);
        }
    }
}