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
        private float _totalSize;
        private float _viewSize;
        private float _currentPosition;
        private Vector2 _start;
        private int _previous;
        private GridLayoutGroup.Constraint _constraint;

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
            _constraint = grid.constraint;
            _height = grid.spacing.y + grid.cellSize.y;
            _width = grid.spacing.x + grid.cellSize.x;

            Rect viewRect = scrollRect.viewport.rect;
            _tableContent = GetComponent<TableContent>();
            int totalCount = _tableContent.Cells.Length;

            switch (_constraint)
            {
                case GridLayoutGroup.Constraint.Flexible: break;
                case GridLayoutGroup.Constraint.FixedColumnCount:
                    _viewSize = viewRect.height;
                    _cols = grid.constraintCount;
                    _rows = totalCount / _cols;
                    break;
                case GridLayoutGroup.Constraint.FixedRowCount:
                    _viewSize = viewRect.width;
                    _rows = grid.constraintCount;
                    _cols = totalCount / _cols;
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

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

                switch (_constraint)
                {
                    case GridLayoutGroup.Constraint.Flexible: break;
                    case GridLayoutGroup.Constraint.FixedColumnCount:
                        _totalSize = Mathf.Max(0, _height * data.Count / _cols - _viewSize);
                        Vector2 vy = _content.sizeDelta;
                        vy.y = _totalSize + _viewSize;
                        _content.sizeDelta = vy;

                        int rowPrevious = (int)(_currentPosition / _height) - 1;

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

                        break;
                    case GridLayoutGroup.Constraint.FixedRowCount:
                        _totalSize = Mathf.Max(0, _width * data.Count / _rows - _viewSize);
                        Vector2 vx = _content.sizeDelta;
                        vx.x = _totalSize + _viewSize;
                        _content.sizeDelta = vx;
                        int colPrevious = (int)(_currentPosition / _width) - 1;
                        PagedData.Clear();

                        for (int i = 0; i < _cols; i++)
                        {
                            int col = i + colPrevious;
                            for (int j = 0; j < _rows; j++)
                            {
                                int cellIndex = i * _rows + j;
                                int dataIndex = (col + _cols) % _cols * _rows + j;
                                _tableContent.Cells[cellIndex].transform.localPosition
                                    = new Vector2(_width * col, -_height * j) + _start;

                                PagedData.Add(col >= 0 && data.Count > dataIndex
                                    ? data[dataIndex]
                                    : null);
                            }
                        }

                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void OnPositionChanged(Vector2 vector)
        {
            switch (_constraint)
            {
                case GridLayoutGroup.Constraint.Flexible: break;
                case GridLayoutGroup.Constraint.FixedColumnCount:
                    float height = (1 - vector.y) * _totalSize;
                    if (Mathf.Abs(height - _currentPosition) > _height)
                    {
                        _currentPosition = height;
                        _table.Refresh(1);
#if DEBUG
                        UgfLogger.Log("Pos: " + vector);
#endif
                    }

                    break;
                case GridLayoutGroup.Constraint.FixedRowCount:
                    float width = (1 - vector.x) * _totalSize;
                    if (Mathf.Abs(width - _currentPosition) > _width)
                    {
                        _currentPosition = width;
                        _table.Refresh(1);
#if DEBUG
                        UgfLogger.Log("Pos: " + vector);
#endif
                    }

                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}