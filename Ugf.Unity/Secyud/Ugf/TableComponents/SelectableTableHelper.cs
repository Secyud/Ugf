#region

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Secyud.Ugf.TableComponents
{
	public abstract class
		SelectableTableHelper<TItem, TCell, TListService> : FunctionalTableHelper<TItem, TCell, TListService>
		where TListService : TableFunctionBase<TItem>
		where TCell : MonoBehaviour
	{
		private SelectableTable SelectableTable => (SelectableTable)Table;

		protected TItem SelectedItem
		{
			get => _selectedItem;
			set
			{
				_selectedItem = value;
				for (int i = 0; i < SelectableTable.ShowCellContent.childCount; i++)
					Object.Destroy(SelectableTable.ShowCellContent.GetChild(i).gameObject);
				TCell cell = Object.Instantiate(CellTemplate, SelectableTable.ShowCellContent);
				SetCell(cell, _selectedItem);
			}
		}
		public UnityAction<TItem> CallBackAction;
		private TItem _selectedItem;

		public void OnInitialize(SelectableTable table, TCell cellTemplate, IList<TItem> totalItems)
		{
			table.EnsureAction += OnEnsure;
			base.OnInitialize(table, cellTemplate, totalItems);
		}

		private void OnEnsure()
		{
			CallBackAction?.Invoke(SelectedItem);
			CallBackAction = null;
		}

		public void ClearCallBackActions()
		{
			CallBackAction = null;
		}

		public override Transform CreateCell(Transform content, int index)
		{
			Transform transform = base.CreateCell(content, index);
			if (transform) transform.gameObject.GetOrAddButton(() => SelectedItem = Items[index]);
			return transform;
		}

		public override void ResetCell(Transform cell, int index)
		{
			base.ResetCell(cell, index);
			if (cell) cell.gameObject.GetOrAddButton(() => SelectedItem = Items[index]);
		}
	}
}