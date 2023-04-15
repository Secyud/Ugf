#region

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace Secyud.Ugf.Unity.Components
{
    public abstract class TableHelper<TItem, TCell> : ITableProperty, IList<TItem>
        where TCell : MonoBehaviour
    {
        public Action<TCell, int> PrepareCellAction;
        public int ItemsCount => ShowItems.Count;
        protected IList<TItem> ShowItems;
        protected TCell CellTemplate { get; private set; }
        protected Table Table { get; private set; }

        public void OnInitialize(Table table, TCell cellTemplate, IList<TItem> showItems)
        {
            Table = table;
            CellTemplate = cellTemplate;
            ShowItems = showItems;
            table.TableProperty = this;
        }

        public virtual Transform SetCell(Transform content, int index)
        {
            TCell cell = Object.Instantiate(CellTemplate, content);
            cell.transform.SetSiblingIndex(index - Table.IndexFirst);
            PrepareCellAction?.Invoke(cell, index);
            SetCell(cell, ShowItems[index]);
            return cell.transform;
        }

        public virtual void ApplyFilter()
        {
        }

        public virtual void ApplySorter()
        {
        }

        protected abstract void SetCell(TCell cell, TItem item);

        public int IndexOf(TItem item)
        {
            return ShowItems.IndexOf(item);
        }

        public void Insert(int index, TItem item)
        {
            ShowItems.Insert(index, item);
            Table.InsertOneCell(index);
        }

        public void RemoveAt(int index)
        {
            ShowItems.RemoveAt(index);
            Table.RemoveOneCell(index);
        }

        public TItem this[int index]
        {
            get => ShowItems[index];
            set => ShowItems[index] = value;
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return ShowItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TItem item)
        {
            ShowItems.AddLast(item);

            Table.InsertOneCell(ShowItems.Count - 1);
        }

        public void Clear()
        {
            ShowItems.Clear();
            Table.Clear();
        }

        public bool Contains(TItem item)
        {
            return ShowItems.Contains(item);
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            ShowItems.CopyTo(array, arrayIndex);
            Table.RefreshRangeImmediately(arrayIndex, array.Length + arrayIndex);
        }

        public bool Remove(TItem item)
        {
            var index = ShowItems.IndexOf(item);
            if (index < 0) return false;
            RemoveAt(index);
            return true;
        }

        public int Count => ShowItems.Count;
        public bool IsReadOnly => ShowItems.IsReadOnly;
    }
}