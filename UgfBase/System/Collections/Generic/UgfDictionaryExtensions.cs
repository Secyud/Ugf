#region

using System.Dynamic;

#endregion

namespace System.Collections.Generic
{
	public static class UgfDictionaryExtensions
	{
		internal static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T value)
		{
			if (dictionary.TryGetValue(key, out var obj) && obj is T valueT)
			{
				value = valueT;
				return true;
			}

			value = default;
			return false;
		}

		public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
		{
			return dictionary.TryGetValue(key, out var obj) ? obj : default;
		}

		public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
		{
			return dictionary.TryGetValue(key, out var obj) ? obj : default;
		}

		public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary,
			TKey key)
		{
			return dictionary.TryGetValue(key, out var obj) ? obj : default;
		}

		public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
			Func<TKey, TValue> factory)
		{
			if (dictionary.TryGetValue(key, out var obj))
				return obj;

			return dictionary[key] = factory(key);
		}

		public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
			Func<TValue> factory)
		{
			return dictionary.GetOrAdd(key, k => factory());
		}

		public static dynamic ConvertToDynamicObject(this Dictionary<string, object> dictionary)
		{
			var expandoObject = new ExpandoObject();
			ICollection<KeyValuePair<string, object>> expendObjectCollection =
				expandoObject;

			foreach (var keyValuePair in dictionary)
				expendObjectCollection.Add(keyValuePair);

			return expandoObject;
		}
	}
}