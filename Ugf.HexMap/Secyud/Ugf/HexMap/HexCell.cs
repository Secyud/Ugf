#region

using Secyud.Ugf.Archiving;
using Secyud.Ugf.HexMap.Utilities;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.HexMap
{
	/// <summary>
	///     Container component for hex cell data.
	/// </summary>
	public class HexCell : MonoBehaviour, IHexCell, IArchivable
	{
		[SerializeField] private HexCell[] Neighbors;
		[SerializeField] private bool[] Roads;

		private int _elevation = int.MinValue;

		private Image _highlight;

		private byte _terrainTypeIndex;

		private bool _walled;
		private int _waterLevel;

		/// <summary>
		///     Hexagonal coordinates unique to the cell.
		/// </summary>
		public HexCoordinates Coordinates { get; set; }

		/// <summary>
		///     Transform component for the cell's UI visualization.
		/// </summary>
		public RectTransform UIRect { get; set; }

		/// <summary>
		///     Grid chunk that contains the cell.
		/// </summary>
		public HexGridChunk Chunk { get; set; }

		/// <summary>
		///     Elevation at which the cell is visible. Highest of surface and water level.
		/// </summary>
		public int ViewElevation => _elevation >= _waterLevel ? _elevation : _waterLevel;

		/// <summary>
		///     Whether there is an incoming river.
		/// </summary>
		public bool HasIncomingRiver { get; private set; }

		/// <summary>
		///     Whether there is an outgoing river.
		/// </summary>
		public bool HasOutgoingRiver { get; private set; }

		/// <summary>
		///     Whether a river begins or ends in the cell.
		/// </summary>
		public bool HasRiverBeginOrEnd => HasIncomingRiver != HasOutgoingRiver;

		/// <summary>
		///     The direction of the incoming or outgoing river, if applicable.
		/// </summary>
		public HexDirection RiverBeginOrEndDirection =>
			HasIncomingRiver ? IncomingRiver : OutgoingRiver;

		/// <summary>
		///     Whether the cell contains roads.
		/// </summary>
		public bool HasRoads => Roads.Any(t => t);

		/// <summary>
		///     Incoming river direction, if applicable.
		/// </summary>
		public HexDirection IncomingRiver { get; private set; }

		/// <summary>
		///     Outgoing river direction, if applicable.
		/// </summary>
		public HexDirection OutgoingRiver { get; private set; }

		/// <summary>
		///     Local position of this cell's game object.
		/// </summary>
		public Vector3 Position => transform.localPosition;

		/// <summary>
		///     Vertical positions the the stream bed, if applicable.
		/// </summary>
		public float StreamBedY =>
			(_elevation + HexMetrics.StreamBedElevationOffset) * HexMetrics.ElevationStep;

		/// <summary>
		///     Vertical position of the river's surface, if applicable.
		/// </summary>
		public float RiverSurfaceY =>
			(_elevation + HexMetrics.WaterElevationOffset) * HexMetrics.ElevationStep;

		/// <summary>
		///     Vertical position of the water surface, if applicable.
		/// </summary>
		public float WaterSurfaceY =>
			(_waterLevel + HexMetrics.WaterElevationOffset) * HexMetrics.ElevationStep;

		/// <summary>
		///     Whether the cell is considered inside a walled region.
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
		///     Distance data used by pathfinding algorithm.
		/// </summary>
		public int Distance { get; set; }

		/// <summary>
		///     Unit currently occupying the cell, if any.
		/// </summary>
		public HexUnit Unit { get; set; }

		/// <summary>
		///     Pathing data used by pathfinding algorithm.
		/// </summary>
		public HexCell PathFrom { get; set; }

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
		///     Linked list reference used by <see cref="HexCellPriorityQueue" /> for pathfinding.
		/// </summary>
		public HexCell NextWithSamePriority { get; set; }

		/// <summary>
		///     Reference to <see cref="HexCellShaderData" /> that contains the cell.
		/// </summary>
		public HexCellShaderData ShaderData { get; set; }

		public bool IsSpecial { get; set; }

		/// <summary>
		///     Save the cell data.
		/// </summary>
		/// <param name="writer"><see cref="BinaryWriter" /> to use.</param>
		public void Save(BinaryWriter writer)
		{
			writer.Write((byte)_terrainTypeIndex);
			writer.Write((byte)(_elevation + 127));
			writer.Write((byte)_waterLevel);


			writer.Write(IsSpecial);
			writer.Write(_walled);

			if (HasIncomingRiver)
				writer.Write((byte)(IncomingRiver + 128));
			else
				writer.Write((byte)0);

			if (HasOutgoingRiver)
				writer.Write((byte)(OutgoingRiver + 128));
			else
				writer.Write((byte)0);

			var roadFlags = 0;
			for (var i = 0; i < Roads.Length; i++)
				if (Roads[i])
					roadFlags |= 1 << i;

			writer.Write((byte)roadFlags);
		}

		/// <summary>
		///     Load the cell data.
		/// </summary>
		/// <param name="reader"><see cref="BinaryReader" /> to use.</param>
		public void Load(BinaryReader reader)
		{
			_terrainTypeIndex = reader.ReadByte();
			_elevation = reader.ReadByte() - 127;

			RefreshPosition();
			_waterLevel = reader.ReadByte();

			IsSpecial = reader.ReadBoolean();
			_walled = reader.ReadBoolean();
			var riverData = reader.ReadByte();
			if (riverData >= 128)
			{
				HasIncomingRiver = true;
				IncomingRiver = (HexDirection)(riverData - 128);
			}
			else
			{
				HasIncomingRiver = false;
			}

			riverData = reader.ReadByte();
			if (riverData >= 128)
			{
				HasOutgoingRiver = true;
				OutgoingRiver = (HexDirection)(riverData - 128);
			}
			else
			{
				HasOutgoingRiver = false;
			}

			int roadFlags = reader.ReadByte();
			for (var i = 0; i < Roads.Length; i++)
				Roads[i] = (roadFlags & (1 << i)) != 0;

			ShaderData.RefreshTerrain(this);
		}


		/// <summary>
		///     Unique global index of the cell.
		/// </summary>
		public int Index { get; set; }

		public int TmpIndex { get; set; }

		/// <summary>
		///     Surface elevation level.
		/// </summary>
		public int Elevation
		{
			get => _elevation;
			set
			{
				if (_elevation == value) return;

				// int originalViewElevation = ViewElevation;
				_elevation = value;
				ShaderData.ViewElevationChanged(this);
				RefreshPosition();
				ValidateRivers();

				for (var i = 0; i < Roads.Length; i++)
					if (Roads[i] && GetElevationDifference((HexDirection)i) > 1)
						SetRoad(i, false);

				Refresh();
			}
		}

		/// <summary>
		///     Water elevation level.
		/// </summary>
		public int WaterLevel
		{
			get => _waterLevel;
			set
			{
				if (_waterLevel == value) return;

				// int originalViewElevation = ViewElevation;
				_waterLevel = value;
				ShaderData.ViewElevationChanged(this);
				ValidateRivers();
				Refresh();
			}
		}

		/// <summary>
		///     Whether the cell counts as underwater, which is when water is higher than surface.
		/// </summary>
		public bool IsUnderwater => _waterLevel > _elevation;

		/// <summary>
		///     Whether there is a river, either incoming, outgoing, or both.
		/// </summary>
		public bool HasRiver => HasIncomingRiver || HasOutgoingRiver;

		/// <summary>
		///     Terrain type index.
		/// </summary>
		public byte TerrainTypeIndex
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
		///     Get one of the neighbor cells.
		/// </summary>
		/// <param name="direction">Neighbor direction relative to the cell.</param>
		/// <returns>Neighbor cell, if it exists.</returns>
		public HexCell GetNeighbor(HexDirection direction)
		{
			return Neighbors[(int)direction];
		}

		/// <summary>
		///     Set a specific neighbor.
		/// </summary>
		/// <param name="direction">Neighbor direction relative to the cell.</param>
		/// <param name="cell">Neighbor.</param>
		public void SetNeighbor(HexDirection direction, HexCell cell)
		{
			Neighbors[(int)direction] = cell;
			cell.Neighbors[(int)direction.Opposite()] = this;
		}

		/// <summary>
		///     Get the <see cref="HexEdgeType" /> of a cell edge.
		/// </summary>
		/// <param name="direction">Edge direction relative to the cell.</param>
		/// <returns><see cref="HexEdgeType" /> based on the neighboring cells.</returns>
		public HexEdgeType GetEdgeType(HexDirection direction)
		{
			return HexMetrics.GetEdgeType(
				_elevation, Neighbors[(int)direction]._elevation
			);
		}

		/// <summary>
		///     Get the <see cref="HexEdgeType" /> based on this and another cell.
		/// </summary>
		/// <param name="otherCell">Other cell to consider as neighbor.</param>
		/// <returns><see cref="HexEdgeType" /> based on this and the other cell.</returns>
		public HexEdgeType GetEdgeType(HexCell otherCell)
		{
			return HexMetrics.GetEdgeType(
				_elevation, otherCell._elevation
			);
		}

		/// <summary>
		///     Whether a river goes through a specific cell edge.
		/// </summary>
		/// <param name="direction">Edge direction relative to the cell.</param>
		/// <returns></returns>
		public bool HasRiverThroughEdge(HexDirection direction)
		{
			return (HasIncomingRiver && IncomingRiver == direction) ||
				(HasOutgoingRiver && OutgoingRiver == direction);
		}

		/// <summary>
		///     Remove the incoming river, if it exists.
		/// </summary>
		public void RemoveIncomingRiver()
		{
			if (!HasIncomingRiver) return;

			HasIncomingRiver = false;
			RefreshSelfOnly();

			var neighbor = GetNeighbor(IncomingRiver);
			neighbor.HasOutgoingRiver = false;
			neighbor.RefreshSelfOnly();
		}

		/// <summary>
		///     Remove the outgoing river, if it exists.
		/// </summary>
		public void RemoveOutgoingRiver()
		{
			if (!HasOutgoingRiver) return;

			HasOutgoingRiver = false;
			RefreshSelfOnly();

			var neighbor = GetNeighbor(OutgoingRiver);
			neighbor.HasIncomingRiver = false;
			neighbor.RefreshSelfOnly();
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

			var neighbor = GetNeighbor(direction);
			if (!IsValidRiverDestination(neighbor)) return;

			RemoveOutgoingRiver();
			if (HasIncomingRiver && IncomingRiver == direction) RemoveIncomingRiver();

			HasOutgoingRiver = true;
			OutgoingRiver = direction;

			neighbor.RemoveIncomingRiver();
			neighbor.HasIncomingRiver = true;
			neighbor.IncomingRiver = direction.Opposite();

			SetRoad((int)direction, false);
		}

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
				SetRoad((int)direction, true);
		}

		/// <summary>
		///     Remove all roads from the cell.
		/// </summary>
		public void RemoveRoads()
		{
			for (var i = 0; i < Neighbors.Length; i++)
				if (Roads[i])
					SetRoad(i, false);
		}

		/// <summary>
		///     Get the elevation difference with a neighbor. The indicated neighbor must exist.
		/// </summary>
		/// <param name="direction">Direction to the neighbor, relative to the cell.</param>
		/// <returns>Absolute elevation difference.</returns>
		public int GetElevationDifference(HexDirection direction)
		{
			var difference = _elevation - GetNeighbor(direction)._elevation;
			return difference >= 0 ? difference : -difference;
		}

		private bool IsValidRiverDestination(HexCell neighbor)
		{
			return neighbor && (_elevation >= neighbor._elevation || _waterLevel == neighbor._elevation);
		}

		private void ValidateRivers()
		{
			if (
				HasOutgoingRiver &&
				!IsValidRiverDestination(GetNeighbor(OutgoingRiver))
			)
				RemoveOutgoingRiver();

			if (
				HasIncomingRiver &&
				!GetNeighbor(IncomingRiver).IsValidRiverDestination(this)
			)
				RemoveIncomingRiver();
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
			var position = transform.localPosition;
			position.y = _elevation * HexMetrics.ElevationStep;
			position.y +=
				(HexMetrics.SampleNoise(position).y * 2f - 1f) *
				HexMetrics.ElevationPerturbStrength;
			transform.localPosition = position;

			var uiPosition = UIRect.localPosition;
			uiPosition.z = -position.y;
			UIRect.localPosition = uiPosition;
		}

		public void Refresh()
		{
			if (Chunk)
			{
				Chunk.Refresh();
				foreach (var neighbor in Neighbors)
					if (neighbor != null && neighbor.Chunk != Chunk)
						neighbor.Chunk.Refresh();

				if (Unit) Unit.ValidateLocation();
			}
		}

		public void RefreshSelfOnly()
		{
			Chunk.Refresh();
			if (Unit) Unit.ValidateLocation();
		}

		/// <summary>
		///     Set the cell's UI label.
		/// </summary>
		/// <param name="text">Label text.</param>
		public void SetLabel(string text)
		{
			var label = UIRect.GetComponent<Text>();
			label.text = text;
		}

		/// <summary>
		///     Disable the cell's highlight.
		/// </summary>
		public void DisableHighlight()
		{
			if (!_highlight)
				_highlight = UIRect.GetChild(0).GetComponent<Image>();
			_highlight.enabled = false;
		}

		/// <summary>
		///     Enable the cell's highlight.
		/// </summary>
		/// <param name="color">Highlight color.</param>
		public void EnableHighlight(Color color)
		{
			if (!_highlight)
				_highlight = UIRect.GetChild(0).GetComponent<Image>();
			_highlight.color = color;
			_highlight.enabled = true;
		}

		public int CostTo(HexCell target, HexDirection direction)
		{
			if (target.Index < 0)
				return -1;

			int dHeight = Math.Abs(target.Elevation - Elevation) + 3;
			if (target.Elevation > Elevation)
				dHeight += 3;
			if (target.IsUnderwater)
				dHeight += 3;
			if (Walled != target.Walled)
				dHeight += 3;
			if (!HasRoadThroughEdge(direction))
				dHeight += 3;
			return dHeight * 32;
		}
	}
}