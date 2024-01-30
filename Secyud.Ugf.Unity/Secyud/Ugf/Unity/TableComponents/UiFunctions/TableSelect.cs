using System;
using Secyud.Ugf.Unity.Ui;
using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents.UiFunctions
{
    [RequireComponent(typeof(Table))]
    public abstract class TableSelect : MonoBehaviour
    {
        protected TableContent TableContent;
        protected virtual void Awake()
        {
            TableContent = GetComponent<TableContent>();
        }

        /// <summary>
        /// The event invoke when cell's select
        /// state is changed.
        /// </summary>
        public event Action<object, bool> SelectChangedEvent;

        public event Action<TableCell, bool> TableCellSetEvent;

        protected void InvokeSelectChanged(object value, bool result)
        {
            SelectChangedEvent?.Invoke(value, result);
        }

        public abstract bool CheckSelected(object value);

        public abstract bool SetSelected(object value);

        protected virtual void ApplyCell(TableCell cell)
        {
            bool result = CheckSelected(cell.CellObject);
            ApplyCellWithResult(cell, result);
        }

        protected virtual void ApplyCellWithResult(TableCell cell, bool result)
        {
            if (cell is ShownCell shown && shown.Select)
                shown.Select.enabled = result;
            TableCellSetEvent?.Invoke(cell, result);
        }

        protected virtual void SelectCell(TableCell cell)
        {
            bool result = SetSelected(cell.CellObject);
            ApplyCellWithResult(cell, result);
        }
    }
}