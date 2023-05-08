using System.Collections.Generic;
using System.Linq;

namespace Secyud.Ugf.Collections
{
	public class RegistrableDictionary<TKey, TItem>
		where TItem : IHasId<TKey>
	{
		private readonly Dictionary<TKey, TItem> _items = new();
		private readonly List<TKey> _keys = new();

		public void Register(TItem item)
		{
			if (!_items.ContainsKey(item.Id))
				_keys.Add(item.Id);
			_items[item.Id] = item;
		}

		public void RegisterList(params TItem[] items)
		{
			foreach (var item in items) Register(item);
		}

		public TItem Get(TKey key)
		{
			_items.TryGetValue(key, out var value);
			return value;
		}

		public TKey GetRandomKey()
		{
			return _keys.RandomPick();
		}
		public List<TKey> GetKeyList()
		{
			return _keys.ToList();
		}
	}
}