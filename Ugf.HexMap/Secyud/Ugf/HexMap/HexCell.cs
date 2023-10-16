using System;
using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf.Archiving;
using Secyud.Ugf.BasicComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.HexMap
{
    public class HexCell : MonoBehaviour, IArchivable
    {
        private readonly HexCell[] _neighbors = new HexCell[6];
        private readonly Dictionary<Type, CellProperty> _properties = new();

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

        public TProperty Get<TProperty>()
            where TProperty : CellProperty
        {
            if (!_properties.TryGetValue(typeof(TProperty), out CellProperty property))
            {
                property = U.Get<TProperty>();
                _properties[typeof(TProperty)] = property;
                property.Initialize(this);
            }

            return property as TProperty;
        }

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
        public void Save(IArchiveWriter writer)
        {
            List<CellProperty> properties = _properties.Values
                .Where(u => u.SaveProperty)
                .ToList();
            writer.Write(properties.Count);
            foreach (CellProperty p in properties)
            {
                writer.WriteObject(p);
            }
        }

        /// <summary>
        ///     Load the cell data.
        /// </summary>
        /// <param name="reader"><see cref="IArchiveReader" /> to use.</param>
        public void Load(IArchiveReader reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                CellProperty p = reader.ReadObject<CellProperty>();
                _properties[p.GetType()] = p;
            }
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