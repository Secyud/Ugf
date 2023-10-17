#region

using System;
using Secyud.Ugf.HexMap;
using Secyud.Ugf.HexUtilities;
using Secyud.Ugf.UgfHexMap;
using UnityEngine;
using UnityEngine.EventSystems;

#endregion

namespace Secyud.Ugf.UgfHexMapEditor
{
	/// <summary>
	///     Component that applies UI commands to the hex map.
	///     Public methods are hooked up to the in-game UI.
	/// </summary>
	public class HexMapEditor : MonoBehaviour
	{
		private static readonly int CellHighlightingId = Shader.PropertyToID("_CellHighlighting");

		[SerializeField] private HexGrid HexGrid;


		private byte _activeMoisture;
		private byte _activeTemperature;
		private byte _activeTerrain;

		private short _activeLevel;

		private ObjectToggle _applyObject;

		private int _brushSize;
		private HexDirection _dragDirection;

		private bool _isDrag;
		private UgfCell _previousCell;

		private void Awake()
		{
			Shader.EnableKeyword("_HEX_MAP_EDIT_MODE");
			HexGrid.Initialize(U.Get<UgfHexGridDrawer>());
		}

		private void Update()
		{
			if (!EventSystem.current.IsPointerOverGameObject())
			{
				if (Input.GetMouseButton(0))
				{
					HandleInput(true);
					return;
				}
				else if (Input.GetMouseButton(1))
				{
					HandleInput(false);
					return;
				}

				// Potential optimization: only do this if camera or cursor has changed.
				UpdateCellHighlightData(GetCellUnderCursor());
			}
			else
			{
				ClearCellHighlightData();
			}

			_previousCell = null;
		}


		public void SetBrushSize(float size)
		{
			_brushSize = (int)size;
		}

		public void SetTemperature(int index)
		{
			_activeTemperature = (byte)index;
			SetTerrain();
		}

		public void SetMoisture(int index)
		{
			_activeMoisture = (byte)index;
			SetTerrain();
		}

		private void SetTerrain()
		{
			_activeTerrain = (byte)(_activeTemperature * 4 + _activeMoisture);
		}

		public void SetLevel(float level)
		{
			_activeLevel = (short)level;
		}

		public void SetApplyObject(int toggle)
		{
			_applyObject = (ObjectToggle)toggle;
		}


		private UgfCell GetCellUnderCursor()
		{
			return HexGrid.GetCellUnderCursor() as UgfCell;
		}


		private void HandleInput(bool apply)
		{
			UgfCell currentCell = GetCellUnderCursor();
			if (currentCell is not null)
			{
				if (_previousCell is not null && 
				    _previousCell != currentCell)
					ValidateDrag(currentCell);
				else
					_isDrag = false;

				EditCells(currentCell,apply);
				_previousCell = currentCell;
			}
			else
			{
				_previousCell = null;
			}

			UpdateCellHighlightData(currentCell);
		}

		private void UpdateCellHighlightData(UgfCell cell)
		{
			if (cell == null)
			{
				ClearCellHighlightData();
				return;
			}

			// Works up to brush size 6.
			Shader.SetGlobalVector(
				CellHighlightingId,
				new Vector4(
					cell.Coordinates.HexX,
					cell.Coordinates.HexZ,
					_brushSize * _brushSize + 0.5f
				)
			);
		}

		private void ClearCellHighlightData()
		{
			Shader.SetGlobalVector(CellHighlightingId, new Vector4(0f, 0f, -1f, 0f));
		}

		private UgfCell GetDragCell(UgfCell cell)
		{
			return cell.GetNeighbor(_dragDirection.Opposite());
		}

		private void ValidateDrag(UgfCell currentCell)
		{
			for (
				_dragDirection = HexDirection.Ne;
				_dragDirection <= HexDirection.Nw;
				_dragDirection++
			)
				if (_previousCell.GetNeighbor(_dragDirection) == currentCell)
				{
					_isDrag = true;
					return;
				}

			_isDrag = false;
		}

		private void EditCells(UgfCell center,bool apply)
		{
			int centerX = center.Coordinates.X;
			int centerZ = center.Coordinates.Z;

			for (int r = 0, z = centerZ - _brushSize; z <= centerZ; z++, r++)
			for (int x = centerX - r; x <= centerX + _brushSize; x++)
				EditCell(HexGrid.GetCell(new HexCoordinates(x, z)) as UgfCell,apply);

			for (int r = 0, z = centerZ + _brushSize; z > centerZ; z--, r++)
			for (int x = centerX - _brushSize; x <= centerX + r; x++)
				EditCell(HexGrid.GetCell(new HexCoordinates(x, z)) as UgfCell,apply);
		}

		private void EditCell(UgfCell cell,bool apply)
		{
			if (cell is null) return;
			
			switch (_applyObject)
			{
				case ObjectToggle.Ignore:
					cell.TerrainType = _activeTerrain;
					break;
				case ObjectToggle.Evaluation:
					if (apply && cell.Elevation< _activeLevel)
					{
						cell.Elevation += 1;
					}
					else if (!apply && cell.Elevation> _activeLevel)
					{
						cell.Elevation -= 1;
					}
					break;
				case ObjectToggle.WaterLevel:
					cell.WaterLevel = _activeLevel;
					break;
				case ObjectToggle.River:
					if (!apply)
						cell.RemoveRiver();
					else if (_isDrag)
						GetDragCell(cell)?.SetOutgoingRiver(_dragDirection);     
					break;
				case ObjectToggle.Road:       
					if (!apply)
						cell.RemoveRoads();
					else if (_isDrag)
						GetDragCell(cell)?.AddRoad(_dragDirection);
					break;
				case ObjectToggle.Wall:
					cell.Walled = apply; 
					break;
				default:            
					throw new ArgumentOutOfRangeException();
			}
		}


		private enum ObjectToggle
		{
			Ignore,  WaterLevel,Evaluation,  River,Road,Wall
		}
	}
}