using System;
using System.Collections.Generic;
using Secyud.Ugf.Unity.Ui;
using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents.UiFunctions
{
    /// <summary>
    /// <para>
    /// Set multiselect action of table cell.
    /// </para>
    /// </summary>
    public class MultiSelect : MonoBehaviour
    {
        [SerializeField] private Table _table;

        /// <summary>
        /// The event invoke when cell's select
        /// state is changed.
        /// </summary>
        public event Action<object, bool> SelectChangedEvent;

        public event Action<TableCell, bool> SelectCellSetEvent;

        /// <summary>
        /// Stored all selected cells.
        /// </summary>
        public List<object> SelectedObjects { get; protected set; }

        public bool SetSelectedObject(object value)
        {
            if (value is null) return false;

            if (SelectedObjects.Contains(value))
            {
                SelectedObjects.Remove(value);
                SelectChangedEvent?.Invoke(value, false);
                return false;
            }
            else
            {
                SelectedObjects.Add(value);
                SelectChangedEvent?.Invoke(value, true);
                return true;
            }
        }

        private void Awake()
        {
            SelectedObjects = new List<object>();
        }

        private void Start()
        {
            _table.Content.SetCellEvent += ApplyCell;
            foreach (TableCell cell in _table.Content.Cells)
            {
                cell.gameObject
                    .GetOrAddComponent<LeftClick>()
                    .OnClick
                    .AddListener(() => SelectCell(cell));
            }
        }

        protected virtual void ApplyCell(TableCell cell)
        {
            bool result = SelectedObjects
                .Contains(cell.CellObject);
            ApplyCellWithResult(cell, result);
        }

        protected virtual void ApplyCellWithResult(TableCell cell, bool result)
        {
            if (cell is ShownCell shown && shown.Select)
                shown.Select.enabled = result;
            SelectCellSetEvent?.Invoke(cell, result);
        }

        protected virtual void SelectCell(TableCell cell)
        {
            bool result = SetSelectedObject(cell.CellObject);
            ApplyCellWithResult(cell,result);
        }
    }
}