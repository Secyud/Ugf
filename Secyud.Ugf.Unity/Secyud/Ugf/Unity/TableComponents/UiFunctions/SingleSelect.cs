using Secyud.Ugf.Unity.Ui;
using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents.UiFunctions
{
    public class SingleSelect : TableSelect
    {
        [SerializeField] private bool _cancelSelectOnSecondClick;

        private object _selectedObject;
        public object SelectedObject => _selectedObject;


        public override bool CheckSelected(object value)
        {
            return _selectedObject == value;
        }

        public override bool SetSelected(object value)
        {
            if (_selectedObject == value)
            {
                if (_selectedObject is null)
                    return false;

                if (_cancelSelectOnSecondClick)
                {
                    InvokeSelectChanged(_selectedObject, false);
                    _selectedObject = null;
                    return false;
                }

                return true;
            }

            if (_selectedObject is not null)
            {
                InvokeSelectChanged(_selectedObject, false);
                for (int i = 0; i < TableContent.Cells.Length; i++)
                {
                    TableCell cell = TableContent.Cells[i];
                    if (cell.CellObject == _selectedObject)
                    {
                        ApplyCellWithResult(cell, false);
                        break;
                    }
                }
            }

            _selectedObject = value;
            InvokeSelectChanged(_selectedObject, true);
            return true;
        }

        private void Start()
        {
            TableContent.SetCellEvent += ApplyCell;
            foreach (TableCell cell in TableContent.Cells)
            {
                cell.GetOrAddComponent<LeftClick>()
                    .OnClick
                    .AddListener(() => ClickCell(cell));
            }
        }

        private void ClickCell(TableCell cell)
        {
            bool result = SetSelected(cell.CellObject);
            ApplyCellWithResult(cell, result);
        }
    }
}