﻿using System.Collections.Generic;

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

        public static void AddToList<T>(this ICollection<T> list, T task) where T : ITask<T>
        {
            list.Add(task);
            foreach (var subTask in task.SubTasks)
                AddToList(list, subTask);
        }

    }
}