#region

using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf;

#endregion

namespace System.Ugf.Collections.Generic
{
    public static class UgfListExtensions
    {
        public static T Pick<T>(this IReadOnlyList<T> source, float choice)
        {
            return source is null || source.Count == 0
                ? default
                : source[(int)choice];
        }

        public static T RandomPick<T>(this IList<T> source)
        {
            return source.IsNullOrEmpty()
                ? default
                : source[U.GetRandom(source.Count)];
        }


        public static T RandomPick<T>(this T[] source)
        {
            return source.IsNullOrEmpty()
                ? default
                : source[U.GetRandom(source.Length)];
        }

        public static void InsertRange<T>(this IList<T> source, int index, IEnumerable<T> items)
        {
            foreach (T item in items)
                source.Insert(index++, item);
        }

        public static int FindIndex<T>(this IList<T> source, Predicate<T> selector)
        {
            for (int i = 0; i < source.Count; ++i)
                if (selector(source[i]))
                    return i;

            return -1;
        }

        public static void AddFirst<T>(this IList<T> source, T item)
        {
            source.Insert(0, item);
        }

        public static void AddLast<T>(this IList<T> source, T item)
        {
            source.Insert(source.Count, item);
        }

        public static void InsertAfter<T>(this IList<T> source, T existingItem, T item)
        {
            int index = source.IndexOf(existingItem);
            if (index < 0)
            {
                source.AddFirst(item);
                return;
            }

            source.Insert(index + 1, item);
        }

        public static void InsertAfter<T>(this IList<T> source, Predicate<T> selector, T item)
        {
            int index = source.FindIndex(selector);
            if (index < 0)
            {
                source.AddFirst(item);
                return;
            }

            source.Insert(index + 1, item);
        }

        public static void InsertBefore<T>(this IList<T> source, T existingItem, T item)
        {
            int index = source.IndexOf(existingItem);
            if (index < 0)
            {
                source.AddLast(item);
                return;
            }

            source.Insert(index, item);
        }

        public static void InsertBefore<T>(this IList<T> source, Predicate<T> selector, T item)
        {
            int index = source.FindIndex(selector);
            if (index < 0)
            {
                source.AddLast(item);
                return;
            }

            source.Insert(index, item);
        }

        public static void ReplaceWhile<T>(this IList<T> source, Predicate<T> selector, T item)
        {
            for (int i = 0; i < source.Count; i++)
                if (selector(source[i]))
                    source[i] = item;
        }

        public static void ReplaceWhile<T>(this IList<T> source, Predicate<T> selector, Func<T, T> itemFactory)
        {
            for (int i = 0; i < source.Count; i++)
            {
                T item = source[i];
                if (selector(item))
                    source[i] = itemFactory(item);
            }
        }

        public static void ReplaceOne<T>(this IList<T> source, Predicate<T> selector, T item)
        {
            for (int i = 0; i < source.Count; i++)
                if (selector(source[i]))
                {
                    source[i] = item;
                    return;
                }
        }

        public static void ReplaceOne<T>(this IList<T> source, Predicate<T> selector, Func<T, T> itemFactory)
        {
            for (int i = 0; i < source.Count; i++)
            {
                T item = source[i];
                if (selector(item))
                {
                    source[i] = itemFactory(item);
                    return;
                }
            }
        }

        public static void ReplaceOne<T>(this IList<T> source, T item, T replaceWith)
        {
            for (int i = 0; i < source.Count; i++)
                if (Comparer<T>.Default.Compare(source[i], item) == 0)
                {
                    source[i] = replaceWith;
                    return;
                }
        }

        public static void MoveItem<T>(this List<T> source, Predicate<T> selector, int targetIndex)
        {
            if (!targetIndex.IsInRange(0, source.Count))
                throw new IndexOutOfRangeException("targetIndex should be between 0 and " + (source.Count - 1));

            int currentIndex = source.FindIndex(0, selector);
            if (currentIndex == targetIndex) return;

            T item = source[currentIndex];
            source.RemoveAt(currentIndex);
            source.Insert(targetIndex, item);
        }

        public static T GetOrAdd<T>(this IList<T> source, Func<T, bool> selector, Func<T> factory)
        {
            Thrower.IfNull(source);

            T item = source.FirstOrDefault(selector);

            if (item == null)
            {
                item = factory();
                source.Add(item);
            }

            return item;
        }

        /// <summary>
        ///     Sort a list by a topological sorting, which consider their dependencies.
        /// </summary>
        /// <typeparam name="T">The type of the members of values.</typeparam>
        /// <param name="source">A list of objects to sort</param>
        /// <param name="getDependencies">Function to resolve the dependencies</param>
        /// <param name="comparer">Equality comparer for dependencies </param>
        /// <returns>
        ///     Returns a new list ordered by dependencies.
        ///     If A depends on B, then B will come before than A in the resulting list.
        /// </returns>
        public static List<T> SortByDependencies<T>(
            this IEnumerable<T> source,
            Func<T, IEnumerable<T>> getDependencies,
            IEqualityComparer<T> comparer = null)
        {
            // See: http://www.codeproject.com/Articles/869059/Topological-sorting-in-Csharp
            //      http://en.wikipedia.org/wiki/Topological_sorting

            List<T> sorted = new List<T>();
            Dictionary<T, bool> visited = new Dictionary<T, bool>(comparer);

            foreach (T item in source)
                SortByDependenciesVisit(item, getDependencies, sorted, visited);

            return sorted;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T">The type of the members of values.</typeparam>
        /// <param name="item">Item to resolve</param>
        /// <param name="getDependencies">Function to resolve the dependencies</param>
        /// <param name="sorted">List with the sorted items</param>
        /// <param name="visited">Dictionary with the visited items</param>
        private static void SortByDependenciesVisit<T>(
            T item,
            Func<T, IEnumerable<T>> getDependencies,
            ICollection<T> sorted,
            IDictionary<T, bool> visited)
        {
            bool alreadyVisited = visited.TryGetValue(item, out bool inProcess);

            if (alreadyVisited)
            {
                if (inProcess)
                    throw new ArgumentException("Cyclic dependency found! Item: " + item);
            }
            else
            {
                visited[item] = true;

                IEnumerable<T> dependencies = getDependencies(item);
                if (dependencies != null)
                    foreach (T dependency in dependencies)
                        SortByDependenciesVisit(dependency, getDependencies, sorted, visited);

                visited[item] = false;
                sorted.Add(item);
            }
        }
    }
}