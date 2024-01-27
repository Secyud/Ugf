using System;
using Secyud.Ugf.Unity.Ui;
using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents.UiFunctions
{
    public class SingleSelect : MonoBehaviour
    {
        [SerializeField] private Table _table;
        [SerializeField] private bool _cancelSelectOnSecondClick;

        private TableCell _selectedCell;

        public event Action<TableCell, TableCell> SelectChangedEvent;

        public TableCell SelectedCell
        {
            get => _selectedCell;
            protected set
            {
                if (_selectedCell == value) return;
                SelectChangedEvent?.Invoke(_selectedCell, value);
                _selectedCell = value;
            }
        }

        private void Start()
        {
            foreach (TableCell cell in _table.Content.Cells)
            {
                cell.gameObject
                    .GetOrAddComponent<LeftClick>()
                    .OnClick
                    .AddListener(() => ClickCell(cell));
            }
        }

        private void ClickCell(TableCell cell)
        {
            if (SelectedCell == cell)
            {
                if (_cancelSelectOnSecondClick)
                {
                    SelectedCell = null;
                }
            }
            else
            {
                SelectedCell = cell;
            }
        }
    }
}