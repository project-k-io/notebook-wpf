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
    }
}