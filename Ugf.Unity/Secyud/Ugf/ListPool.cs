#region

using System.Collections.Generic;

#endregion

namespace Secyud.Ugf
{
	/// <summary>
	///     Generic static pool for lists.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static class ListPool<T>
	{
		private static readonly Stack<List<T>> Stack = new();

		/// <summary>
		///     Get a pooled list.
		/// </summary>
		/// <returns>The requested list.</returns>
		public static List<T> Get()
		{
			if (Stack.Count > 0)
			{
				return Stack.Pop();
			}

			return new List<T>();
		}

		/// <summary>
		///     Add a list back to the pool so it can be reused.
		/// </summary>
		/// <param name="list">List to add.</param>
		public static void Add(List<T> list)
		{
			list.Clear();
			Stack.Push(list);
		}
	}
}