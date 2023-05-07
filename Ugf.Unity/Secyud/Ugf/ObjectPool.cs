using System.Collections.Generic;

namespace Secyud.Ugf
{
	public static class ObjectPool<T>
	{
		private static readonly Stack<T> Stack = new();

		/// <summary>
		///     Get a pooled list.
		/// </summary>
		/// <returns>The requested list.</returns>
		public static T Get()
		{
			return Stack.Count > 0 ? Stack.Pop() : default;
		}

		/// <summary>
		///     Add a list back to the pool so it can be reused.
		/// </summary>
		/// <param name="t">T to add.</param>
		public static void Add(T t)
		{
			Stack.Push(t);
		}
	}
}