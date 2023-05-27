#region

using Secyud.Ugf;
using System.Linq;

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
			List<T> addedItems = new List<T>();
			foreach (T item in items)
				if (!source.Contains(item))
				{
					source.Add(item);
					addedItems.Add(item);
				}

			return addedItems;
		}

		public static bool AddIfNotContains<T>(this ICollection<T> source, Func<T, bool> predicate,
			Func<T> itemFactory)
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
			List<T> items = source.Where(predicate).ToList();

			foreach (T item in items)
				source.Remove(item);

			return items;
		}

		public static void RemoveAll<T>(this ICollection<T> source, IEnumerable<T> items)
		{
			foreach (T item in items)
				source.Remove(item);
		}
	}
}