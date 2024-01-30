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
    public class MultiSelect : TableSelect
    {
        /// <summary>
        /// Stored all selected cells.
        /// </summary>
        public List<object> SelectedObjects { get; protected set; }

        protected override void Awake()
        {
            base.Awake();
            SelectedObjects = new List<object>();
        }

        private void Start()
        {
            TableContent.SetCellEvent += ApplyCell;
            foreach (TableCell cell in TableContent.Cells)
            {
                cell.gameObject
                    .GetOrAddComponent<LeftClick>()
                    .OnClick
                    .AddListener(() => SelectCell(cell));
            }
        }

        public override bool CheckSelected(object value)
        {
            return value is not null && 
                   SelectedObjects.Contains(value);
        }

        public override bool SetSelected(object value)
        {
            if (value is null) return false;

            if (SelectedObjects.Contains(value))
            {
                SelectedObjects.Remove(value);
                InvokeSelectChanged(value, false);
                return false;
            }

            SelectedObjects.Add(value);
            InvokeSelectChanged(value, true);
            return true;
        }
    }
}