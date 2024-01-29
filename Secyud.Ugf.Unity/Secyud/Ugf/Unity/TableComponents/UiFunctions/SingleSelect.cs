using System;
using Secyud.Ugf.Unity.Ui;
using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents.UiFunctions
{
    public class SingleSelect : MonoBehaviour
    {
        [SerializeField] private Table _table;
        [SerializeField] private bool _cancelSelectOnSecondClick;

        private object _selectedObject;

        public event Action<object, object> SelectChangedEvent;
        public event Action<TableCell, bool> SelectCellSetEvent;

        public object SelectedObject => _selectedObject;

        public bool SetSelectedObject(object value)
        {
            if (_selectedObject == value)
            {
                if (_cancelSelectOnSecondClick)
                {
                    if (_selectedObject is not null)
                    {
                        SelectChangedEvent?.Invoke(_selectedObject, null);
                    }
                    _selectedObject = null;
                    return false;
                }

                return true;
            }
            
            SelectChangedEvent?.Invoke(_selectedObject, value);
            
            _selectedObject = value;

            return true;
        }

        private void Start()
        {
            _table.Content.SetCellEvent += ApplyCell;
            foreach (TableCell cell in _table.Content.Cells)
            {
                cell.gameObject
                    .GetOrAddComponent<LeftClick>()
                    .OnClick
                    .AddListener(() => ClickCell(cell));
            }
        }

        protected virtual void ApplyCell(TableCell cell)
        {
            ApplyCellWithResult(cell, _selectedObject == cell.CellObject);
        }
        protected virtual void ApplyCellWithResult(TableCell cell,bool result)
        {
            if (cell is ShownCell shown && shown.Select)
                shown.Select.enabled = result;
            SelectCellSetEvent?.Invoke(cell, result);
        }

        private void ClickCell(TableCell cell)
        {
            bool result = SetSelectedObject(cell.CellObject);
            ApplyCellWithResult(cell,result);
        }
    }
}