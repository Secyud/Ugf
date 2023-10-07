#region

using System.Collections;
using Secyud.Ugf.HexMap.Generator;
using Secyud.Ugf.HexMap.Utilities;
using System.Collections.Generic;
using System.IO;
using Secyud.Ugf.Archiving;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.HexMap
{
    /// <summary>
    ///     Component that represents an entire hexagon map.
    /// </summary>
    public class HexGrid : MonoBehaviour, IEnumerable<HexCell>, IEnumerator<HexCell>
    {
        [SerializeField] private HexCell CellPrefab;
        [SerializeField] private Text CellLabelPrefab;
        [SerializeField] private HexGridChunk ChunkPrefab;
        [SerializeField] private int BorderChunkSize;
        public static readonly Color ToCellColor = Color.red;
        public static readonly Color FromCellColor = Color.blue;
        public static readonly Color PathCellColor = Color.white;

        private readonly List<HexUnit> _units = new();
        private HexCell[] _cells;
        private HexCell[] _tmpCells;
        private HexGridChunk[] _chunks;

        private HexCellShaderData _cellShaderData;
        private int _chunkCountX, _chunkCountZ;

        private int _searchFrontierPhase;
        private HexCell _currentPathFrom, _currentPathTo;
        private HexCellPriorityQueue _searchFrontier;
        private int _dx;
        private int _dz;

        public IHexMapManager HexMapManager { get; set; }

        /// <summary>
        ///     Amount of cells in the X dimension.
        /// </summary>
        public int CellCountX { get; private set; }

        /// <summary>
        ///     Amount of cells in the Z dimension.
        /// </summary>
        public int CellCountZ { get; private set; }

        /// <summary>
        ///     Whether there currently exists a path that should be displayed.
        /// </summary>
        public bool HasPath { get; private set; }

        public int BorderWidth => BorderChunkSize;

        private void Awake()
        {
            _cellShaderData = gameObject.AddComponent<HexCellShaderData>();
            _cellShaderData.Grid = this;
        }

        private void OnEnable()
        {
            if (_cellShaderData && _cells is not null)
            {
                _cellShaderData.Initialize(CellCountX + _dx * 2, CellCountZ + _dz * 2);
                foreach (HexCell c in _tmpCells)
                {
                    _cellShaderData.RefreshTerrain(c);
                }
            }
        }


        /// <summary>
        ///     Add a unit to the map.
        /// </summary>
        /// <param name="unitBase"></param>
        /// <param name="unit">Unit to add.</param>
        /// <param name="location">Cell in which to place the unit.</param>
        /// <param name="orientation">Orientation of the unit.</param>
        public void AddUnit(IUnitBase unitBase, HexUnit unit, HexCell location, float orientation)
        {
            _units.Add(unit);
            unit.UnitBase = unitBase;
            unit.Grid = this;
            unit.Location = location;
            unit.Orientation = orientation;
        }

        /// <summary>
        ///     Remove a unit from the map.
        /// </summary>
        /// <param name="unit">The unit to remove.</param>
        public void RemoveUnit(HexUnit unit)
        {
            _units.Remove(unit);
            unit.Die();
        }

        /// <summary>
        ///     Create a new map.
        /// </summary>
        /// <param name="x">X size of the map.</param>
        /// <param name="z">Z size of the map.</param>
        /// <returns>
        ///     Whether the map was successfully created. It fails if the X or Z size
        ///     is not a multiple of the respective chunk size.
        /// </returns>
        public bool CreateMap(int x, int z)
        {
            if (x <= 0 || x % HexMetrics.ChunkSizeX != 0 ||
                z <= 0 || z % HexMetrics.ChunkSizeZ != 0)
            {
                Debug.LogError("Unsupported map size.");
                return false;
            }

            ClearPath();
            ClearUnits();
            if (_chunks != null)
                foreach (HexGridChunk chunk in _chunks)
                    Destroy(chunk.gameObject);

            CellCountX = x;
            CellCountZ = z;
            _chunkCountX = CellCountX / HexMetrics.ChunkSizeX;
            _chunkCountZ = CellCountZ / HexMetrics.ChunkSizeZ;
            _dx = BorderWidth * HexMetrics.ChunkSizeX;
            _dz = BorderWidth * HexMetrics.ChunkSizeZ;
            _cellShaderData.Initialize(CellCountX + _dx * 2, CellCountZ + _dz * 2);
            CreateChunks();
            CreateCells();
            return true;
        }

        private void CreateChunks()
        {
            int chunkCountX = _chunkCountX + 2 * BorderWidth;
            int chunkCountZ = _chunkCountZ + 2 * BorderWidth;

            _chunks = new HexGridChunk[chunkCountX * chunkCountZ];
            for (int z = 0, i = 0; z < chunkCountZ; z++)
            for (int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = _chunks[i++] = Instantiate(ChunkPrefab);
                chunk.transform.SetParent(transform, false);
                chunk.Grid = this;
            }
        }

        private void CreateCells()
        {
            _cells = new HexCell[CellCountZ * CellCountX];

            _tmpCells = new HexCell[(CellCountZ + 2 * _dz) * (CellCountX + 2 * _dx)];

            for (int z = -_dz, i = 0; z < CellCountZ + _dz; z++)
            for (int x = -_dx; x < CellCountX + _dx; x++)
                CreateCell(x, z, i++);
        }

        private void ClearUnits()
        {
            foreach (HexUnit unit in _units) unit.Die();

            _units.Clear();
        }

        /// <summary>
        ///     Get a cell given a <see cref="Ray" />.
        /// </summary>
        /// <param name="ray"><see cref="Ray" /> used to perform a raycast.</param>
        /// <returns>The hit cell, if any.</returns>
        public HexCell GetCell(Ray ray)
        {
            if (Physics.Raycast(ray, out RaycastHit hit)) return GetCell(hit.point);

            return null;
        }

        /// <summary>
        ///     Get the cell that contains a position.
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
        ///     Get the cell with specific <see cref="HexCoordinates" />.
        /// </summary>
        /// <param name="coordinates"><see cref="HexCoordinates" /> of the cell.</param>
        /// <returns>The cell with the given coordinates, if it exists.</returns>
        public HexCell GetCell(HexCoordinates coordinates)
        {
            int index = GetCellIndex(coordinates);
            if (index < 0) return null;

            return _cells[index];
        }

        public int GetCellIndex(HexCoordinates coordinates)
        {
            int z = coordinates.Z;
            if (z < 0 || z >= CellCountZ) return -1;

            int x = coordinates.X + HexCoordinates.Dx(z);
            if (x < 0 || x >= CellCountX) return -1;

            return x + z * CellCountX;
        }

        /// <summary>
        ///     Get the cell with specific offset coordinates.
        /// </summary>
        /// <param name="xOffset">X array offset coordinate.</param>
        /// <param name="zOffset">Z array offset coordinate.</param>
        /// <returns></returns>
        public HexCell GetCell(int xOffset, int zOffset)
        {
            return _cells[xOffset + zOffset * CellCountX];
        }

        /// <summary>
        ///     Get the cell with a specific index.
        /// </summary>
        /// <param name="cellIndex">Cell index, which should be valid.</param>
        /// <returns>The indicated cell.</returns>
        public HexCell GetCell(int cellIndex)
        {
            return _cells[cellIndex];
        }

        /// <summary>
        ///     Control whether the map UI should be visible or hidden.
        /// </summary>
        /// <param name="visible">Whether the UI should be visible.</param>
        public void ShowUI(bool visible)
        {
            foreach (HexGridChunk chunk in _chunks)
                chunk.ShowUI(visible);
        }

        private void CreateCell(int x, int z, int iTmp)
        {
            int tmpIndex = (z + _dz) * (CellCountX + _dx * 2) + x + _dx;

            HexCell cell = _tmpCells[tmpIndex] = Instantiate(CellPrefab);
            cell.Coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.transform.localPosition = cell.Coordinates.Position3D();
            cell.Index = -1;
            cell.TmpIndex = tmpIndex;
            cell.ShaderData = _cellShaderData;

            bool valid = x >= 0 && x < CellCountX && z >= 0 && z < CellCountZ;

            if (valid)
            {
                int i = z * CellCountX + x;
                _cells[i] = cell;
                cell.Index = i;
                HexMapManager.InitMessage(x, z, this).Bind(cell);
            }

            if (x > -_dx)
                cell.SetNeighbor(HexDirection.W, _tmpCells[iTmp - 1]);

            if (z > -_dz)
            {
                if ((z & 1) == 0)
                {
                    cell.SetNeighbor(HexDirection.Se, _tmpCells[iTmp - CellCountX - _dx * 2]);
                    if (x > -_dx)
                        cell.SetNeighbor(HexDirection.SW, _tmpCells[iTmp - CellCountX - _dx * 2 - 1]);
                }
                else
                {
                    cell.SetNeighbor(HexDirection.SW, _tmpCells[iTmp - CellCountX - _dx * 2]);
                    if (x < CellCountX + _dx * 2 - 1)
                        cell.SetNeighbor(HexDirection.Se, _tmpCells[iTmp - CellCountX - _dx * 2 + 1]);
                }
            }


            Text label = Instantiate(CellLabelPrefab);
            label.rectTransform.anchoredPosition = cell.Coordinates.Position2D();
            cell.UIRect = label.rectTransform;

            if (!valid)
            {
                cell.EnableHighlight(new Color(0f, 0f, 0f, 0.5f));
            }

            cell.Elevation = 0;

            AddCellToChunk(x, z, cell);
        }

        private void AddCellToChunk(int x, int z, HexCell cell)
        {
            int chunkX = x % HexMetrics.ChunkSizeX < 0
                ? x / HexMetrics.ChunkSizeX - 1
                : x / HexMetrics.ChunkSizeX;
            int chunkZ = z % HexMetrics.ChunkSizeZ < 0
                ? z / HexMetrics.ChunkSizeZ - 1
                : z / HexMetrics.ChunkSizeZ;
            HexGridChunk chunk = GetChunk(chunkX, chunkZ);
            chunk.AddCell(x, z, cell);
        }

        private HexGridChunk GetChunk(int x, int z)
        {
            return _chunks[x + BorderWidth + (z + BorderWidth) * (_chunkCountX + 2 * BorderWidth)];
        }

        public void SetGenerator(IHexMapGenerator generator)
        {
            generator.TmpCells = _tmpCells;
            generator.CellCountX = CellCountX + _dx * 2;
            generator.CellCountZ = CellCountZ + _dz * 2;
            generator.DeltaX = _dx;
            generator.DeltaZ = _dz;
        }

        /// <summary>
        ///     Save the map.
        /// </summary>
        /// <param name="writer"><see cref="BinaryWriter" /> to use.</param>
        public void Save(IArchiveWriter writer)
        {
            writer.Write(CellCountX);
            writer.Write(CellCountZ);

            foreach (HexCell cell in _tmpCells)
                cell.Save(writer);
        }

        /// <summary>
        ///    
        /// </summary>
        /// <param name="reader"><see cref="BinaryReader" /> to use.</param>
        public void Load(IArchiveReader reader)
        {
            ClearPath();
            ClearUnits();
            int x = reader.ReadInt32();
            int z = reader.ReadInt32();
            if (x != CellCountX || z != CellCountZ)
                if (!CreateMap(x, z))
                    return;

            bool originalImmediateMode = _cellShaderData.ImmediateMode;
            _cellShaderData.ImmediateMode = true;

            foreach (HexCell cell in _tmpCells)
            {
                cell.Load(reader);
                HexCoordinates c = cell.Coordinates;
                cell.TmpIndex = (c.Z + _dz) * (CellCountX + _dx * 2) + c.X + _dx;
            }

            foreach (HexGridChunk chunk in _chunks)
                chunk.Refresh();

            _cellShaderData.ImmediateMode = originalImmediateMode;
        }

        /// <summary>
        ///     Get a list of cells representing the currently visible path.
        /// </summary>
        /// <returns>The current path list, if a visible path exists.</returns>
        public List<HexCell> GetPath()
        {
            if (!HasPath) return null;

            List<HexCell> path = ListPool<HexCell>.Get();
            for (HexCell c = _currentPathTo; c != _currentPathFrom; c = c.PathFrom) path.Add(c);

            path.Add(_currentPathFrom);
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
                HexCell current = _currentPathTo;
                while (current != _currentPathFrom)
                {
                    current.SetLabel(null);
                    current.DisableHighlight();
                    current = current.PathFrom;
                }

                current.DisableHighlight();
                HasPath = false;
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
            if (HasPath)
            {
                HexCell current = _currentPathTo;
                while (current != _currentPathFrom)
                {
                    int turn = (current.Distance - 1) / speed;
                    current.SetLabel(turn.ToString());
                    current.EnableHighlight(PathCellColor);
                    current = current.PathFrom;
                }
            }

            _currentPathFrom.EnableHighlight(FromCellColor);
            _currentPathTo.EnableHighlight(ToCellColor);
        }

        /// <summary>
        ///     Try to find a path.
        /// </summary>
        /// <param name="fromCell">Cell to start the search from.</param>
        /// <param name="toCell">Cell to find a path towards.</param>
        /// <param name="unit">Unit for which the path is.</param>
        public void FindPath(HexCell fromCell, HexCell toCell, HexUnit unit)
        {
            ClearPath();
            _currentPathFrom = fromCell;
            _currentPathTo = toCell;
            HasPath = Search(fromCell, toCell, unit);
            ShowPath(HexMapManager.GetSpeed(unit));
        }

        private bool Search(HexCell fromCell, HexCell toCell, HexUnit unit)
        {
            int speed = HexMapManager.GetSpeed(unit);
            _searchFrontierPhase += 2;
            if (_searchFrontier == null)
                _searchFrontier = new HexCellPriorityQueue();
            else
                _searchFrontier.Clear();

            fromCell.SearchPhase = _searchFrontierPhase;
            fromCell.Distance = 0;
            _searchFrontier.Enqueue(fromCell);
            while (_searchFrontier.Count > 0)
            {
                HexCell current = _searchFrontier.Dequeue();
                current.SearchPhase += 1;

                if (current == toCell) return true;

                int currentTurn = (current.Distance - 1) / speed;

                for (HexDirection d = HexDirection.Ne; d <= HexDirection.Nw; d++)
                {
                    HexCell neighbor = current.GetNeighbor(d);
                    if (neighbor == null ||
                        neighbor.SearchPhase > _searchFrontierPhase)
                        continue;

                    int moveCost = HexMapManager.GetMoveCost(current, neighbor, d);
                    if (moveCost < 0) continue;

                    int distance = current.Distance + moveCost;
                    int turn = (distance - 1) / speed;
                    if (turn > currentTurn) distance = turn * speed + moveCost;

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

        IEnumerator<HexCell> IEnumerable<HexCell>.GetEnumerator()
        {
            return this;
        }

        public IEnumerator GetEnumerator()
        {
            return _cells.GetEnumerator();
        }


        private int _currentIndex;

        public bool MoveNext()
        {
            _currentIndex++;
            return _currentIndex < _cells.Length;
        }

        public void Reset()
        {
            _currentIndex = 0;
        }

        public HexCell Current => _cells[_currentIndex];

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }
    }
}