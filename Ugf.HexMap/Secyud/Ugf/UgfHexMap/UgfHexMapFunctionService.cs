using System;
using System.Collections;
using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.HexMap;
using Secyud.Ugf.HexUtilities;
using UnityEngine;

namespace Secyud.Ugf.UgfHexMap
{
    public interface IUgfHexMapFunction
    {
        int CurrentPathFromIndex { get; }
        int CurrentPathToIndex { get; }
        bool HasPath { get; }
        void Travel();
    }

    public abstract class UgfHexMapFunctionService<TMessageService> :
        IUgfHexMapFunction, IRegistry
        where TMessageService : IHexMapMessageService
    {
        private readonly UgfCellPriorityQueue _searchFrontier = new();
        private int _searchFrontierPhase;
        private TMessageService _messageService;
        public bool HasPath { get; private set; }

        public int CurrentPathFromIndex { get; private set; }
        public int CurrentPathToIndex { get; private set; }

        protected UgfHexMapFunctionService(TMessageService service)
        {
            _messageService = service;
        }

        /// <summary>
        ///     Get a list of cells representing the currently visible path.
        /// </summary>
        /// <returns>The current path list, if a visible path exists.</returns>
        public IList<int> GetPath()
        {
            var grid = _messageService.Grid;

            if (!HasPath)
            {
                return Array.Empty<int>();
            }

            List<int> path = new();
            for (int c = CurrentPathToIndex;
                 c != CurrentPathFromIndex;
                 c = (grid.GetCell(c) as UgfCell)!.PathFromIndex)
            {
                path.Add(c);
            }

            path.Add(CurrentPathFromIndex);
            path.Reverse();
            return path;
        }

        /// <summary>
        ///     Clear the current path.
        /// </summary>
        public void ClearPath()
        {
            if (HasPath)
            {
                var grid = _messageService.Grid;
                int current = CurrentPathToIndex;
                while (current != CurrentPathFromIndex)
                {
                    UgfCell cell = grid.GetCell(current) as UgfCell;
                    cell!.SetLabel(null);
                    current = cell.PathFromIndex;
                }

                HasPath = false;
            }

            CurrentPathFromIndex = CurrentPathToIndex = -1;
        }

        /// <summary>
        ///     Try to find a path.
        /// </summary>
        /// <param name="fromCell">Cell to start the search from.</param>
        /// <param name="toCell">Cell to find a path towards.</param>
        /// <param name="unit">Unit for which the path is.</param>
        public void FindPath(UgfCell fromCell, UgfCell toCell, HexUnit unit)
        {
            ClearPath();
            CurrentPathFromIndex = fromCell.Index;
            CurrentPathToIndex = toCell.Index;
            HasPath = Search(fromCell, toCell, unit);
            _messageService.ShowPath(this, unit);
        }

        private bool Search(UgfCell fromCell, UgfCell toCell, HexUnit unit)
        {
            float speed = _messageService.GetSpeed(unit);
            _searchFrontierPhase += 2;

            _searchFrontier.Clear();

            fromCell.SearchPhase = _searchFrontierPhase;
            fromCell.Distance = 0;
            _searchFrontier.Enqueue(fromCell);
            while (_searchFrontier.Count > 0)
            {
                UgfCell current = _searchFrontier.Dequeue();
                current.SearchPhase += 1;

                if (current == toCell) return true;

                float currentTurn = (current.Distance - 1) / speed;

                for (HexDirection d = HexDirection.Ne; d <= HexDirection.Nw; d++)
                {
                    UgfCell neighbor = current.GetNeighbor(d);
                    if (neighbor is null || neighbor.SearchPhase > _searchFrontierPhase)
                        continue;

                    float moveCost = _messageService.GetMoveCost(
                        current, neighbor, d);
                    if (moveCost < 0) continue;

                    float distance = current.Distance + moveCost;
                    float turn = (distance - 1) / speed;
                    if (turn > currentTurn)
                    {
                        distance = turn * speed + moveCost;
                    }

                    if (neighbor.SearchPhase < _searchFrontierPhase)
                    {
                        neighbor.SearchPhase = _searchFrontierPhase;
                        neighbor.Distance = (int)distance;
                        neighbor.PathFromIndex = current.Index;
                        neighbor.SearchHeuristic = neighbor.DistanceTo(toCell);
                        _searchFrontier.Enqueue(neighbor);
                    }
                    else if (distance < neighbor.Distance)
                    {
                        int oldPriority = neighbor.SearchPriority;
                        neighbor.Distance = (int)distance;
                        neighbor.PathFromIndex = current.Index;
                        _searchFrontier.Change(neighbor, oldPriority);
                    }
                }
            }

            return false;
        }

        public void Travel()
        {
            if (HasPath)
            {
                IList<int> path = GetPath();
                int location = path[0];
                HexUnit unit = _messageService.Grid.GetCell(location).Unit;
                if (unit)
                {
                    unit.Location = _messageService.Grid.GetCell(path[^1]);
                    unit.StopAllCoroutines();
                    unit.StartCoroutine(TravelPath(path, unit));
                }
            }
        }

        private IEnumerator TravelPath(IList<int> path, HexUnit unit)
        {
            var grid = _messageService.Grid;
            Vector3 a, b, c = grid.GetCell(path[0]).Position;
            Transform transform = unit.transform;

            float t = Time.deltaTime * _messageService.TravelSpeed;
            for (int i = 1; i < path.Count; i++)
            {
                UgfCell currentTravelLocation = grid.GetCell(path[i]) as UgfCell;
                a = c;
                b = grid.GetCell(path[i - 1]).Position;

                c = (b + currentTravelLocation!.Position) * 0.5f;

                for (; t < 1f; t += Time.deltaTime * _messageService.TravelSpeed)
                {
                    transform.localPosition = Bezier.GetPoint(a, b, c, t);
                    Vector3 d = Bezier.GetDerivative(a, b, c, t);
                    d.y = 0f;
                    transform.localRotation = Quaternion.LookRotation(d);
                    yield return null;
                }

                t -= 1f;
            }

            a = c;
            b = unit.Location.Position;
            c = b;
            for (; t < 1f; t += Time.deltaTime * _messageService.TravelSpeed)
            {
                transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0f;
                transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }

            transform.localPosition = unit.Location.Position;
            unit.Orientation = transform.localRotation.eulerAngles.y;
            ClearPath();
        }
    }
}