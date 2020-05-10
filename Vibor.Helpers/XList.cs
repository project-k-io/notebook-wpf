using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectK.Utils
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
            return !IsValidIndex(a, 0);
        }

        public static string ToString(TimeSpan ts)
        {
            return new DateTime(ts.Ticks).ToString("HH:mm:ss");
        }

        public static void SortedListIterator<TKey, TValue>(SortedList<TKey, TValue> sortedList,
            Action<TKey, TValue> action)
        {
            foreach (var sorted in sortedList)
                action(sorted.Key, sorted.Value);
        }

        public bool Contains(int[] idList, int searchId)
        {
            return idList.Any(id => id == searchId);
        }
    }
}