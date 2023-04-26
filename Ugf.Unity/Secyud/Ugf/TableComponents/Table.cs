#region

using System;
using Secyud.Ugf.BasicComponents;
using UnityEngine;

#endregion

namespace Secyud.Ugf.TableComponents
{
    public class Table : MonoBehaviour
    {
        [SerializeField] private Transform TableContent;
        [SerializeField] private int PageSize = 12;
        [SerializeField] private SButton[] PageButtons;

        private Transform[] _cells;

        private int _page = 1;

        private int _refreshLevel;
        private ITableProperty _tableProperty;
        public int IndexFirst => (_page - 1) * PageSize;
        public int IndexLast => IndexFirst + PageSize;

        public ITableProperty TableProperty
        {
            get => _tableProperty;
            set
            {
                _tableProperty = value;
                Page = 1;
            }
        }

        public int Page
        {
            get => _page;
            private set
            {
                if (MaxPage == 0 || value < 1)
                    _page = 1;
                else if (value > MaxPage)
                    _page = MaxPage;
                else
                    _page = value;

                RefreshPage();
            }
        }

        public int MaxPage => (TableProperty.Count + PageSize - 1) / PageSize;

        public int Count => _tableProperty.Count;

        protected virtual void Awake()
        {
            _cells = new Transform[PageSize];
        }

        private void LateUpdate()
        {
            if (_refreshLevel > 0)
            {
                if (_refreshLevel > 1) TableProperty.ApplySorter();
                if (_refreshLevel > 2) TableProperty.ApplyFilter();
                _refreshLevel = 0;
                OnInitialize();
            }
        }

        public void TurnToFirstPage()
        {
            Page = 1;
        }

        public void TurnToPreviewPage()
        {
            Page -= 1;
        }

        public void TurnToNextPage()
        {
            Page += 1;
        }

        public void TurnToLastPage()
        {
            Page = MaxPage;
        }

        public void CheckButtonState()
        {
            if (Page <= 1)
            {
                PageButtons[0].enabled = false;
                PageButtons[1].enabled = false;
            }
            else
            {
                PageButtons[0].enabled = true;
                PageButtons[1].enabled = true;
            }

            if (Page >= MaxPage)
            {
                PageButtons[2].enabled = false;
                PageButtons[3].enabled = false;
            }
            else
            {
                PageButtons[2].enabled = true;
                PageButtons[3].enabled = true;
            }
        }

        public void RefreshFilter()
        {
            _refreshLevel = 3;
        }

        public void RefreshSorter()
        {
            _refreshLevel = 2;
        }

        public void RefreshPage()
        {
            _refreshLevel = 1;
        }


        public void InsertAt(int index)
        {
            var cellIndex = Math.Max(GetIndex(index), 0);
            if (cellIndex >= PageSize) return;

            if (_cells[PageSize - 1])
                Destroy(_cells[PageSize - 1].gameObject);

            for (var i = PageSize - 1; i > cellIndex; i--)
            {
                if (!_cells[i - 1]) continue;
                _cells[i] = _cells[i - 1];
                TableProperty.ResetCell(_cells[i], i + IndexFirst);
            }

            _cells[cellIndex] = TableProperty.CreateCell(TableContent, IndexFirst + cellIndex);
            if (_cells[cellIndex])
                _cells[cellIndex].SetSiblingIndex(cellIndex);
        }

        public void RemoveAt(int index)
        {
            var cellIndex = Math.Max(GetIndex(index), 0);
            if (cellIndex >= PageSize) return;

            if (_cells[cellIndex])
                Destroy(_cells[cellIndex].gameObject);
            else
                return;

            for (var i = cellIndex; i < PageSize - 1; i++)
            {
                _cells[i] = _cells[i + 1];
                if (!_cells[i]) return;
                TableProperty.ResetCell(_cells[i], i + IndexFirst);
            }


            _cells[PageSize - 1] = TableProperty.CreateCell(TableContent, IndexLast - 1);
        }

        public void ReplaceAt(int index)
        {
            var cellIndex = GetIndex(index);
            if (cellIndex >= PageSize || cellIndex < 0) return;
            if (_cells[cellIndex])
                Destroy(_cells[cellIndex].gameObject);

            _cells[cellIndex] = TableProperty.CreateCell(TableContent, index);
            if (_cells[cellIndex])
                _cells[cellIndex].SetSiblingIndex(cellIndex);
        }

        public void OnInitialize()
        {
            Clear();
            for (var i = 0; i < PageSize; i++)
            {
                _cells[i] = TableProperty.CreateCell(TableContent, i + IndexFirst);
                if (!_cells[i])
                    return;
            }
        }

        public void Clear()
        {
            for (var i = 0; i < PageSize; i++)
            {
                if (_cells[i])
                    Destroy(_cells[i].gameObject);
                else
                    return;
                _cells[i] = null;
            }
        }

        private int GetIndex(int index)
        {
            return index - IndexFirst;
        }
    }
}