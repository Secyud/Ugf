using System.Collections.Generic;

namespace System.Ugf.Collections.Generic
{
    public static class UgfCollectionExtension
    {
        public static void AddIfNotContains<T>(this IList<T> list, T value)
        {
            if (!list.Contains(value))
            {
                list.Add(value);
            }
        }

        public static bool IsNullOrEmpty<T>(this IList<T> source)
        {
            return source is null || source.Count == 0;
        }

        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return source is null || source.Count == 0;
        }
    }
}