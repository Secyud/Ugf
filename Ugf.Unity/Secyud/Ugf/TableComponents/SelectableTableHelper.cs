#region

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Secyud.Ugf.TableComponents
{
    public abstract class
        SelectableTableHelper<TItem, TCell, TListService> : FunctionalTableHelper<TItem, TCell, TListService>
        where TListService : TableFunctionBase<TItem>
        where TCell : MonoBehaviour
    {
        private TItem _selectedItem;
        public Action<TItem> CallBackAction;

        public void OnInitialize(SelectableTable table, TCell cellTemplate, IList<TItem> totalItems)
        {
            table.EnsureAction += OnEnsure;
            base.OnInitialize(table, cellTemplate, totalItems);
        }

        private void OnEnsure()
        {
            CallBackAction?.Invoke(_selectedItem);
            CallBackAction = null;
        }

        public void ClearCallBackActions()
        {
            CallBackAction = null;
        }

        public override Transform CreateCell(Transform content, int index)
        {
            var transform = base.CreateCell(content, index);
            if (transform) transform.gameObject.GetOrAddButton(() => _selectedItem = Items[index]);
            return transform;
        }

        public override void ResetCell(Transform cell, int index)
        {
            base.ResetCell(cell, index);
            if (cell) cell.gameObject.GetOrAddButton(() => _selectedItem = Items[index]);
        }
    }
}