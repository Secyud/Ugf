#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Ugf.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace Secyud.Ugf.TableComponents
{
	public abstract class TableHelper<TItem, TCell> : ITableProperty, IList<TItem>
		where TCell : MonoBehaviour
	{
		private Action<TCell, int> _prepareCellAction;
		protected TCell CellTemplate;
		protected IList<TItem> Items;
		protected Table Table;

		public int IndexOf(TItem item)
		{
			return Items.IndexOf(item);
		}

		public void Insert(int index, TItem item)
		{
			Items.Insert(index, item);
			Table.InsertAt(index);
		}

		public void RemoveAt(int index)
		{
			Items.RemoveAt(index);
			Table.RemoveAt(index);
		}

		public TItem this[int index]
		{
			get => Items[index];
			set => Items[index] = value;
		}

		public IEnumerator<TItem> GetEnumerator()
		{
			return Items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(TItem item)
		{
			Items.AddLast(item);
			Table.InsertAt(Items.Count - 1);
		}

		public void Clear()
		{
			Items.Clear();
			Table.Clear();
		}

		public bool Contains(TItem item)
		{
			return Items.Contains(item);
		}

		public void CopyTo(TItem[] array, int arrayIndex)
		{
			Items.CopyTo(array, arrayIndex);
			for (int i = arrayIndex; i < array.Length + arrayIndex; i++) Table.ReplaceAt(i);
		}

		public bool Remove(TItem item)
		{
			int index = Items.IndexOf(item);
			if (index < 0) return false;

			RemoveAt(index);
			return true;
		}

		public bool IsReadOnly => Items.IsReadOnly;

		public virtual Transform CreateCell(Transform content, int index)
		{
			if (index >= Count)
				return null;

			TCell cell = Object.Instantiate(CellTemplate, content);
			_prepareCellAction?.Invoke(cell, index);
			SetCell(cell, Items[index]);
			return cell.transform;
		}

		public virtual void ResetCell(Transform cell, int index)
		{
			TCell c = cell.GetComponent<TCell>();
			_prepareCellAction?.Invoke(c, index);
			SetCell(c, Items[index]);
		}

		public virtual void ApplyFilter()
		{
		}

		public virtual void ApplySorter()
		{
		}

		public int Count => Items.Count;

		public void BindPrepareCellAction(Action<TCell, int> action)
		{
			_prepareCellAction = action;
		}

		public void OnInitialize(Table table, TCell cellTemplate, IList<TItem> showItems)
		{
			Table = table;
			CellTemplate = cellTemplate;
			Items = showItems;
			table.TableProperty = this;
		}

		protected abstract void SetCell(TCell cell, TItem item);
	}
}