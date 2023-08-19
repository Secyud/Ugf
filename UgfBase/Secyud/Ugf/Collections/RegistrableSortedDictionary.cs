﻿#region

using System.Collections.Generic;
using System.Linq;
using System.Ugf.Collections.Generic;

#endregion

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
            foreach (TItem item in items) Register(item);
        }

        public TItem Get(TKey key)
        {
            _items.TryGetValue(key, out TItem value);
            return value;
        }
        
        public TItem GetByIndex(int index)
        {
            return _keys.Any() ? _items[_keys[index]] : default;
        }

        public TKey GetRandomKey()
        {
            return _keys.RandomPick();
        }

        public IReadOnlyList<TKey> KeyList => _keys;
    }
}