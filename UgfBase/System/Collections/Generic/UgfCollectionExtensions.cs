#region

using System.Linq;
using Secyud.Ugf;

#endregion

namespace System.Collections.Generic
{
    public static class UgfCollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return source == null || source.Count == 0;
        }

        public static bool AddIfNotContains<T>(this ICollection<T> source, T item)
        {
            Thrower.IfNull(source);
            if (source.Contains(item))
                return false;

            source.Add(item);
            return true;
        }

        public static IEnumerable<T> AddIfNotContains<T>(this ICollection<T> source, IEnumerable<T> items)
        {
            Thrower.IfNull(source);
            var addedItems = new List<T>();
            foreach (var item in items)
                if (!source.Contains(item))
                {
                    source.Add(item);
                    addedItems.Add(item);
                }

            return addedItems;
        }

        public static bool AddIfNotContains<T>(this ICollection<T> source, Func<T, bool> predicate, Func<T> itemFactory)
        {
            Thrower.IfNull(source);
            Thrower.IfNull(predicate);
            Thrower.IfNull(itemFactory);

            if (source.Any(predicate))
                return false;

            source.Add(itemFactory());
            return true;
        }

        public static IList<T> RemoveAll<T>(this ICollection<T> source, Func<T, bool> predicate)
        {
            var items = source.Where(predicate).ToList();

            foreach (var item in items)
                source.Remove(item);

            return items;
        }

        public static void RemoveAll<T>(this ICollection<T> source, IEnumerable<T> items)
        {
            foreach (var item in items)
                source.Remove(item);
        }
        
        public static T Pick<T>(this T[] source)
        {
            return source[Og.GetRandom(source.Length)];
        }
    }
}