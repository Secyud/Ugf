#region

using UnityEngine;
using UnityEngine.EventSystems;

#endregion

namespace Secyud.Ugf.HexMap
{
    /// <summary>
    /// Component that applies UI commands to the hex map.
    /// Public methods are hooked up to the in-game UI.
    /// </summary>
    public class HexMapEditor : MonoBehaviour
    {
        private static readonly int CellHighlightingId = Shader.PropertyToID("_CellHighlighting");

        [SerializeField] private HexGrid HexGrid;
        [SerializeField] private Material TerrainMaterial;

        private int _activeElevation;
        private int _activeWaterLevel;

        private enum EvaluationWaterToggle
        {
            Ignore,
            Evaluation,
            WaterLevel
        }

        private EvaluationWaterToggle _applyElevationWater;

        private int _activeUrbanLevel, _activeFarmLevel, _activePlantLevel, _activeSpecialIndex;

        private enum SpecialToggle
        {
            Ignore,
            Urban,
            Farm,
            Plant,
            Special
        }

        private SpecialToggle _applySpecial;

        private int _activeTerrainTypeIndex;


        private bool _riverMode, _roadMode;

        private enum RiverRoadToggle
        {
            Ignore,
            River,
            Road
        }

        private RiverRoadToggle _applyRiverRoad;

        private enum OptionalToggle
        {
            Ignore,
            Yes,

            // ReSharper disable once UnusedMember.Local
            No
        }

        private OptionalToggle _walledMode;

        private int _brushSize;

        private bool _isDrag;
        private HexDirection _dragDirection;
        private HexCell _previousCell;


        public void SetBrushSize(float size) => _brushSize = (int)size;

        public void SetTerrainTypeIndex(int index) => _activeTerrainTypeIndex = index;

        public void SetWaterLevel(float level) => _activeWaterLevel = (int)level;
        public void SetElevation(float elevation) => _activeElevation = (int)elevation;
        public void SetApplyEvaluationWater(int toggle) => _applyElevationWater = (EvaluationWaterToggle)toggle;

        public void SetUrbanLevel(float level) => _activeUrbanLevel = (int)level;
        public void SetFarmLevel(float level) => _activeFarmLevel = (int)level;
        public void SetPlantLevel(float level) => _activePlantLevel = (int)level;
        public void SetSpecialIndex(float index) => _activeSpecialIndex = (int)index;
        public void SetApplySpecial(int toggle) => _applySpecial = (SpecialToggle)toggle;

        public void SetRiverRoadMode(bool mode) => _riverMode = mode;
        public void SetRoadMode(bool mode) => _roadMode = mode;
        public void SetApplyRiverRoad(int toggle) => _applyRiverRoad = (RiverRoadToggle)toggle;

        public void SetWalledMode(int mode) => _walledMode = (OptionalToggle)mode;

        public void SetEditMode(bool toggle) => enabled = toggle;

        public void ShowGrid(bool visible)
        {
            if (visible)
            {
                TerrainMaterial.EnableKeyword("_SHOW_GRID");
            }
            else
            {
                TerrainMaterial.DisableKeyword("_SHOW_GRID");
            }
        }

        private void Awake()
        {
            TerrainMaterial.DisableKeyword("_SHOW_GRID");
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
                else
                {
                    // Potential optimization: only do this if camera or cursor has changed.
                    UpdateCellHighlightData(GetCellUnderCursor());
                }

                if (Input.GetKeyDown(KeyCode.U))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        DestroyUnit();
                    }
                    else
                    {
                        CreateUnit();
                    }

