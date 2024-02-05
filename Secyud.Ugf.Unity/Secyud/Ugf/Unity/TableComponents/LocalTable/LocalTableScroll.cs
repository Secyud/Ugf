using System;
using System.Collections.Generic;
using Secyud.Ugf.Logging;
using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.Unity.TableComponents.LocalTable
{
    [RequireComponent(typeof(ScrollRect))]
    public class LocalTableScroll : TablePager
    {
        private Table _table;
        private RectTransform _content;
        private TableContent _tableContent;
        private int _rows;
        private int _cols;
        private float _height;
        private float _width;
        private float _totalHeight;
        private float _viewHeight;
        private float _currentHeight;
        private Vector2 _start;
        private int _rowPrevious;

        protected override void Awake()
        {
            base.Awake();
            _table = GetComponent<Table>();
        }

        private void Start()
        {
            ScrollRect scrollRect = GetComponent<ScrollRect>();
            scrollRect.onValueChanged.AddListener(OnPositionChanged);
            _content = scrollRect.content;
            GridLayoutGroup grid = _content
                .GetComponent<GridLayoutGroup>();
            _height = grid.spacing.y + grid.cellSize.y;
            _width = grid.spacing.x + grid.cellSize.x;
            _viewHeight = scrollRect.viewport.rect.height;
            _tableContent = GetComponent<TableContent>();
            int totalCount = _tableContent.Cells.Length;
            _cols = grid.constraintCount;
            _rows = totalCount / _cols;
            _start = _tableContent.Cells[0].transform.localPosition;
        }

        public override void Apply()
        {
            if (Sorter is LocalTableSorter
                {
                    SortedData : not null
                } localSorter)
            {
                IList<object> data = localSorter.SortedData;

                _totalHeight = Mathf.Max(0, _height * data.Count / _cols - _viewHeight);
                Vector2 v = _content.sizeDelta;
                v.y = _totalHeight + _viewHeight;
                _content.sizeDelta = v;

                int rowPrevious = (int)(_currentHeight / _height) - 1;

                PagedData.Clear();

                for (int i = 0; i < _rows; i++)
                {
                    int row = i + rowPrevious;
                    for (int j = 0; j < _cols; j++)
                    {
                        int cellIndex = i * _cols + j;
                        int dataIndex = (row + _rows) % _rows * _cols + j;
                        _tableContent.Cells[cellIndex].transform.localPosition
                            = new Vector2(_width * j, -_height * row) + _start;

                        PagedData.Add(row >= 0 && data.Count > dataIndex
                            ? data[dataIndex]
                            : null);
                    }
                }
            }
        }

        private void OnPositionChanged(Vector2 vector)
        {
            float height = (1 - vector.y) * _totalHeight;
            if (Mathf.Abs(height - _currentHeight) > _height)
            {
                _currentHeight = height;
                _table.Refresh(1);
#if DEBUG
                UgfLogger.Log("Pos: " + vector);
#endif
            }
        }
    }
}