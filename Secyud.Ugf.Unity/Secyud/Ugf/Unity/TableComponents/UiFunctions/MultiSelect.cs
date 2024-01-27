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
    public class MultiSelect: MonoBehaviour
    {
        [SerializeField] private Table _table;
        
        /// <summary>
        /// The event invoke when cell's select
        /// state is changed.
        /// </summary>
        public event Action<TableCell, bool> SelectChangedEvent;
        /// <summary>
        /// Stored all selected cells.
        /// </summary>
        public List<TableCell> SelectedCells { get; protected set; }

        private void Awake()
        {
            SelectedCells = new List<TableCell>();
        }

        private void Start()
        {
            foreach (TableCell cell in _table.Content.Cells)
            {
                cell.gameObject
                    .GetOrAddComponent<LeftClick>()
                    .OnClick
                    .AddListener(() => SelectCell(cell));
            }
        }

        protected virtual void SelectCell(TableCell cell)
        {
            if (SelectedCells.Contains(cell))
            {
                SelectedCells.Remove(cell);
                SelectChangedEvent?.Invoke(cell,false);
            }
            else
            {
                SelectedCells.Add(cell);
                SelectChangedEvent?.Invoke(cell,true);
            }
        }
    }
}