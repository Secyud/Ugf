#region

using System.Linq;

#endregion

namespace System.Collections.Generic
{
	public static class UgfEnumerableExtensions
	{
		public static string JoinAsString(this IEnumerable<string> source, string separator)
		{
			return string.Join(separator, source);
		}

		public static string JoinAsString<T>(this IEnumerable<T> source, string separator)
		{
			return string.Join(separator, source);
		}

		public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition,
			Func<T, bool> predicate)
		{
			return condition
				? source.Where(predicate)
				: source;
		}

		public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition,
			Func<T, int, bool> predicate)
		{
			return condition
				? source.Where(predicate)
				: source;
		}

		public static List<TTarget> TryFindCast<TSource, TTarget>(this IEnumerable<TSource> source)
		{
			return source.Where(u => u is TTarget).Cast<TTarget>().ToList();
		}
	}
}