using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents.LocalTable
{
    public class LocalTablePager : TablePager
    {
        [SerializeField] private int _maxCount;
        [SerializeField] private TextMeshProUGUI _pageText;

        private int _currentPage;
        private int _maxPage;

        public override void Apply()
        {
            if (Table.Sorter is LocalTableSorter
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
                _pageText.text = $"{_currentPage}/{_maxPage}";

                PagedData = data
                    .Skip(_currentPage * _maxCount)
                    .Take(_maxCount)
                    .ToList();
            }
            else
            {
                _pageText.text = "-";
            }
        }

        public void TurnToPage(int page)
        {
            _currentPage = page;
            Table.Refresh(1);
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