                    return;
                }
            }
            else
            {
                ClearCellHighlightData();
            }

            _previousCell = null;
        }

        private HexCell GetCellUnderCursor() =>
            HexGrid.GetCell(Camera.main!.ScreenPointToRay(Input.mousePosition));

        private void CreateUnit()
        {
            HexCell cell = GetCellUnderCursor();
            if (cell && !cell.Unit)
            {
                // HexGrid.AddUnit(
                //     Instantiate(HexUnit.UnitPrefab), cell, Random.Range(0f, 360f)
                // );
            }
        }

        private void DestroyUnit()
        {
            HexCell cell = GetCellUnderCursor();
            if (cell && cell.Unit)
            {
                HexGrid.RemoveUnit(cell.Unit);
            }
        }

        private void HandleInput()
        {
            HexCell currentCell = GetCellUnderCursor();
            if (currentCell)
            {
                if (_previousCell && _previousCell != currentCell)
                {
                    ValidateDrag(currentCell);
                }
                else
                {
                    _isDrag = false;
                }

                EditCells(currentCell);
                _previousCell = currentCell;
            }
            else
            {
                _previousCell = null;
            }

            UpdateCellHighlightData(currentCell);
        }

        private void UpdateCellHighlightData(HexCell cell)
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
                    _brushSize * _brushSize + 0.5f,
                    HexMetrics.WrapSize
                )
            );
        }

        private void ClearCellHighlightData() =>
            Shader.SetGlobalVector(CellHighlightingId, new Vector4(0f, 0f, -1f, 0f));

        private HexCell GetDragCell(HexCell cell) => cell.GetNeighbor(_dragDirection.Opposite());

        private void ValidateDrag(HexCell currentCell)
        {
            for (
                _dragDirection = HexDirection.Ne;
                _dragDirection <= HexDirection.Nw;
                _dragDirection++
            )
            {
                if (_previousCell.GetNeighbor(_dragDirection) == currentCell)
                {
                    _isDrag = true;
                    return;
                }
            }

            _isDrag = false;
        }

        private void EditCells(HexCell center)
        {
            int centerX = center.Coordinates.X;
            int centerZ = center.Coordinates.Z;

            for (int r = 0, z = centerZ - _brushSize; z <= centerZ; z++, r++)
            {
                for (int x = centerX - r; x <= centerX + _brushSize; x++)
                {
                    EditCell(HexGrid.GetCell(new HexCoordinates(x, z)));
                }
            }

            for (int r = 0, z = centerZ + _brushSize; z > centerZ; z--, r++)
            {
                for (int x = centerX - _brushSize; x <= centerX + r; x++)
                {
                    EditCell(HexGrid.GetCell(new HexCoordinates(x, z)));
                }
            }
        }

        private void EditCell(HexCell cell)
        {
            if (cell)
            {
                if (_activeTerrainTypeIndex >= 0)
                    cell.TerrainTypeIndex = _activeTerrainTypeIndex;

                switch (_applyElevationWater)
                {
                    case EvaluationWaterToggle.Evaluation:
                        cell.Elevation = _activeElevation;
                        break;
                    case EvaluationWaterToggle.WaterLevel:
                        cell.WaterLevel = _activeWaterLevel;
                        break;
                    case EvaluationWaterToggle.Ignore:
                    default:
                        break;
                }

                // switch (_applySpecial)
                // {
                //     case SpecialToggle.Urban:
                //         cell.UrbanLevel = _activeUrbanLevel;
                //         break;
                //     case SpecialToggle.Farm:
                //         cell.FarmLevel = _activeFarmLevel;
                //         break;
                //     case SpecialToggle.Plant:
                //         cell.PlantLevel = _activePlantLevel;
                //         break;
                //     case SpecialToggle.Special:
                //         cell.SpecialIndex = _activeSpecialIndex;
                //         break;
                //     case SpecialToggle.Ignore:
                //     default:
                //         break;
                // }

                if (_walledMode != OptionalToggle.Ignore)
                {
                    cell.Walled = _walledMode == OptionalToggle.Yes;
                }

                switch (_applyRiverRoad)
                {
                    case RiverRoadToggle.River:
                        if (!_riverMode)
                            cell.RemoveRiver();
                        else if (_isDrag)
                            GetDragCell(cell)?.SetOutgoingRiver(_dragDirection);
                        break;
                    case RiverRoadToggle.Road:
                        if (!_roadMode)
                            cell.RemoveRoads();
                        else if (_isDrag)
                            GetDragCell(cell)?.AddRoad(_dragDirection);
                        break;
                    case RiverRoadToggle.Ignore:
                    default:
                        break;
                }
            }
        }
    }
}