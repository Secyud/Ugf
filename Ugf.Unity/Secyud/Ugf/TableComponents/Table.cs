#region

using System;
using System.Collections.Generic;
using Secyud.Ugf.LayoutComponents;
using UnityEngine;

#endregion

namespace Secyud.Ugf.TableComponents
{
    public sealed class Table : TableComponentBase<Table, TableDelegate>
    {
        public override string Name => nameof(Table);

        [SerializeField] private LayoutGroupTrigger TableContent;
        [SerializeField] private TableComponentBase[] TableComponents;
        [SerializeField] private TableCell CellTemplate;

        private Dictionary<string, TableComponentBase> _componentDict;
        private readonly SortedDictionary<int, Action> _refreshAction = new();

        private TableCell[] _cells;

        public RectTransform Content => TableContent.RectTransform;
        public TableCell CellPrefab => CellTemplate;

        public TableComponentBase this[string componentName]
        {
            get
            {
                if (_componentDict is null)
                {
                    _componentDict = new Dictionary<string, TableComponentBase>();


                    foreach (TableComponentBase component in TableComponents)
                    {
                        if (component is not null)
                        {
                            _componentDict[component.Name] = component;
                        }
                    }
                }

                _componentDict.TryGetValue(componentName, out TableComponentBase v);
                return v;
            }
        }

        private void Awake()
        {
            _cells = Array.Empty<TableCell>();
        }

        public void AddRefreshAction(int index, Action action)
        {
            enabled = true;
            _refreshAction[index] = action;
        }

        public void Refresh()
        {
            enabled = true;
        }

        private void LateUpdate()
        {
            if (Delegate is not null)
            {
                Delegate.RefreshPrepare();
                foreach (Action action in _refreshAction.Values)
                    action.Invoke();
                OnInitialize();
            }

            enabled = false;
        }


        public void InsertAt(int index)
        {
            if (_cells[^1])
                Destroy(_cells[^1].gameObject);

            int indexFirst = Delegate.IndexFirst;
            int length = _cells.Length - 1;

            for (int i = length; i > index; i--)
            {
                if (!_cells[i - 1]) continue;
                _cells[i] = _cells[i - 1];
                _cells[i].CellIndex = indexFirst + i;
            }

            SetParentOfCell(index);
        }

        public void RemoveAt(int index)
        {
            if (_cells[index])
                Destroy(_cells[index].gameObject);
            else
                return;

            int indexFirst = Delegate.IndexFirst;
            // only need to check until last but one
            int length = _cells.Length - 1;

            for (int i = index; i < length; i++)
            {
                _cells[i] = _cells[i + 1];
                if (!_cells[i])
                {
                    TableContent.enabled = true;
                    return;
                }

                _cells[i].CellIndex = indexFirst + i;
            }

            SetParentOfCell(length);
        }

        public void ReplaceAt(int index)
        {
            if (_cells[index])
                Destroy(_cells[index].gameObject);

            SetParentOfCell(index);
        }

        public void Clear()
        {
            for (int i = 0; i < _cells.Length; i++)
            {
                if (!_cells[i]) continue;
                Destroy(_cells[i].gameObject);
                _cells[i] = null;
            }
        }

        public void SetParentOfCell(int index)
        {
            TableCell fillCell = Delegate.CreateCell(index);

            _cells[index] = fillCell;

            TableContent.enabled = true;
        }

        public void SetCellCount(int count)
        {
            _cells = new TableCell[count];
        }

        public void OnInitialize()
        {
            Clear();
            for (int i = 0; i < _cells.Length; i++)
                SetParentOfCell(i);
        }
    }
}