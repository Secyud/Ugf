using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.Unity.TableComponents.LocalTable
{
    public class LocalTablePager : TablePager
    {
        [SerializeField] private TextMeshProUGUI _pageText;
        [SerializeField] private Button _nextPageButton;
        [SerializeField] private Button _prePageButton;
        [SerializeField] private Button _firstPageButton;
        [SerializeField] private Button _lastPageButton;

        private int _currentPage;
        private int _maxPage;
        private int _maxCount;
        private Table _table;

        private void Start()
        {
            _maxCount = GetComponent<TableContent>().Cells.Length;
            _table = GetComponent<Table>();
        }

        public override void Apply()
        {
            if (Sorter is LocalTableSorter
                {
                    SortedData : not null
                } localSorter)
            {
                IList<object> data = localSorter.SortedData;

                _maxPage = (data.Count - 1) / _maxCount;
                if (_currentPage < 0)
                    _currentPage = 0;
                else if (_currentPage > _maxPage)
                    _currentPage = _maxPage;
                _pageText.text = $"{_currentPage + 1}/{_maxPage + 1}";

                PagedData.Clear();
                PagedData.AddRange(data
                    .Skip(_currentPage * _maxCount)
                    .Take(_maxCount));
            }
            else
            {
                _pageText.text = "-";
            }

            CheckButtonState();
        }

        private void CheckButtonState()
        {
            bool b = _currentPage != 0;
            _firstPageButton.interactable = b;
            _prePageButton.interactable = b;

            b = _currentPage != _maxPage;
            _nextPageButton.interactable = b;
            _lastPageButton.interactable = b;
        }

        public void TurnToPage(int page)
        {
            _currentPage = page;
            _table.Refresh(1);
        }

        public void TurnToFirstPage()
        {
            TurnToPage(0);
        }

        public void TurnToPreviewPage()
        {
            TurnToPage(_currentPage - 1);
        }

        public void TurnToNextPage()
        {
            TurnToPage(_currentPage + 1);
        }

        public void TurnToLastPage()
        {
            TurnToPage(_maxPage);
        }
    }
}