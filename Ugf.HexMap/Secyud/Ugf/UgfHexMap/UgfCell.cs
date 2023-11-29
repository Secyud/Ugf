using System.Linq;
using Secyud.Ugf.Archiving;
using Secyud.Ugf.HexMap;
using Secyud.Ugf.HexUtilities;
using UnityEngine;

namespace Secyud.Ugf.UgfHexMap
{
    public class UgfCell : HexCell
    {
        private byte _terrainType;
        private bool _walled;
        public HexDirection IncomingRiver { get; private set; } = HexDirection.InValid;
        public HexDirection OutgoingRiver { get; private set; }= HexDirection.InValid;

        private short _elevation = short.MinValue;
        private short _waterLevel;

        public Transform FeaturePrefab { get; set; }

        public byte TerrainType
        {
            get => _terrainType;
            set
            {
                if (_terrainType != value)
                {
                    _terrainType = value;
                    Chunk.Grid.SetShaderData(this);
                }
            }
        }

        #region River

        public bool HasRiver =>
            HasIncomingRiver || HasOutgoingRiver;

        public bool HasIncomingRiver =>
            IncomingRiver != HexDirection.InValid;

        public bool HasOutgoingRiver =>
            OutgoingRiver != HexDirection.InValid;

        public HexDirection RiverBeginOrEndDirection =>
            HasIncomingRiver ? IncomingRiver : OutgoingRiver;

        public bool HasRiverBeginOrEnd =>
            HasIncomingRiver != HasOutgoingRiver;

        public bool HasRiverThroughEdge(HexDirection direction)
        {
            return IncomingRiver == direction ||
                   OutgoingRiver == direction;
        }

        /// <summary>
        ///     Remove the incoming river, if it exists.
        /// </summary>
        public void RemoveIncomingRiver()
        {
            if (!HasIncomingRiver)
            {
                return;
            }

            RefreshSelfOnly();

            UgfCell neighbor = GetNeighbor(IncomingRiver);
            if (neighbor is not null)
            {
                neighbor.OutgoingRiver = HexDirection.InValid;
                neighbor.RefreshSelfOnly();
            }

            IncomingRiver = HexDirection.InValid;
        }

        /// <summary>
        ///     Remove the outgoing river, if it exists.
        /// </summary>
        public void RemoveOutgoingRiver()
        {
            if (!HasOutgoingRiver) return;

            RefreshSelfOnly();

            UgfCell neighbor = GetNeighbor(OutgoingRiver);

            if (neighbor  is not null)
            {
                neighbor.IncomingRiver = HexDirection.InValid;
                neighbor.RefreshSelfOnly();
            }

            OutgoingRiver = HexDirection.InValid;
        }

        /// <summary>
        ///     Remove both incoming and outgoing rivers, if they exist.
        /// </summary>
        public void RemoveRiver()
        {
            RemoveOutgoingRiver();
            RemoveIncomingRiver();
        }

        /// <summary>
        ///     Define an outgoing river.
        /// </summary>
        /// <param name="direction">Direction of the river.</param>
        public void SetOutgoingRiver(HexDirection direction)
        {
            if (HasOutgoingRiver && OutgoingRiver == direction) return;

            UgfCell neighbor = GetNeighbor(direction);
            if (!IsValidRiverDestination(neighbor)) return;

            RemoveOutgoingRiver();
            if (HasIncomingRiver && IncomingRiver == direction) RemoveIncomingRiver();

            OutgoingRiver = direction;

            neighbor.RemoveIncomingRiver();
            neighbor.IncomingRiver = direction.Opposite();

            SetRoad(direction, false);
        }

        private void ValidateRivers()
        {
            if (HasOutgoingRiver)
            {
                UgfCell neighbor = GetNeighbor(OutgoingRiver);
                if (neighbor  is not null&& !IsValidRiverDestination(neighbor))
                {
                    RemoveOutgoingRiver();
                }
            }

            if (HasIncomingRiver)
            {
                UgfCell neighbor = GetNeighbor(IncomingRiver);
                if (neighbor  is not null&& !neighbor.IsValidRiverDestination(this))
                {
                    RemoveIncomingRiver();
                }
            }
        }

        #endregion

        #region Road

        public bool[] Roads { get; } = new bool[6];
        public bool HasRoads => Roads.Any(t => t);

        /// <summary>
        ///     Whether a road goes through a specific cell edge.
        /// </summary>
        /// <param name="direction">Edge direction relative to cell.</param>
        /// <returns></returns>
        public bool HasRoadThroughEdge(HexDirection direction)
        {
            return Roads[(int)direction];
        }

        /// <summary>
        ///     Define a road that goes in a specific direction.
        /// </summary>
        /// <param name="direction">Direction relative to cell.</param>
        public void AddRoad(HexDirection direction)
        {
            if (!Roads[(int)direction] && !HasRiverThroughEdge(direction) &&
                GetElevationDifference(direction) <= 1)
            {
                SetRoad(direction, true);
            }
        }

