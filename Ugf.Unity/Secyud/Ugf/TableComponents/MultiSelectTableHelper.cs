#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Secyud.Ugf.TableComponents
{
	public class MultiSelectTableHelper<TItem, TCell, TListService, TSubTableHelper>
		where TListService : TableFunctionBase<TItem>
		where TCell : MonoBehaviour
		where TSubTableHelper : FunctionalTableHelper<TItem, TCell, TListService>
	{
		public readonly TSubTableHelper AllItemsTableHelper;
		public readonly TSubTableHelper SelectedTableHelper;

		public MultiSelectTableHelper(TSubTableHelper allItemsTableHelper, TSubTableHelper selectedTableHelper)
		{
			AllItemsTableHelper = allItemsTableHelper;
			SelectedTableHelper = selectedTableHelper;

			AllItemsTableHelper.BindPrepareCellAction(PrepareAllItemsCell);
			SelectedTableHelper.BindPrepareCellAction(PrepareSelectedCell);
		}

		protected virtual void PrepareAllItemsCell(TCell cell, int index)
		{
			cell.gameObject.GetOrAddButton(() => OnAllItemsCellClick(index));
		}

		protected virtual void PrepareSelectedCell(TCell cell, int index)
		{
			var button = cell.gameObject.GetOrAddButton(() => OnSelectedCellClick(index));
		}

		protected virtual void OnAllItemsCellClick(int index)
		{
			var item = AllItemsTableHelper[index];
			SelectedTableHelper.Add(item);
			AllItemsTableHelper.RemoveAt(index);
		}

		protected virtual void OnSelectedCellClick(int index)
		{
			var item = SelectedTableHelper[index];
			AllItemsTableHelper.Add(item);
			SelectedTableHelper.RemoveAt(index);
		}

		public void OnInitialize(
			MultiSelectTable table,
			TCell cellTemplate,
			IList<TItem> totalList,
			IList<TItem> selectedList)
		{
			AllItemsTableHelper.OnInitialize(table.AllItemsTable, cellTemplate, totalList);
			SelectedTableHelper.OnInitialize(table.SelectedTable, cellTemplate, selectedList);
		}
	}
}