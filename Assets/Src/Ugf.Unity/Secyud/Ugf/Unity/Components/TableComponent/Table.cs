#region

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Secyud.Ugf.Unity.Components
{
    public class Table : MonoBehaviour
    {
        [SerializeField] private Transform TableContent;
        [SerializeField] private int PageSize = 12;
        [SerializeField] private SButton[] PageButtons;

        private int _refreshLevel;
        private ITableProperty _tableProperty;
        public int IndexFirst { get; private set; }
        public int IndexLast { get; private set; }

        public ITableProperty TableProperty
        {
            get => _tableProperty;
            set
            {
                _tableProperty = value;
                Page = 1;
            }
        }

        public List<Transform> Cells { get; private set; }

        protected virtual void Awake()
        {
            Cells = new List<Transform>();
        }

        private int _page;

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

                CalculateIndex();
                Refresh();
            }
        }

        public int MaxPage => (TableProperty.ItemsCount + PageSize - 1) / PageSize;

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

        private void CalculateIndex()
        {
            IndexFirst = Math.Min((_page - 1) * PageSize, TableProperty.ItemsCount);
            IndexLast = Math.Min(IndexFirst + PageSize, TableProperty.ItemsCount);
            CheckButtonState();
        }

        public void SetPageSize(int pageSize)
        {
            PageSize = pageSize;
            Page = 1;
            CheckButtonState();
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

        private void LateUpdate()
        {
            if (_refreshLevel > 0)
            {
                if (_refreshLevel > 1) TableProperty.ApplySorter();
                if (_refreshLevel > 2) TableProperty.ApplyFilter();
                _refreshLevel = 0;
                RefreshRangeImmediately(IndexFirst, IndexLast);
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

        public void Refresh()
        {
            _refreshLevel = 1;
        }

        public void RefreshOneCell(int index)
        {
            if (index >= IndexFirst && index < IndexLast)
            {
                int i = index - IndexFirst;
                Destroy(Cells[i].gameObject);
                Cells.RemoveAt(i);
                Cells.Insert(i, TableProperty.SetCell(TableContent, index));
            }
        }

        public void InsertOneCell(int index)
        {
            CalculateIndex();
            if (index >= IndexFirst && index < IndexLast)
            {
                int i = index - IndexFirst;

                if (IndexLast - IndexFirst >= PageSize)
                {
                    Destroy(Cells[^1].gameObject);
                    Cells.RemoveAt(Cells.Count - 1);
                }

                Cells.Insert(i, TableProperty.SetCell(TableContent, IndexLast - 1));
            }
        }

        public void RemoveOneCell(int index)
        {
            CalculateIndex();
            if (index >= IndexFirst && index <= IndexLast)
            {
                int i = index - IndexFirst;
                Destroy(Cells[i].gameObject);
                Cells.RemoveAt(i);
                if (IndexLast <= TableProperty.ItemsCount && IndexLast - IndexFirst == PageSize)
                    Cells.Insert(IndexLast - 1, TableProperty.SetCell(TableContent, IndexLast - 1));
            }
        }

        public void RefreshRangeImmediately(int startIndex, int endIndex)
        {
            if (endIndex - IndexFirst > PageSize)
                endIndex = PageSize + IndexFirst;

            for (int i = startIndex - IndexFirst; i < Math.Min(endIndex - IndexFirst, Cells.Count); i++)
            {
                Destroy(Cells[i].gameObject);
                Cells.RemoveAt(i);
            }

            for (int i = startIndex; i < Math.Min(endIndex, IndexLast); i++)
                Cells.Insert(i - IndexFirst, TableProperty.SetCell(TableContent, i));
        }

        public void Clear()
        {
            foreach (var t in Cells)
                Destroy(t.gameObject);
            Cells.Clear();
        }
    }
}