        /// <summary>
        ///     Remove all roads from the cell.
        /// </summary>
        public void RemoveRoads()
        {
            for (int i = 0; i < 6; i++)
            {
                if (Roads[i])
                {
                    SetRoad((HexDirection)i, false);
                }
            }
        }

        private void SetRoad(HexDirection index, bool state)
        {
            Roads[(int)index] = state;
            UgfCell neighbor = GetNeighbor(index);
            neighbor.Roads[(int)index.Opposite()] = state;
            neighbor.RefreshSelfOnly();
            RefreshSelfOnly();
        }

        #endregion

        #region Elevation

        public short Elevation
        {
            get => _elevation;
            set
            {
                if (_elevation == value) return;

                _elevation = value;
                RefreshPosition();
                ValidateRivers();

                for (int i = 0; i < 6; i++)
                {
                    if (Roads[i] && GetElevationDifference((HexDirection)i) > 1)
                    {
                        SetRoad((HexDirection)i, false);
                    }
                }

                Refresh();
            }
        }

        public float StreamBedY =>
            (Elevation + HexMetrics.StreamBedElevationOffset)
            * HexMetrics.ElevationStep;

        public float RiverSurfaceY =>
            (Elevation + HexMetrics.WaterElevationOffset)
            * HexMetrics.ElevationStep;

        public int GetElevationDifference(HexDirection direction)
        {
            int difference = _elevation - GetNeighbor(direction)._elevation;
            return difference >= 0 ? difference : -difference;
        }

        public HexEdgeType GetEdgeType(HexDirection direction)
        {
            return GetEdgeType(GetNeighbor(direction));
        }

        public HexEdgeType GetEdgeType(UgfCell otherCell)
        {
            return HexMetrics.GetEdgeType(
                _elevation, otherCell._elevation
            );
        }

        #endregion

        #region Water

        public short WaterLevel
        {
            get => _waterLevel;
            set
            {
                if (_waterLevel == value) return;

                // int originalViewElevation = ViewElevation;
                _waterLevel = value;
                ValidateRivers();
                Refresh();
            }
        }

        public int ViewElevation => Elevation >= WaterLevel ? Elevation : WaterLevel;
        public float WaterSurfaceY => (WaterLevel + HexMetrics.WaterElevationOffset) * HexMetrics.ElevationStep;
        public bool IsUnderwater => _waterLevel > _elevation;

        #endregion

        private bool IsValidRiverDestination(UgfCell neighbor)
        {
            return neighbor  is not null&&
                   (_elevation >= neighbor._elevation ||
                    _waterLevel == neighbor._elevation);
        }

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

        #region Position

        private void RefreshPosition()
        {
            Vector3 position = Position;
            position.y = _elevation * HexMetrics.ElevationStep;
            position.y +=
                (HexMetrics.SampleNoise(position).y * 2f - 1f) *
                HexMetrics.ElevationPerturbStrength;
            Position = position;

            Vector3 uiPosition = UiRect.localPosition;
            uiPosition.z = -position.y;
            UiRect.localPosition = uiPosition;
        }

        #endregion

        public new UgfCell GetNeighbor(HexDirection direction)
        {
            return base.GetNeighbor(direction) as UgfCell;
        }

        #region Path

        /// <summary>
        ///     Distance data used by pathfinding algorithm.
        /// </summary>
        public int Distance { get; set; }

        /// <summary>
        ///     Pathing data used by pathfinding algorithm.
        /// </summary>
        public int PathFromIndex { get; set; }

        /// <summary>
        ///     Heuristic data used by pathfinding algorithm.
        /// </summary>
        public int SearchHeuristic { get; set; }

        /// <summary>
        ///     Search priority used by pathfinding algorithm.
        /// </summary>
        public int SearchPriority => Distance + SearchHeuristic;

        /// <summary>
        ///     Search phases data used by pathfinding algorithm.
        /// </summary>
        public int SearchPhase { get; set; }

        /// <summary>
        ///     Linked list reference used by <see cref="UgfCellPriorityQueue" /> for pathfinding.
        /// </summary>
        public UgfCell NextWithSamePriority { get; set; }

        #endregion

        public override void Save(IArchiveWriter writer)
        {
            writer.Write(_terrainType);
            writer.Write(_walled);
            writer.Write(_elevation);
            writer.Write(_waterLevel);
            writer.Write((sbyte)IncomingRiver);
            writer.Write((sbyte)OutgoingRiver);
            for (int i = 0; i < Roads.Length; i++)
            {
                writer.Write(Roads[i]);
            }
        }

        public override void Load(IArchiveReader reader)
        {
            _terrainType = reader.ReadByte();
            _walled = reader.ReadBoolean();
            _elevation = reader.ReadInt16();
            _waterLevel = reader.ReadInt16();
            IncomingRiver = (HexDirection)reader.ReadSByte();
            OutgoingRiver = (HexDirection)reader.ReadSByte();
            for (int i = 0; i < Roads.Length; i++)
            {
                Roads[i] = reader.ReadBoolean();
            }

            RefreshPosition();
        }
    }
}