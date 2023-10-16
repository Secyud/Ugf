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


		private byte _activeTerrainIndex;

		private short _activeLevel;

		private ObjectToggle _applyObject;

		private int _brushSize;
		private HexDirection _dragDirection;

		private bool _isDrag;
		private UgfCell _previousCell;
		private bool _apply;

		private void Awake()
		{
			Shader.EnableKeyword("_HEX_MAP_EDIT_MODE");
			SetEditMode(true);
		}

		private void Update()
		{
			if (!EventSystem.current.IsPointerOverGameObject())
			{
				if (Input.GetMouseButton(0))
				{
					HandleInput();
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

		public void SetTerrainTypeIndex(byte index)
		{
			_activeTerrainIndex = index;
		}

		public void SetLevel(float level)
		{
			level = (int)level;
		}

		public void SetMode(bool mode)
		{
			_apply = mode;
		}

		public void SetApplyObject(int toggle)
		{
			_applyObject = (ObjectToggle)toggle;
		}

		public void SetEditMode(bool toggle)
		{
			enabled = toggle;
		}


		private UgfCell GetCellUnderCursor()
		{
			return HexGrid.GetCellUnderCursor() as UgfCell;
		}


		private void HandleInput()
		{
			UgfCell currentCell = GetCellUnderCursor();
			if (currentCell is not null)
			{
				if (_previousCell is not null && 
				    _previousCell != currentCell)
					ValidateDrag(currentCell);
				else
					_isDrag = false;

				EditCells(currentCell);
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

		private void EditCells(UgfCell center)
		{
			int centerX = center.Coordinates.X;
			int centerZ = center.Coordinates.Z;

			for (int r = 0, z = centerZ - _brushSize; z <= centerZ; z++, r++)
			for (int x = centerX - r; x <= centerX + _brushSize; x++)
				EditCell(HexGrid.GetCell(new HexCoordinates(x, z)) as UgfCell);

			for (int r = 0, z = centerZ + _brushSize; z > centerZ; z--, r++)
			for (int x = centerX - _brushSize; x <= centerX + r; x++)
				EditCell(HexGrid.GetCell(new HexCoordinates(x, z)) as UgfCell);
		}

		private void EditCell(UgfCell cell)
		{
			if (cell is not null)
			{
				cell.TerrainType = _activeTerrainIndex;

				switch (_applyObject)
				{
					case ObjectToggle.Ignore:
						break;
					case ObjectToggle.Evaluation: 
						cell.Elevation = _activeLevel;
						break;
					case ObjectToggle.WaterLevel:
						cell.WaterLevel = _activeLevel;
						break;
					case ObjectToggle.River:
						if (!_apply)
							cell.RemoveRiver();
						else if (_isDrag)
							GetDragCell(cell)?.SetOutgoingRiver(_dragDirection);     
						break;
					case ObjectToggle.Road:       
						if (!_apply)
							cell.RemoveRoads();
						else if (_isDrag)
							GetDragCell(cell)?.AddRoad(_dragDirection);
						break;
					case ObjectToggle.Wall:
						cell.Walled = _apply; 
						break;
					default:            
						throw new ArgumentOutOfRangeException();
				}
			}
		}


		private enum ObjectToggle
		{
			Ignore, Evaluation, WaterLevel, River, Road,Wall
		}
	}
}