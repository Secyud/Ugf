using System;
using System.Collections;
using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.HexMap;
using UnityEngine;

namespace Secyud.Ugf.UgfHexMap
{
    public interface IUgfHexMapFunction
    {
        UgfCell CurrentPathFrom { get; }
        UgfCell CurrentPathTo { get; }
        bool HasPath { get; }
        void Travel();
    }

    public abstract class UgfHexMapFunctionService<TMessageService> :
        IUgfHexMapFunction,IRegistry
        where TMessageService : IHexMapMessageService
    {
        private int _searchFrontierPhase;
        private UgfCellPriorityQueue _searchFrontier;
        private TMessageService _messageService;
        public bool HasPath { get; private set; }

        public UgfCell CurrentPathFrom { get; private set; }
        public UgfCell CurrentPathTo { get; private set; }

        protected UgfHexMapFunctionService(TMessageService service)
        {
            _messageService = service;
        }

        /// <summary>
        ///     Get a list of cells representing the currently visible path.
        /// </summary>
        /// <returns>The current path list, if a visible path exists.</returns>
        public IList<UgfCell> GetPath()
        {
            if (!HasPath)
            {
                return Array.Empty<UgfCell>();
            }

            List<UgfCell> path = new();
            for (UgfCell c = CurrentPathTo; c != CurrentPathFrom; c = c.PathFrom)
            {
                path.Add(c);
            }

            path.Add(CurrentPathFrom);
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
                UgfCell current = CurrentPathTo;
                while (current != CurrentPathFrom)
                {
                    current.Cell.SetLabel(null);
                    current = current.PathFrom;
                }

                HasPath = false;
            }
            else if (CurrentPathFrom is not null)
            {
                CurrentPathFrom.Cell.DisableHighlight();
                CurrentPathTo.Cell.DisableHighlight();
            }

            CurrentPathFrom = CurrentPathTo = null;
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
            CurrentPathFrom = fromCell;
            CurrentPathTo = toCell;
            HasPath = Search(fromCell, toCell, unit);
            _messageService.ShowPath(this, unit);
        }

        private bool Search(UgfCell fromCell, UgfCell toCell, HexUnit unit)
        {
            float speed = _messageService.GetSpeed(unit);
            _searchFrontierPhase += 2;
            if (_searchFrontier == null)
                _searchFrontier = new UgfCellPriorityQueue();
            else
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
                    if (neighbor == null ||
                        neighbor.SearchPhase > _searchFrontierPhase)
                        continue;

                    float moveCost = _messageService.GetMoveCost(
                        current, neighbor, d);
                    if (moveCost < 0) continue;

                    float distance = current.Distance + moveCost;
                    float turn = (distance - 1) / speed;
                    if (turn > currentTurn)
                        distance = turn * speed + moveCost;

                    if (neighbor.SearchPhase < _searchFrontierPhase)
                    {
                        neighbor.SearchPhase = _searchFrontierPhase;
                        neighbor.Distance = (int)distance;
                        neighbor.PathFrom = current;
                        neighbor.SearchHeuristic =
                            neighbor.Cell.Coordinates.DistanceTo(toCell.Cell.Coordinates);
                        _searchFrontier.Enqueue(neighbor);
                    }
                    else if (distance < neighbor.Distance)
                    {
                        int oldPriority = neighbor.SearchPriority;
                        neighbor.Distance = (int)distance;
                        neighbor.PathFrom = current;
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
                IList<UgfCell> path = GetPath();
                UgfCell location = path[0];
                HexUnit unit = location.Cell.Unit;
                if (unit)
                {
                    location.Cell.Unit = null;
                    location = path[^1];
                    location.Cell.Unit = unit;
                    unit.StopAllCoroutines();
                    unit.StartCoroutine(TravelPath(path,unit));
                    ClearPath();
                }
            }
        }
        
        private IEnumerator TravelPath(IList<UgfCell> path,HexUnit unit)
        {
            Vector3 a, b, c = path[0].Position;
            Transform transform = unit.transform;

            float t = Time.deltaTime * _messageService.TravelSpeed;
            for (int i = 1; i < path.Count; i++)
            {
                UgfCell currentTravelLocation = path[i];
                a = c;
                b = path[i - 1].Position;

                c = (b + currentTravelLocation.Position) * 0.5f;

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
        }

    }
}