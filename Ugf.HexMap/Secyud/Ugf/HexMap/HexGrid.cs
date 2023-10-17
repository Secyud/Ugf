using System;
using System.Collections.Generic;
using Secyud.Ugf.Archiving;
using Secyud.Ugf.HexUtilities;
using UnityEngine;

namespace Secyud.Ugf.HexMap
{
    public class HexGrid : MonoBehaviour, IArchivable
    {
        [SerializeField] private HexChunk ChunkPrefab;
        [SerializeField] private HexCell CellPrefab;
        [SerializeField] private RectTransform UiPrefab;
        [SerializeField] protected Camera Camera;
        [SerializeField] public HexMapCamera MapCamera;
        
        private ShaderData _shaderData;
        private readonly List<HexUnit> _units = new();

        public HexChunk ChunkTemplate => ChunkPrefab;
        public HexCell CellTemplate => CellPrefab;
        public RectTransform UiTemplate => UiPrefab;

        public IHexGridDrawer HexGridDrawer { get; private set; }
        public CellShaderDataManager ShaderManager { get; private set; }
        public HexChunk[] Chunks { get; private set; }
        public HexCell[] Cells { get; private set; }

        public int CellCountX { get; private set; }
        public int CellCountZ { get; private set; }
        public int ChunkCountX { get; private set; }
        public int ChunkCountZ { get; private set; }


        public HexCell GetCellUnderCursor()
        {
            return GetCell(Camera.ScreenPointToRay(Input.mousePosition));
        }

        public virtual void Hide()
        {
            transform.position = new Vector3(65535, 65535, 65535);
            enabled = false;
            MapCamera.enabled = false;
            Camera.enabled = false;
        }

        public virtual void Show()
        {
            transform.position = new Vector3(0, 0, 0);
            enabled = true;
            MapCamera.enabled = true;
            Camera.enabled = true;
        }
        
        
        protected virtual void Awake()
        {
            Cells = Array.Empty<HexCell>();
            ShaderManager = U.Get<CellShaderDataManager>();
            _shaderData = gameObject.GetOrAddComponent<ShaderData>();
            _shaderData.Initialize(this);
        }

        private void OnEnable()
        {
            ShaderManager.ChangeTextureSize(CellCountX, CellCountZ);

            foreach (HexCell c in Cells)
            {
                ShaderManager.CellData[c.Index] = HexGridDrawer.GetCellShaderData(c);
            }

            ShaderManager.ApplyTexture();
        }

        public void Initialize(IHexGridDrawer drawer)
        {
            HexGridDrawer = drawer;
        }

        public void CreateMap(int chunkCountX, int chunkCountZ)
        {
            if (Chunks is not null)
            {
                foreach (HexChunk chunk in Chunks)
                {
                    Destroy(chunk.gameObject);
                }
            }

            ChunkCountX = chunkCountX;
            ChunkCountZ = chunkCountZ;
            CellCountX = chunkCountX * HexMetrics.ChunkSizeX;
            CellCountZ = chunkCountZ * HexMetrics.ChunkSizeZ;

            Chunks = new HexChunk[ChunkCountX * ChunkCountZ];
            Cells = new HexCell[CellCountX * CellCountZ];

            for (int z = 0; z < chunkCountZ; z++)
            for (int x = 0; x < chunkCountX; x++)
            {
                HexChunk chunk = Chunks[z * chunkCountX + x]
                    = Instantiate(ChunkTemplate, transform, false);
                chunk.Initialize(this, x, z);
            }

            foreach (HexChunk chunk in Chunks)
            {
                chunk.CreateMap();
            }

            ShaderManager.ChangeTextureSize(CellCountX, CellCountZ);
        }

        /// <summary>
        ///     Control whether the map UI should be visible or hidden.
        /// </summary>
        /// <param name="visible">Whether the UI should be visible.</param>
        public void ShowUI(bool visible)
        {
            foreach (HexChunk chunk in Chunks)
            {
                chunk.ShowUI(visible);
            }
        }

        public void SetShaderData(HexCell cell)
        {
            ShaderManager.CellData[cell.Index] =
                HexGridDrawer.GetCellShaderData(cell);
            _shaderData.Refresh();
        }

        #region GetMethod

        private HexChunk GetChunk(int xOffset, int zOffset)
        {
            return Chunks[xOffset + zOffset * ChunkCountX];
        }

        /// <summary>
        ///     Get a cell given a <see cref="Ray" />.
        /// </summary>
        /// <param name="ray"><see cref="Ray" /> used to perform a raycast.</param>
        /// <returns>The hit cell, if any.</returns>
        public HexCell GetCell(Ray ray)
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                return GetCell(hit.point);
            }

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

            return Cells[index];
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
            return Cells[xOffset + zOffset * CellCountX];
        }

        /// <summary>
        ///     Get the cell with a specific index.
        /// </summary>
        /// <param name="cellIndex">Cell index, which should be valid.</param>
        /// <returns>The indicated cell.</returns>
        public HexCell GetCell(int cellIndex)
        {
            return Cells[cellIndex];
        }

        #endregion

        #region Unit


        public void AddUnit(HexUnit unit, HexCell location, float orientation)
        {
            _units.Add(unit);
            unit.Location = location;
            unit.Orientation = orientation;
        }

        public void RemoveUnit(HexUnit unit)
        {
            _units.Remove(unit);
            unit.Die();
        }

        private void ClearUnits()
        {
            foreach (HexUnit unit in _units)
            {
                unit.Die();
            }

            _units.Clear();
        }

        #endregion
        
        public void Save(IArchiveWriter writer)
        {
            writer.Write(ChunkCountX);
            writer.Write(ChunkCountZ);

            foreach (HexCell cell in Cells)
            {
                cell.Save(writer);
            }
        }

        public void Load(IArchiveReader reader)
        {
            enabled = false;
            
            int x = reader.ReadInt32();
            int z = reader.ReadInt32();

            if (x != ChunkCountX || z != ChunkCountZ)
            {
                CreateMap(x, z);
            }

            foreach (HexCell cell in Cells)
            {
                cell.Load(reader);
            }

            enabled = true;
            
            foreach (HexChunk chunk in Chunks)
            {
                chunk.Refresh();
            }
        }
    }
}