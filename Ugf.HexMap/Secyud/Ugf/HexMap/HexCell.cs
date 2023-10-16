using System;
using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf.Archiving;
using Secyud.Ugf.BasicComponents;
using Secyud.Ugf.HexUtilities;
using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.HexMap
{
    public class HexCell : MonoBehaviour, IArchivable
    {
        private readonly HexCell[] _neighbors = new HexCell[6];
        public RectTransform UiRect { get; private set; }
        public HexChunk Chunk { get; private set; }
        public int Index { get; private set; }
        public HexCoordinates Coordinates { get; private set; }

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
                            .GetComponent<SImage>();
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

        public void CreateMap()
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

            UiRect.anchoredPosition = Coordinates.Position2D();
        }


        /// <summary>
        ///     Save the cell data.
        /// </summary>
        /// <param name="writer"><see cref="IArchiveWriter" /> to use.</param>
        public virtual void Save(IArchiveWriter writer)
        {
        }

        /// <summary>
        ///     Load the cell data.
        /// </summary>
        /// <param name="reader"><see cref="IArchiveReader" /> to use.</param>
        public virtual void Load(IArchiveReader reader)
        {
        }

        public void Refresh()
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

        public void RefreshSelfOnly()
        {
            Chunk.Refresh();
        }

        public HexDirection DirectionTo(HexCell other)
        {
            return Coordinates.DirectionFrom(other.Coordinates);
        }

        public float DistanceTo(HexCell cell)
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

        /// <summary>
        ///     Unit currently occupying the cell, if any.
        /// </summary>
        public HexUnit Unit { get; set; }
        
        
        public Vector3 Position => transform.localPosition;
    }
}