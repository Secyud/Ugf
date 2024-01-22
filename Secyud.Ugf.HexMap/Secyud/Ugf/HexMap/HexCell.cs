using System.IO;
using Secyud.Ugf.DataManager;
using Secyud.Ugf.HexMapExtensions;
using Secyud.Ugf.HexUtilities;
using Secyud.Ugf.Unity.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.HexMap
{
    public class HexCell : IArchivable
    {
        private readonly HexCell[] _neighbors = new HexCell[6];

        protected RectTransform UiRect { get; private set; }

        protected HexChunk Chunk { get; private set; }

        public int Index { get; private set; }

        public HexCoordinates Coordinates { get; private set; }

        /// <summary>
        ///     Unit currently occupying the cell, if any.
        /// </summary>
        public HexUnit Unit { get; set; }

        public Vector3 Position { get; protected set; }

        public int X => Index % Chunk.Grid.CellCountX - HexCellExtension.Border;

        public int Z => Index / Chunk.Grid.CellCountX - HexCellExtension.Border;


        private Image _highlight;

        public Image Highlight
        {
            get
            {
                if (!_highlight)
                {
                    if (!UiRect)
                    {
                        return null;
                    }

                    _highlight =
                        UiRect
                            .GetChild(0)
                            .GetComponent<CircleImage>();
                }

                return _highlight;
            }
        }

        private Text _label;
        private Text Label => _label ? _label : _label = UiRect.GetComponent<Text>();


        /// <summary>
        ///     Get one of the neighbor cells.
        /// </summary>
        /// <param name="direction">Neighbor direction relative to the cell.</param>
        /// <returns>Neighbor cell, if it exists.</returns>
        public HexCell GetNeighbor(HexDirection direction)
        {
            return _neighbors[(int)direction];
        }

        /// <summary>
        ///     Set a specific neighbor.
        /// </summary>
        /// <param name="direction">Neighbor direction relative to the cell.</param>
        /// <param name="cell">Neighbor.</param>
        public void SetNeighbor(HexDirection direction, HexCell cell)
        {
            _neighbors[(int)direction] = cell;
            cell._neighbors[(int)direction.Opposite()] = this;
        }

        public void Initialize(RectTransform ui, HexChunk chunk, int x, int z)
        {
            UiRect = ui;
            Chunk = chunk;
            Coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            Index = x + z * Chunk.Grid.CellCountX;
            chunk.Grid.Cells[Index] = this;
        }

        public virtual void CreateMap()
        {
            HexGrid grid = Chunk.Grid;
            HexCell[] cells = grid.Cells;

            int x = Index % grid.CellCountX;
            int z = Index / grid.CellCountX;

            if (x > 0)
            {
                SetNeighbor(HexDirection.W, cells[Index - 1]);
            }

            if (z > 0)
            {
                if ((z & 1) == 0)
                {
                    SetNeighbor(HexDirection.Se, cells[Index - grid.CellCountX]);
                    if (x > 0)
                    {
                        SetNeighbor(HexDirection.SW, cells[Index - grid.CellCountX - 1]);
                    }
                }
                else
                {
                    SetNeighbor(HexDirection.SW, cells[Index - grid.CellCountX]);
                    if (x < grid.CellCountX + -1)
                    {
                        SetNeighbor(HexDirection.Se, cells[Index - grid.CellCountX + 1]);
                    }
                }
            }

            Position = Coordinates.Position3D();
            UiRect.anchoredPosition = Coordinates.Position2D();
        }


        public virtual void Save(BinaryWriter writer)
        {
        }

        public virtual void Load(BinaryReader reader)
        {
        }

        protected void Refresh()
        {
            RefreshSelfOnly();
            foreach (HexCell neighbor in _neighbors)
            {
                if (neighbor != null && neighbor.Chunk != Chunk)
                {
                    neighbor.Chunk.Refresh();
                }
            }
        }

        protected void RefreshSelfOnly()
        {
            Chunk.Refresh();
        }

        public HexDirection DirectionTo(HexCell other)
        {
            return Coordinates.DirectionTo(other.Coordinates);
        }

        public int DistanceTo(HexCell cell)
        {
            return Coordinates.DistanceTo(cell.Coordinates);
        }

        public void SetLabel(string text)
        {
            Label.text = text;
        }

        public void DisableHighlight()
        {
            Image highlight = Highlight;
            if (highlight)
            {
                highlight.enabled = false;
            }
        }

        public void EnableHighlight(Color color)
        {
            Image highlight = Highlight;
            if (highlight)
            {
                highlight.color = color;
                highlight.enabled = true;
            }
        }
    }
}