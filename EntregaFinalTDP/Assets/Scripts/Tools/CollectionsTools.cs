using System;
using System.Collections.Generic;

namespace Tools
{
    public static class CollectionsTools
    {
        public static void ResetWithCapacity<T>(this List<T> list, int desiredCapacity)
        {
            list.Capacity = Math.Max(desiredCapacity, list.Capacity);
            list.Clear();
        }
    }
}