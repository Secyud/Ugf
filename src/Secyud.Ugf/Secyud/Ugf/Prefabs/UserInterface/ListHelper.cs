using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.Prefabs
{
    public class ListHelper<TItem>
    {
        private readonly Func<TItem, GameObject> _row;
        private readonly List<TItem> _items;

        private readonly List<GameObject> _rows = new();

        private readonly Func<IEnumerable<TItem>, IEnumerable<TItem>> _filter;
        private IEnumerable<TItem> _filteredList;
        private readonly Func<IEnumerable<TItem>, IEnumerable<TItem>> _sorter;
        private IEnumerable<TItem> _sortedList;
        private readonly Func<IEnumerable<TItem>, IEnumerable<TItem>> _pager;
        private IEnumerable<TItem> _pagedList;
        
        public ListHelper(
            List<TItem> items,
            Func<TItem,GameObject> row,
            Func<IEnumerable<TItem>,IEnumerable<TItem>> filter = null,
            Func<IEnumerable<TItem>,IEnumerable<TItem>> sorter = null,
            Func<IEnumerable<TItem>,IEnumerable<TItem>> pager = null)
        {
            _items = items;
            _row = row;
            _filter = filter;
            _sorter = sorter;
            _pager = pager;
        }

        public void ApplyFilter()
        {
            _filteredList = _filter is null ? _items : _filter(_items);
            ApplySorter();
        }
        
        public void ApplySorter()
        {
            _sortedList = _sorter is null ? _filteredList : _sorter(_filteredList);
            ApplyPager();
        }
        
        public void ApplyPager()
        {
            _pagedList = _pager is null ? _sortedList : _pager(_sortedList);
            RefreshTable();
        }

        public void RefreshTable()
        {
            foreach (var gameObject in _rows)
                Object.Destroy(gameObject);
            _rows.Clear();

            foreach (var item in _pagedList)
                _rows.Add(_row(item));
        }
    }
}