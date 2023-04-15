#region

using System.IO;
using System.Linq;
using Secyud.Ugf.Archiving;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.HexMap
{
    /// <summary>
    /// Container component for hex cell data.
    /// </summary>
    public class HexCell : MonoBehaviour,IHexCell, IArchivable
    {
        /// <summary>
        /// Hexagonal coordinates unique to the cell.
        /// </summary>
        public HexCoordinates Coordinates { get; set; }

        /// <summary>
        /// Transform component for the cell's UI visualization. 
        /// </summary>
        public RectTransform UIRect { get; set; }

        /// <summary>
        /// Grid chunk that contains the cell.
        /// </summary>
        public HexGridChunk Chunk { get; set; }


        /// <summary>
        /// Unique global index of the cell.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Map column index of the cell.
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <summary>
        /// Surface elevation level.
        /// </summary>
        public int Elevation
        {
            get => _elevation;
            set
            {
                if (_elevation == value)
                {
                    return;
                }

                // int originalViewElevation = ViewElevation;
                _elevation = value;
                ShaderData.ViewElevationChanged(this);
                RefreshPosition();
                ValidateRivers();

                for (int i = 0; i < Roads.Length; i++)
                {
                    if (Roads[i] && GetElevationDifference((HexDirection)i) > 1)
                    {
                        SetRoad(i, false);
                    }
                }

                Refresh();
            }
        }

        /// <summary>
        /// Water elevation level.
        /// </summary>
        public int WaterLevel
        {
            get => _waterLevel;
            set
            {
                if (_waterLevel == value)
                {
                    return;
                }

                // int originalViewElevation = ViewElevation;
                _waterLevel = value;
                ShaderData.ViewElevationChanged(this);
                ValidateRivers();
                Refresh();
            }
        }

        /// <summary>
        /// Elevation at which the cell is visible. Highest of surface and water level.
        /// </summary>
        public int ViewElevation => _elevation >= _waterLevel ? _elevation : _waterLevel;

        /// <summary>
        /// Whether the cell counts as underwater, which is when water is higher than surface.
        /// </summary>
        public bool IsUnderwater => _waterLevel > _elevation;

        /// <summary>
        /// Whether there is an incoming river.
        /// </summary>
        public bool HasIncomingRiver => _hasIncomingRiver;

        /// <summary>
        /// Whether there is an outgoing river.
        /// </summary>
        public bool HasOutgoingRiver => _hasOutgoingRiver;

        /// <summary>
        /// Whether there is a river, either incoming, outgoing, or both.
        /// </summary>
        public bool HasRiver => _hasIncomingRiver || _hasOutgoingRiver;

        /// <summary>
        /// Whether a river begins or ends in the cell.
        /// </summary>
        public bool HasRiverBeginOrEnd => _hasIncomingRiver != _hasOutgoingRiver;

        /// <summary>
        /// The direction of the incoming or outgoing river, if applicable.
        /// </summary>
        public HexDirection RiverBeginOrEndDirection =>
            _hasIncomingRiver ? _incomingRiver : _outgoingRiver;

        /// <summary>
        /// Whether the cell contains roads.
        /// </summary>
        public bool HasRoads => Roads.Any(t => t);

        /// <summary>
        /// Incoming river direction, if applicable.
        /// </summary>
        public HexDirection IncomingRiver => _incomingRiver;

        /// <summary>
        /// Outgoing river direction, if applicable.
        /// </summary>
        public HexDirection OutgoingRiver => _outgoingRiver;

        /// <summary>
        /// Local position of this cell's game object.
        /// </summary>
        public Vector3 Position => transform.localPosition;

        /// <summary>
        /// Vertical positions the the stream bed, if applicable.
        /// </summary>
        public float StreamBedY =>
            (_elevation + HexMetrics.StreamBedElevationOffset) * HexMetrics.ElevationStep;

        /// <summary>
        /// Vertical position of the river's surface, if applicable.
        /// </summary>
        public float RiverSurfaceY =>
            (_elevation + HexMetrics.WaterElevationOffset) * HexMetrics.ElevationStep;

        /// <summary>
        /// Vertical position of the water surface, if applicable.
        /// </summary>
        public float WaterSurfaceY =>
            (_waterLevel + HexMetrics.WaterElevationOffset) * HexMetrics.ElevationStep;

        /// <summary>
        /// Whether the cell is considered inside a walled region.
        /// </summary>
        public bool Walled
        {
            get => _walled;
            set
            {
                if (_walled != value)
                {
                    _walled = value;
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Terrain type index.
        /// </summary>
        public int TerrainTypeIndex
        {
            get => _terrainTypeIndex;
            set
            {
                if (_terrainTypeIndex != value)
                {
                    _terrainTypeIndex = value;
                    ShaderData.RefreshTerrain(this);
                }
            }
        }

        /// <summary>
        /// Distance data used by pathfinding algorithm.
        /// </summary>
        public int Distance
        {
            get => _distance;
            set => _distance = value;
        }

        /// <summary>
        /// Unit currently occupying the cell, if any.
        /// </summary>
        public HexUnit Unit { get; set; }

        /// <summary>
        /// Pathing data used by pathfinding algorithm.
        /// </summary>
        public HexCell PathFrom { get; set; }

        /// <summary>
        /// Heuristic data used by pathfinding algorithm.
        /// </summary>
        public int SearchHeuristic { get; set; }

        /// <summary>
        /// Search priority used by pathfinding algorithm.
        /// </summary>
        public int SearchPriority => _distance + SearchHeuristic;

        /// <summary>
        /// Search phases data used by pathfinding algorithm.
        /// </summary>
        public int SearchPhase { get; set; }

        /// <summary>
        /// Linked list reference used by <see cref="HexCellPriorityQueue"/> for pathfinding.
        /// </summary>
        public HexCell NextWithSamePriority { get; set; }

        /// <summary>
        /// Reference to <see cref="HexCellShaderData"/> that contains the cell.
        /// </summary>
        public HexCellShaderData ShaderData { get; set; }

        public bool IsSpecial
        {
            get => _isSpecial;
            set => _isSpecial = value;
        }

        private int _terrainTypeIndex;

        private int _elevation = int.MinValue;
        private int _waterLevel;

        private int _distance;

        private bool _walled;
        private bool _hasIncomingRiver, _hasOutgoingRiver;
        private bool _isSpecial;

        private HexDirection _incomingRiver, _outgoingRiver;


        [SerializeField] private HexCell[] Neighbors;
        [SerializeField] private bool[] Roads;

        /// <summary>
        /// Get one of the neighbor cells.
        /// </summary>
        /// <param name="direction">Neighbor direction relative to the cell.</param>
        /// <returns>Neighbor cell, if it exists.</returns>
        public HexCell GetNeighbor(HexDirection direction) => Neighbors[(int)direction];

        /// <summary>
        /// Set a specific neighbor.
        /// </summary>
        /// <param name="direction">Neighbor direction relative to the cell.</param>
        /// <param name="cell">Neighbor.</param>
        public void SetNeighbor(HexDirection direction, HexCell cell)
        {
            Neighbors[(int)direction] = cell;
            cell.Neighbors[(int)direction.Opposite()] = this;
        }

        /// <summary>
        /// Get the <see cref="HexEdgeType"/> of a cell edge.
        /// </summary>
        /// <param name="direction">Edge direction relative to the cell.</param>
        /// <returns><see cref="HexEdgeType"/> based on the neighboring cells.</returns>
        public HexEdgeType GetEdgeType(HexDirection direction) => HexMetrics.GetEdgeType(
            _elevation, Neighbors[(int)direction]._elevation
        );

        /// <summary>
        /// Get the <see cref="HexEdgeType"/> based on this and another cell.
        /// </summary>
        /// <param name="otherCell">Other cell to consider as neighbor.</param>
        /// <returns><see cref="HexEdgeType"/> based on this and the other cell.</returns>
        public HexEdgeType GetEdgeType(HexCell otherCell) => HexMetrics.GetEdgeType(
            _elevation, otherCell._elevation
        );

        /// <summary>
        /// Whether a river goes through a specific cell edge.
        /// </summary>
        /// <param name="direction">Edge direction relative to the cell.</param>
        /// <returns></returns>
        public bool HasRiverThroughEdge(HexDirection direction) =>
            _hasIncomingRiver && _incomingRiver == direction ||
            _hasOutgoingRiver && _outgoingRiver == direction;

        /// <summary>
        /// Remove the incoming river, if it exists.
        /// </summary>
        public void RemoveIncomingRiver()
        {
            if (!_hasIncomingRiver)
            {
                return;
            }

            _hasIncomingRiver = false;
            RefreshSelfOnly();

            HexCell neighbor = GetNeighbor(_incomingRiver);
            neighbor._hasOutgoingRiver = false;
            neighbor.RefreshSelfOnly();
        }

        /// <summary>
        /// Remove the outgoing river, if it exists.
        /// </summary>
        public void RemoveOutgoingRiver()
        {
            if (!_hasOutgoingRiver)
            {
                return;
            }

            _hasOutgoingRiver = false;
            RefreshSelfOnly();

            HexCell neighbor = GetNeighbor(_outgoingRiver);
            neighbor._hasIncomingRiver = false;
            neighbor.RefreshSelfOnly();
        }

        /// <summary>
        /// Remove both incoming and outgoing rivers, if they exist.
        /// </summary>
        public void RemoveRiver()
        {
            RemoveOutgoingRiver();
            RemoveIncomingRiver();
        }

        /// <summary>
        /// Define an outgoing river.
        /// </summary>
        /// <param name="direction">Direction of the river.</param>
        public void SetOutgoingRiver(HexDirection direction)
        {
            if (_hasOutgoingRiver && _outgoingRiver == direction)
            {
                return;
            }

            HexCell neighbor = GetNeighbor(direction);
            if (!IsValidRiverDestination(neighbor))
            {
                return;
            }

            RemoveOutgoingRiver();
            if (_hasIncomingRiver && _incomingRiver == direction)
            {
                RemoveIncomingRiver();
            }

            _hasOutgoingRiver = true;
            _outgoingRiver = direction;

            neighbor.RemoveIncomingRiver();
            neighbor._hasIncomingRiver = true;
            neighbor._incomingRiver = direction.Opposite();

            SetRoad((int)direction, false);
        }

        /// <summary>
        /// Whether a road goes through a specific cell edge.
        /// </summary>
        /// <param name="direction">Edge direction relative to cell.</param>
        /// <returns></returns>
        public bool HasRoadThroughEdge(HexDirection direction) => Roads[(int)direction];

        /// <summary>
        /// Define a road that goes in a specific direction.
        /// </summary>
        /// <param name="direction">Direction relative to cell.</param>
        public void AddRoad(HexDirection direction)
        {
            if (!Roads[(int)direction] && !HasRiverThroughEdge(direction) &&
                !_isSpecial && !GetNeighbor(direction)._isSpecial &&
                GetElevationDifference(direction) <= 1)
            {
                SetRoad((int)direction, true);
            }
        }

        /// <summary>
        /// Remove all roads from the cell.
        /// </summary>
        public void RemoveRoads()
        {
            for (int i = 0; i < Neighbors.Length; i++)
            {
                if (Roads[i])
                {
                    SetRoad(i, false);
                }
            }
        }

        /// <summary>
        /// Get the elevation difference with a neighbor. The indicated neighbor must exist.
        /// </summary>
        /// <param name="direction">Direction to the neighbor, relative to the cell.</param>
        /// <returns>Absolute elevation difference.</returns>
        public int GetElevationDifference(HexDirection direction)
        {
            int difference = _elevation - GetNeighbor(direction)._elevation;
            return difference >= 0 ? difference : -difference;
        }

        private bool IsValidRiverDestination(HexCell neighbor) =>
            neighbor && (_elevation >= neighbor._elevation || _waterLevel == neighbor._elevation);

        private void ValidateRivers()
        {
            if (
                _hasOutgoingRiver &&
                !IsValidRiverDestination(GetNeighbor(_outgoingRiver))
            )
            {
                RemoveOutgoingRiver();
            }

            if (
                _hasIncomingRiver &&
                !GetNeighbor(_incomingRiver).IsValidRiverDestination(this)
            )
            {
                RemoveIncomingRiver();
            }
        }

        private void SetRoad(int index, bool state)
        {
            Roads[index] = state;
            Neighbors[index].Roads[(int)((HexDirection)index).Opposite()] = state;
            Neighbors[index].RefreshSelfOnly();
            RefreshSelfOnly();
        }

        private void RefreshPosition()
        {
            Vector3 position = transform.localPosition;
            position.y = _elevation * HexMetrics.ElevationStep;
            position.y +=
                (HexMetrics.SampleNoise(position).y * 2f - 1f) *
                HexMetrics.ElevationPerturbStrength;
            transform.localPosition = position;

            Vector3 uiPosition = UIRect.localPosition;
            uiPosition.z = -position.y;
            UIRect.localPosition = uiPosition;
        }

        private void Refresh()
        {
            if (Chunk)
            {
                Chunk.Refresh();
                foreach (HexCell neighbor in Neighbors)
                {
                    if (neighbor != null && neighbor.Chunk != Chunk)
                    {
                        neighbor.Chunk.Refresh();
                    }
                }

                if (Unit)
                {
                    Unit.ValidateLocation();
                }
            }
        }

        private void RefreshSelfOnly()
        {
            Chunk.Refresh();
            if (Unit)
            {
                Unit.ValidateLocation();
            }
        }

        /// <summary>
        /// Save the cell data.
        /// </summary>
        /// <param name="writer"><see cref="BinaryWriter"/> to use.</param>
        public void Save(BinaryWriter writer)
        {
            writer.Write((byte)_terrainTypeIndex);
            writer.Write((byte)(_elevation + 127));
            writer.Write((byte)_waterLevel);


            writer.Write(_isSpecial);
            writer.Write(_walled);

            if (_hasIncomingRiver)
                writer.Write((byte)(_incomingRiver + 128));
            else
                writer.Write((byte)0);

            if (_hasOutgoingRiver)
                writer.Write((byte)(_outgoingRiver + 128));
            else
                writer.Write((byte)0);

            int roadFlags = 0;
            for (int i = 0; i < Roads.Length; i++)
                if (Roads[i])
                    roadFlags |= 1 << i;

            writer.Write((byte)roadFlags);
        }

        /// <summary>
        /// Load the cell data.
        /// </summary>
        /// <param name="reader"><see cref="BinaryReader"/> to use.</param>
        public void Load(BinaryReader reader)
        {
            _terrainTypeIndex = reader.ReadByte();
            _elevation = reader.ReadByte() - 127;

            RefreshPosition();
            _waterLevel = reader.ReadByte();

            _isSpecial = reader.ReadBoolean();
            _walled = reader.ReadBoolean();

            byte riverData = reader.ReadByte();
            if (riverData >= 128)
            {
                _hasIncomingRiver = true;
                _incomingRiver = (HexDirection)(riverData - 128);
            }
            else
                _hasIncomingRiver = false;

            riverData = reader.ReadByte();
            if (riverData >= 128)
            {
                _hasOutgoingRiver = true;
                _outgoingRiver = (HexDirection)(riverData - 128);
            }
            else
                _hasOutgoingRiver = false;

            int roadFlags = reader.ReadByte();
            for (int i = 0; i < Roads.Length; i++)
                Roads[i] = (roadFlags & (1 << i)) != 0;

            ShaderData.RefreshTerrain(this);
        }

        /// <summary>
        /// Set the cell's UI label.
        /// </summary>
        /// <param name="text">Label text.</param>
        public void SetLabel(string text)
        {
            Text label = UIRect.GetComponent<Text>();
            label.text = text;
        }

        /// <summary>
        /// Disable the cell's highlight.
        /// </summary>
        public void DisableHighlight()
        {
            if (!_highlight)
                _highlight = UIRect.GetChild(0).GetComponent<Image>();
            _highlight.enabled = false;
        }

        private Image _highlight;

        /// <summary>
        /// Enable the cell's highlight. 
        /// </summary>
        /// <param name="color">Highlight color.</param>
        public void EnableHighlight(Color color)
        {
            if (!_highlight)
                _highlight = UIRect.GetChild(0).GetComponent<Image>();
            _highlight.color = color;
            _highlight.enabled = true;
        }

        /// <summary>
        /// Set arbitrary map data for this cell's <see cref="ShaderData"/>.
        /// </summary>
        /// <param name="data">Data value, 0-1 inclusive.</param>
        public void SetMapData(float data) => ShaderData.SetMapData(this, data);
    }
}