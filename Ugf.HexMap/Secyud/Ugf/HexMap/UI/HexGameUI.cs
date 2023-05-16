#region

using UnityEngine;
using UnityEngine.EventSystems;

#endregion

namespace Secyud.Ugf.HexMap.UI
{
	/// <summary>
	///     Component that manages the game UI.
	/// </summary>
	public class HexGameUI : MonoBehaviour
	{
		[SerializeField] private HexGrid Grid;
		[SerializeField] private Camera Camera;

		private HexCell _currentCell;

		private HexUnit _selectedUnit;

		private void Update()
		{
			if (!EventSystem.current.IsPointerOverGameObject())
			{
				if (Input.GetMouseButtonDown(0))
				{
					DoSelection();
				}
				else if (_selectedUnit)
				{
					if (Input.GetMouseButtonDown(1))
						DoMove();
					else
						DoPathfinding();
				}
			}
		}

		/// <summary>
		///     Set whether map edit mode is active.
		/// </summary>
		/// <param name="toggle">Whether edit mode is enabled.</param>
		public void SetEditMode(bool toggle)
		{
			enabled = !toggle;
			Grid.ShowUI(!toggle);
			Grid.ClearPath();
			if (toggle)
				Shader.EnableKeyword("_HEX_MAP_EDIT_MODE");
			else
				Shader.DisableKeyword("_HEX_MAP_EDIT_MODE");
		}

		private void DoSelection()
		{
			Grid.ClearPath();
			UpdateCurrentCell();
			if (_currentCell) _selectedUnit = _currentCell.Unit;
		}

		private void DoPathfinding()
		{
			if (UpdateCurrentCell())
			{
				if (_currentCell)
					Grid.FindPath(_selectedUnit.Location, _currentCell, _selectedUnit);
				else
					Grid.ClearPath();
			}
		}

		private void DoMove()
		{
			if (Grid.HasPath)
			{
				_selectedUnit.Travel(Grid.GetPath(),null);
				Grid.ClearPath();
			}
		}

		private bool UpdateCurrentCell()
		{
			var cell =
				Grid.GetCell(Camera.ScreenPointToRay(Input.mousePosition));
			if (cell != _currentCell)
			{
				_currentCell = cell;
				return true;
			}

			return false;
		}
	}
}