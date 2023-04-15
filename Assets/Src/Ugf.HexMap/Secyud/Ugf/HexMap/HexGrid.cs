#region

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.HexMap
{
    /// <summary>
    /// Component that represents an entire hexagon map.
    /// </summary>
    public class HexGrid : MonoBehaviour
    {
        [SerializeField] private HexCell CellPrefab;
        [SerializeField] private Text CellLabelPrefab;
        [SerializeField] private HexGridChunk ChunkPrefab;

        public IHexMapManager HexMapManager { get; set; }

        /// <summary>
        /// Amount of cells in the X dimension.
        /// </summary>
        public int CellCountX { get; private set; }

        /// <summary>
        /// Amount of cells in the Z dimension.
        /// </summary>
        public int CellCountZ { get; private set; }

        /// <summary>
        /// Whether there currently exists a path that should be displayed.
        /// </summary>
        public bool HasPath => _currentPathExists;

        /// <summary>
        /// Whether east-west wrapping is enabled.
        /// </summary>
        public bool Wrapping { get; private set; }

        private Transform[] _columns;
        private HexGridChunk[] _chunks;
        private HexCell[] _cells;

        private int _chunkCountX, _chunkCountZ;

        private HexCellPriorityQueue _searchFrontier;

        private int _searchFrontierPhase;

        private HexCell _currentPathFrom, _currentPathTo;
        private bool _currentPathExists;

        private int _currentCenterColumnIndex = -1;

        private readonly List<HexUnit> _units = new();

        private HexCellShaderData _cellShaderData;

        private void Awake()
        {
            _cellShaderData = gameObject.AddComponent<HexCellShaderData>();
            _cellShaderData.Grid = this;
        }

        /// <summary>
        /// Add a unit to the map.
        /// </summary>
        /// <param name="unit">Unit to add.</param>
        /// <param name="location">Cell in which to place the unit.</param>
        /// <param name="orientation">Orientation of the unit.</param>
        public void AddUnit(HexUnit unit, HexCell location, float orientation)
        {
            _units.Add(unit);
            unit.Grid = this;
            unit.Location = location;
            unit.Orientation = orientation;
        }

        /// <summary>
        /// Remove a unit from the map.
        /// </summary>
        /// <param name="unit">The unit to remove.</param>
        public void RemoveUnit(HexUnit unit)
        {
            _units.Remove(unit);
            unit.Die();
        }

        /// <summary>
        /// Make a game object a child of a map column.
        /// </summary>
        /// <param name="child"><see cref="Transform"/> of the child game object.</param>
        /// <param name="columnIndex">Index of the parent column.</param>
        public void MakeChildOfColumn(Transform child, int columnIndex) =>
            child.SetParent(_columns[columnIndex], false);

        /// <summary>
        /// Create a new map.
        /// </summary>
        /// <param name="x">X size of the map.</param>
        /// <param name="z">Z size of the map.</param>
        /// <param name="wrapping">Whether the map wraps east-west.</param>
        /// <returns>Whether the map was successfully created. It fails if the X or Z size
        /// is not a multiple of the respective chunk size.</returns>
        public bool CreateMap(int x, int z, bool wrapping)
        {
            if (x <= 0 || x % HexMetrics.ChunkSizeX != 0 ||
                z <= 0 || z % HexMetrics.ChunkSizeZ != 0)
            {
                Debug.LogError("Unsupported map size.");
                return false;
            }

            ClearPath();
            ClearUnits();
            if (_columns != null)
                foreach (var columns in _columns)
                    Destroy(columns.gameObject);

            CellCountX = x;
            CellCountZ = z;
            Wrapping = wrapping;
            _currentCenterColumnIndex = -1;
            HexMetrics.WrapSize = wrapping ? CellCountX : 0;
            _chunkCountX = CellCountX / HexMetrics.ChunkSizeX;
            _chunkCountZ = CellCountZ / HexMetrics.ChunkSizeZ;
            _cellShaderData.Initialize(CellCountX, CellCountZ);
            CreateChunks();
            CreateCells();
            return true;
        }

        private void CreateChunks()
        {
            _columns = new Transform[_chunkCountX];
            for (int x = 0; x < _chunkCountX; x++)
            {
                _columns[x] = new GameObject("Column").transform;
                _columns[x].SetParent(transform, false);
            }

            _chunks = new HexGridChunk[_chunkCountX * _chunkCountZ];
            for (int z = 0, i = 0; z < _chunkCountZ; z++)
            {
                for (int x = 0; x < _chunkCountX; x++)
                {
                    HexGridChunk chunk = _chunks[i++] = Instantiate(ChunkPrefab);
                    chunk.transform.SetParent(_columns[x], false);
                    chunk.Grid = this;
                }
            }
        }

        private void CreateCells()
        {
            _cells = new HexCell[CellCountZ * CellCountX];

            for (int z = 0, i = 0; z < CellCountZ; z++)
            {
                for (int x = 0; x < CellCountX; x++)
                {
                    CreateCell(x, z, i++);
                }
            }
        }

        private void ClearUnits()
        {
            foreach (var unit in _units)
            {
                unit.Die();
            }

            _units.Clear();
        }

        private void OnEnable()
        {
            if (!HexMetrics.NoiseSource)
            {
                HexMetrics.WrapSize = Wrapping ? CellCountX : 0;
            }
        }

        /// <summary>
        /// Get a cell given a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray"><see cref="Ray"/> used to perform a raycast.</param>
        /// <returns>The hit cell, if any.</returns>
        public HexCell GetCell(Ray ray)
        {
            if (Physics.Raycast(ray, out var hit))
            {
                return GetCell(hit.point);
            }

            return null;
        }

        /// <summary>
        /// Get the cell that contains a position.
        /// </summary>
        /// <param name="position">Position to check.</param>
        /// <returns>The cell containing the position, if it exists.</returns>
        public HexCell GetCell(Vector3 position)
        {
            position = transform.InverseTransformPoint(position);
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);
            return GetCell(coordinates);
        }

        /// <summary>
        /// Get the cell with specific <see cref="HexCoordinates"/>.
        /// </summary>
        /// <param name="coordinates"><see cref="HexCoordinates"/> of the cell.</param>
        /// <returns>The cell with the given coordinates, if it exists.</returns>
        public HexCell GetCell(HexCoordinates coordinates)
        {
            int z = coordinates.Z;
            if (z < 0 || z >= CellCountZ)
            {
                return null;
            }

            int x = coordinates.X + z / 2;
            if (x < 0 || x >= CellCountX)
            {
                return null;
            }

            return _cells[x + z * CellCountX];
        }

        /// <summary>
        /// Get the cell with specific offset coordinates.
        /// </summary>
        /// <param name="xOffset">X array offset coordinate.</param>
        /// <param name="zOffset">Z array offset coordinate.</param>
        /// <returns></returns>
        public HexCell GetCell(int xOffset, int zOffset) =>
            _cells[xOffset + zOffset * CellCountX];

        /// <summary>
        /// Get the cell with a specific index.
        /// </summary>
        /// <param name="cellIndex">Cell index, which should be valid.</param>
        /// <returns>The indicated cell.</returns>
        public HexCell GetCell(int cellIndex) => _cells[cellIndex];

        /// <summary>
        /// Control whether the map UI should be visible or hidden.
        /// </summary>
        /// <param name="visible">Whether the UI should be visible.</param>
        public void ShowUI(bool visible)
        {
            foreach (var chunk in _chunks)
            {
                chunk.ShowUI(visible);
            }
        }

        private void CreateCell(int x, int z, int i)
        {
            Vector3 position;
            // ReSharper disable once PossibleLossOfFraction
            position.x = (x + z * 0.5f - z / 2) * HexMetrics.InnerDiameter;
            position.y = 0f;
            position.z = z * (HexMetrics.OuterRadius * 1.5f);

            HexCell cell = _cells[i] = Instantiate(CellPrefab);
            cell.transform.localPosition = position;
            cell.Coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.Index = i;
            cell.ColumnIndex = x / HexMetrics.ChunkSizeX;
            cell.ShaderData = _cellShaderData;

            if (x > 0)
            {
                cell.SetNeighbor(HexDirection.W, _cells[i - 1]);
                if (Wrapping && x == CellCountX - 1)
                {
                    cell.SetNeighbor(HexDirection.E, _cells[i - x]);
                }
            }

            if (z > 0)
            {
                if ((z & 1) == 0)
                {
                    cell.SetNeighbor(HexDirection.Se, _cells[i - CellCountX]);
                    if (x > 0)
                    {
                        cell.SetNeighbor(HexDirection.SW, _cells[i - CellCountX - 1]);
                    }
                    else if (Wrapping)
                    {
                        cell.SetNeighbor(HexDirection.SW, _cells[i - 1]);
                    }
                }
                else
                {
                    cell.SetNeighbor(HexDirection.SW, _cells[i - CellCountX]);
                    if (x < CellCountX - 1)
                    {
                        cell.SetNeighbor(HexDirection.Se, _cells[i - CellCountX + 1]);
                    }
                    else if (Wrapping)
                    {
                        cell.SetNeighbor(
                            HexDirection.Se, _cells[i - CellCountX * 2 + 1]
                        );
                    }
                }
            }

            Text label = Instantiate(CellLabelPrefab);
            label.rectTransform.anchoredPosition =
                new Vector2(position.x, position.z);
            cell.UIRect = label.rectTransform;

            cell.Elevation = 0;

            AddCellToChunk(x, z, cell);
        }

        private void AddCellToChunk(int x, int z, HexCell cell)
        {
            int chunkX = x / HexMetrics.ChunkSizeX;
            int chunkZ = z / HexMetrics.ChunkSizeZ;
            HexGridChunk chunk = _chunks[chunkX + chunkZ * _chunkCountX];

            int localX = x - chunkX * HexMetrics.ChunkSizeX;
            int localZ = z - chunkZ * HexMetrics.ChunkSizeZ;
            chunk.AddCell(localX + localZ * HexMetrics.ChunkSizeX, cell);
        }

        /// <summary>
        /// Save the map.
        /// </summary>
        /// <param name="writer"><see cref="BinaryWriter"/> to use.</param>
        public void Save(BinaryWriter writer)
        {
            writer.Write(CellCountX);
            writer.Write(CellCountZ);
            writer.Write(Wrapping);

            foreach (var cell in _cells)
                cell.Save(writer);
        }

        /// <summary>
        /// TODO Load the map.
        /// </summary>
        /// <param name="reader"><see cref="BinaryReader"/> to use.</param>
        public void Load(BinaryReader reader)
        {
            ClearPath();
            ClearUnits();
            int x = reader.ReadInt32();
            int z = reader.ReadInt32();
            bool wrapping = reader.ReadBoolean();
            if (x != CellCountX || z != CellCountZ || Wrapping != wrapping)
                if (!CreateMap(x, z, wrapping))
                    return;

            bool originalImmediateMode = _cellShaderData.ImmediateMode;
            _cellShaderData.ImmediateMode = true;

            foreach (var cell in _cells)
                cell.Load(reader);

            foreach (var chunk in _chunks)
                chunk.Refresh();

            _cellShaderData.ImmediateMode = originalImmediateMode;
        }

        /// <summary>
        /// Get a list of cells representing the currently visible path.
        /// </summary>
        /// <returns>The current path list, if a visible path exists.</returns>
        public List<HexCell> GetPath()
        {
            if (!_currentPathExists)
            {
                return null;
            }

            List<HexCell> path = ListPool<HexCell>.Get();
            for (HexCell c = _currentPathTo; c != _currentPathFrom; c = c.PathFrom)
            {
                path.Add(c);
            }

            path.Add(_currentPathFrom);
            path.Reverse();
            return path;
        }

        /// <summary>
        /// Clear the current path.
        /// </summary>
        public void ClearPath()
        {
            if (_currentPathExists)
            {
                HexCell current = _currentPathTo;
                while (current != _currentPathFrom)
                {
                    current.SetLabel(null);
                    current.DisableHighlight();
                    current = current.PathFrom;
                }

                current.DisableHighlight();
                _currentPathExists = false;
            }
            else if (_currentPathFrom)
            {
                _currentPathFrom.DisableHighlight();
                _currentPathTo.DisableHighlight();
            }

            _currentPathFrom = _currentPathTo = null;
        }

        private void ShowPath(int speed)
        {
            if (_currentPathExists)
            {
                HexCell current = _currentPathTo;
                while (current != _currentPathFrom)
                {
                    int turn = (current.Distance - 1) / speed;
                    current.SetLabel(turn.ToString());
                    current.EnableHighlight(Color.white);
                    current = current.PathFrom;
                }
            }

            _currentPathFrom.EnableHighlight(Color.blue);
            _currentPathTo.EnableHighlight(Color.red);
        }

        /// <summary>
        /// Try to find a path.
        /// </summary>
        /// <param name="fromCell">Cell to start the search from.</param>
        /// <param name="toCell">Cell to find a path towards.</param>
        /// <param name="unit">Unit for which the path is.</param>
        public void FindPath(HexCell fromCell, HexCell toCell, HexUnit unit)
        {
            ClearPath();
            _currentPathFrom = fromCell;
            _currentPathTo = toCell;
            _currentPathExists = Search(fromCell, toCell, unit);
            ShowPath(unit.Speed);
        }

        private bool Search(HexCell fromCell, HexCell toCell, HexUnit unit)
        {
            int speed = unit.Speed;
            _searchFrontierPhase += 2;
            if (_searchFrontier == null)
            {
                _searchFrontier = new HexCellPriorityQueue();
            }
            else
            {
                _searchFrontier.Clear();
            }

            fromCell.SearchPhase = _searchFrontierPhase;
            fromCell.Distance = 0;
            _searchFrontier.Enqueue(fromCell);
            while (_searchFrontier.Count > 0)
            {
                HexCell current = _searchFrontier.Dequeue();
                current.SearchPhase += 1;

                if (current == toCell)
                {
                    return true;
                }

                int currentTurn = (current.Distance - 1) / speed;

                for (HexDirection d = HexDirection.Ne; d <= HexDirection.Nw; d++)
                {
                    HexCell neighbor = current.GetNeighbor(d);
                    if (neighbor == null ||
                        neighbor.SearchPhase > _searchFrontierPhase)
                    {
                        continue;
                    }

                    int moveCost = HexMapManager.GetMoveCost(unit, current, neighbor, d);
                    if (moveCost < 0)
                    {
                        continue;
                    }

                    int distance = current.Distance + moveCost;
                    int turn = (distance - 1) / speed;
                    if (turn > currentTurn)
                    {
                        distance = turn * speed + moveCost;
                    }

                    if (neighbor.SearchPhase < _searchFrontierPhase)
                    {
                        neighbor.SearchPhase = _searchFrontierPhase;
                        neighbor.Distance = distance;
                        neighbor.PathFrom = current;
                        neighbor.SearchHeuristic =
                            neighbor.Coordinates.DistanceTo(toCell.Coordinates);
                        _searchFrontier.Enqueue(neighbor);
                    }
                    else if (distance < neighbor.Distance)
                    {
                        int oldPriority = neighbor.SearchPriority;
                        neighbor.Distance = distance;
                        neighbor.PathFrom = current;
                        _searchFrontier.Change(neighbor, oldPriority);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Center the map given an X position, to facilitate east-west wrapping.
        /// </summary>
        /// <param name="xPosition">X position.</param>
        public void CenterMap(float xPosition)
        {
            int centerColumnIndex = (int)
                (xPosition / (HexMetrics.InnerDiameter * HexMetrics.ChunkSizeX));

            if (centerColumnIndex == _currentCenterColumnIndex)
            {
                return;
            }

            _currentCenterColumnIndex = centerColumnIndex;

            int minColumnIndex = centerColumnIndex - _chunkCountX / 2;
            int maxColumnIndex = centerColumnIndex + _chunkCountX / 2;

            Vector3 position;
            position.y = position.z = 0f;
            for (int i = 0; i < _columns.Length; i++)
            {
                if (i < minColumnIndex)
                {
                    position.x = _chunkCountX *
                                 (HexMetrics.InnerDiameter * HexMetrics.ChunkSizeX);
                }
                else if (i > maxColumnIndex)
                {
                    position.x = _chunkCountX *
                                 -(HexMetrics.InnerDiameter * HexMetrics.ChunkSizeX);
                }
                else
                {
                    position.x = 0f;
                }

                _columns[i].localPosition = position;
            }
        }
    }
}