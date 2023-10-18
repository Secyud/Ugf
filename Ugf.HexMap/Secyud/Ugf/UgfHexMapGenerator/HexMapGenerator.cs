#region

using System.Collections.Generic;
using Secyud.Ugf.HexMap;
using Secyud.Ugf.HexUtilities;
using Secyud.Ugf.UgfHexMap;
using UnityEngine;

#endregion

namespace Secyud.Ugf.UgfHexMapGenerator
{
    public class HexMapGenerator : MonoBehaviour
    {
        private static readonly float[] TemperatureBands = { 0.1f, 0.3f, 0.6f };
        private static readonly float[] MoistureBands = { 0.12f, 0.28f, 0.85f };
        private readonly List<HexDirection> _flowDirections = new();
        private int CellCount => Grid.Cells.Length;
        private int _landCells;
        private List<ClimateData> _climate = new();
        private List<ClimateData> _nextClimate = new();
        private List<MapRegion> _regions;
        private UgfCellPriorityQueue _searchFrontier;
        private int _searchFrontierPhase;
        private int _temperatureJitterChannel;


        public int Seed;

        public HemisphereMode Hemisphere;

        public HexDirection WindDirection  = HexDirection.Nw;


        public int ChunkSizeMin  = 30;

        public int ChunkSizeMax = 100;

        public int RiverPercentage  = 10;

        public int LandPercentage = 50;

        public int ErosionPercentage = 50;

        public int ElevationMinimum  = -2;

        public int WaterLevel => 2;

        public int ElevationMaximum = 8;

        public int RegionBorder = 5;

        public int RegionCount = 1;

        public float WindStrength  = 4f;

        public float JitterProbability = 0.25f;

        public float SinkProbability= 0.2f;

        // min 0 max 1
        public float HighRiseProbability = 0.25f;

        public float StartingMoisture  = 0.1f;

        public float EvaporationFactor  = 0.5f;

        public float PrecipitationFactor = 0.25f;

        public float RunoffFactor = 0.25f;

        public float SeepageFactor = 0.125f;

        public float ExtraLakeProbability = 0.25f;

        public float LowTemperature = 0f;

        public float HighTemperature = 1f;

        public float TemperatureJitter = 0.1f;

        public int ChunkCountX = 4;

        public int ChunkCountZ = 4;

        public HexGrid Grid;

        public int CellCountX => Grid.CellCountX;
        public int CellCountZ => Grid.CellCountZ;

        public void GenerateMap()
        {
            Random.State originalRandomState = Random.state;

            Random.InitState(Seed);

            Grid.CreateMap(ChunkCountX, ChunkCountZ);
            _searchFrontier ??= new UgfCellPriorityQueue();
            
            foreach (HexCell hexCell in Grid.Cells)
            {
                UgfCell cell = (UgfCell)hexCell;
                cell.Elevation = 0;
            }
            
            CreateRegions();
            CreateLand();
            ErodeLand();
            CreateClimate();
            CreateRivers();
            SetTerrainType();

            Random.state = originalRandomState;
        }

        private void CreateRegions()
        {
            if (_regions == null)
                _regions = new List<MapRegion>();
            else
                _regions.Clear();

            MapRegion region;

            switch (RegionCount)
            {
                default:
                    region.XMin = 0;
                    region.XMax = CellCountX;
                    region.ZMin = 0;
                    region.ZMax = CellCountZ;
                    _regions.Add(region);
                    break;
                case 2:
                    if (Random.value < 0.5f)
                    {
                        region.XMin = 0;
                        region.XMax = CellCountX / 2 - RegionBorder;
                        region.ZMin = 0;
                        region.ZMax = CellCountZ - 0;
                        _regions.Add(region);
                        region.XMin = CellCountX / 2 + RegionBorder;
                        region.XMax = CellCountZ - 0;
                        _regions.Add(region);
                    }
                    else
                    {
                        region.XMin = 0;
                        region.XMax = CellCountX - 0;
                        region.ZMin = 0;
                        region.ZMax = CellCountZ / 2 - RegionBorder;
                        _regions.Add(region);
                        region.ZMin = CellCountZ / 2 + RegionBorder;
                        region.ZMax = CellCountZ - 0;
                        _regions.Add(region);
                    }

                    break;
                case 3:
                    region.XMin = 0;
                    region.XMax = CellCountX / 3 - RegionBorder;
                    region.ZMin = 0;
                    region.ZMax = CellCountZ - 0;
                    _regions.Add(region);
                    region.XMin = CellCountX / 3 + RegionBorder;
                    region.XMax = CellCountX * 2 / 3 - RegionBorder;
                    _regions.Add(region);
                    region.XMin = CellCountX * 2 / 3 + RegionBorder;
                    region.XMax = CellCountX - 0;
                    _regions.Add(region);
                    break;
                case 4:
                    region.XMin = 0;
                    region.XMax = CellCountX / 2 - RegionBorder;
                    region.ZMin = 0;
                    region.ZMax = CellCountZ / 2 - RegionBorder;
                    _regions.Add(region);
                    region.XMin = CellCountX / 2 + RegionBorder;
                    region.XMax = CellCountX - 0;
                    _regions.Add(region);
                    region.ZMin = CellCountZ / 2 + RegionBorder;
                    region.ZMax = CellCountZ - 0;
                    _regions.Add(region);
                    region.XMin = 0;
                    region.XMax = CellCountX / 2 - RegionBorder;
                    _regions.Add(region);
                    break;
            }
        }

        private void CreateLand()
        {
            int landBudget = Mathf.RoundToInt(CellCount * LandPercentage * 0.01f);
            _landCells = landBudget;
            for (int guard = 0; guard < 10000; guard++)
            {
                bool sink = Random.value < SinkProbability;
                foreach (MapRegion region in _regions)
                {
                    int chunkSize = Random.Range(ChunkSizeMin, ChunkSizeMax - 1);
                    if (sink)
                    {
                        landBudget = SinkTerrain(chunkSize, landBudget, region);
                    }
                    else
                    {
                        landBudget = RaiseTerrain(chunkSize, landBudget, region);
                        if (landBudget == 0) return;
                    }
                }
            }

            if (landBudget > 0)
            {
                Debug.LogWarning("Failed to use up " + landBudget + " land budget.");
                _landCells -= landBudget;
            }
        }

        private int RaiseTerrain(int chunkSize, int budget, MapRegion region)
        {
            _searchFrontierPhase += 1;
            UgfCell firstCell = GetRandomCell(region);
            firstCell.SearchPhase = _searchFrontierPhase;
            firstCell.Distance = 0;
            firstCell.SearchHeuristic = 0;
            _searchFrontier.Enqueue(firstCell);
            HexCoordinates center = firstCell.Coordinates;

            int rise = Random.value < HighRiseProbability ? 2 : 1;
            int size = 0;
            while (size < chunkSize && _searchFrontier.Count > 0)
            {
                UgfCell current = _searchFrontier.Dequeue();
                int originalElevation = current.Elevation;
                int newElevation = originalElevation + rise;
                if (newElevation > ElevationMaximum) continue;

                current.Elevation = (short)newElevation;
                if (
                    originalElevation < WaterLevel &&
                    newElevation >= WaterLevel && --budget == 0
                )
                    break;

                size += 1;

                for (HexDirection d = HexDirection.Ne; d <= HexDirection.Nw; d++)
                {
                    UgfCell neighbor = current.GetNeighbor(d);
                    if (neighbor && neighbor.SearchPhase < _searchFrontierPhase)
                    {
                        neighbor.SearchPhase = _searchFrontierPhase;
                        neighbor.Distance = neighbor.Coordinates.DistanceTo(center);
                        neighbor.SearchHeuristic =
                            Random.value < JitterProbability ? 1 : 0;
                        _searchFrontier.Enqueue(neighbor);
                    }
                }
            }

            _searchFrontier.Clear();
            return budget;
        }

        private int SinkTerrain(int chunkSize, int budget, MapRegion region)
        {
            _searchFrontierPhase += 1;
            UgfCell firstCell = GetRandomCell(region);
            firstCell.SearchPhase = _searchFrontierPhase;
            firstCell.Distance = 0;
            firstCell.SearchHeuristic = 0;
            _searchFrontier.Enqueue(firstCell);
            HexCoordinates center = firstCell.Coordinates;

            int sink = Random.value < HighRiseProbability ? 2 : 1;
            int size = 0;
            while (size < chunkSize && _searchFrontier.Count > 0)
            {
                UgfCell current = _searchFrontier.Dequeue();
                int originalElevation = current.Elevation;
                int newElevation = current.Elevation - sink;
                if (newElevation < ElevationMinimum) continue;

                current.Elevation = (short)newElevation;
                if (originalElevation >= WaterLevel &&
                    newElevation < WaterLevel)
                    budget += 1;

                size += 1;

                for (HexDirection d = HexDirection.Ne; d <= HexDirection.Nw; d++)
                {
                    UgfCell neighbor = current.GetNeighbor(d);
                    if (neighbor  && neighbor.SearchPhase < _searchFrontierPhase)
                    {
                        neighbor.SearchPhase = _searchFrontierPhase;
                        neighbor.Distance = neighbor.Coordinates.DistanceTo(center);
                        neighbor.SearchHeuristic =
                            Random.value < JitterProbability ? 1 : 0;
                        _searchFrontier.Enqueue(neighbor);
                    }
                }
            }

            _searchFrontier.Clear();
            return budget;
        }

        private void ErodeLand()
        {
            List<UgfCell> erodibleCells = ListPool<UgfCell>.Get();
            for (int i = 0; i < CellCount; i++)
            {
                UgfCell cell = Grid.GetCell(i) as UgfCell;
                if (IsErodible(cell)) erodibleCells.Add(cell);
            }

            int targetErodibleCount =
                (int)(erodibleCells.Count * (100 - ErosionPercentage) * 0.01f);

            while (erodibleCells.Count > targetErodibleCount)
            {
                int index = Random.Range(0, erodibleCells.Count);
                UgfCell cell = erodibleCells[index];
                UgfCell targetCell = GetErosionTarget(cell);

                cell.Elevation -= 1;
                targetCell.Elevation += 1;

                if (!IsErodible(cell))
                {
                    erodibleCells[index] = erodibleCells[^1];
                    erodibleCells.RemoveAt(erodibleCells.Count - 1);
                }

                for (HexDirection d = HexDirection.Ne; d <= HexDirection.Nw; d++)
                {
                    UgfCell neighbor = cell.GetNeighbor(d);
                    if (
                        neighbor  && neighbor.Elevation == cell.Elevation + 2 &&
                        !erodibleCells.Contains(neighbor)
                    )
                        erodibleCells.Add(neighbor);
                }

                if (IsErodible(targetCell) && !erodibleCells.Contains(targetCell)) erodibleCells.Add(targetCell);

                for (HexDirection d = HexDirection.Ne; d <= HexDirection.Nw; d++)
                {
                    UgfCell neighbor = targetCell.GetNeighbor(d);
                    if (
                        neighbor && neighbor != cell &&
                        neighbor.Elevation == targetCell.Elevation + 1 &&
                        !IsErodible(neighbor)
                    )
                        erodibleCells.Remove(neighbor);
                }
            }

            ListPool<UgfCell>.Add(erodibleCells);
        }

        private bool IsErodible(UgfCell cell)
        {
            int erodibleElevation = cell.Elevation - 2;
            for (HexDirection d = HexDirection.Ne; d <= HexDirection.Nw; d++)
            {
                UgfCell neighbor = cell.GetNeighbor(d);
                if (neighbor && neighbor.Elevation <= erodibleElevation) return true;
            }

            return false;
        }

        private UgfCell GetErosionTarget(UgfCell cell)
        {
            List<UgfCell> candidates = ListPool<UgfCell>.Get();
            int erodibleElevation = cell.Elevation - 2;
            for (HexDirection d = HexDirection.Ne; d <= HexDirection.Nw; d++)
            {
                UgfCell neighbor = cell.GetNeighbor(d);
                if (neighbor && neighbor.Elevation <= erodibleElevation) candidates.Add(neighbor);
            }

            UgfCell target = candidates[Random.Range(0, candidates.Count)];
            ListPool<UgfCell>.Add(candidates);
            return target;
        }

        private void CreateClimate()
        {
            _climate.Clear();
            _nextClimate.Clear();
            ClimateData initialData = new ClimateData
            {
                Moisture = StartingMoisture
            };
            ClimateData clearData = new ClimateData();
            for (int i = 0; i < CellCount; i++)
            {
                _climate.Add(initialData);
                _nextClimate.Add(clearData);
            }

            for (int cycle = 0; cycle < 40; cycle++)
            {
                for (int i = 0; i < CellCount; i++) EvolveClimate(i);

                (_climate, _nextClimate) = (_nextClimate, _climate);
            }
        }

        private void EvolveClimate(int i)
        {
            UgfCell cell = Grid.GetCell(i) as UgfCell;
            ClimateData cellClimate = _climate[i];

            if (cell!.IsUnderwater)
            {
                cellClimate.Moisture = 1f;
                cellClimate.Clouds += EvaporationFactor;
            }
            else
            {
                float evaporation = cellClimate.Moisture * EvaporationFactor;
                cellClimate.Moisture -= evaporation;
                cellClimate.Clouds += evaporation;
            }

            float precipitation = cellClimate.Clouds * PrecipitationFactor;
            cellClimate.Clouds -= precipitation;
            cellClimate.Moisture += precipitation;

            float cloudMaximum = 1f - cell.ViewElevation / (ElevationMaximum + 1f);
            if (cellClimate.Clouds > cloudMaximum)
            {
                cellClimate.Moisture += cellClimate.Clouds - cloudMaximum;
                cellClimate.Clouds = cloudMaximum;
            }

            HexDirection mainDispersalDirection = WindDirection.Opposite();
            float cloudDispersal = cellClimate.Clouds * (1f / (5f + WindStrength));
            float runoff = cellClimate.Moisture * RunoffFactor * (1f / 6f);
            float seepage = cellClimate.Moisture * SeepageFactor * (1f / 6f);
            for (HexDirection d = HexDirection.Ne; d <= HexDirection.Nw; d++)
            {
                UgfCell neighbor = cell.GetNeighbor(d);
                if (!neighbor) continue;

                ClimateData neighborClimate = _nextClimate[neighbor.Index];
                if (d == mainDispersalDirection)
                    neighborClimate.Clouds += cloudDispersal * WindStrength;
                else
                    neighborClimate.Clouds += cloudDispersal;

                int elevationDelta = neighbor.ViewElevation - cell.ViewElevation;
                if (elevationDelta < 0)
                {
                    cellClimate.Moisture -= runoff;
                    neighborClimate.Moisture += runoff;
                }
                else if (elevationDelta == 0)
                {
                    cellClimate.Moisture -= seepage;
                    neighborClimate.Moisture += seepage;
                }

                _nextClimate[neighbor.Index] = neighborClimate;
            }

            ClimateData nextCellClimate = _nextClimate[i];
            nextCellClimate.Moisture += cellClimate.Moisture;
            if (nextCellClimate.Moisture > 1f) nextCellClimate.Moisture = 1f;

            _nextClimate[i] = nextCellClimate;
            _climate[i] = new ClimateData();
        }

        private void CreateRivers()
        {
            List<UgfCell> riverOrigins = ListPool<UgfCell>.Get();
            for (int i = 0; i < CellCount; i++)
            {
                UgfCell cell = Grid.GetCell(i) as UgfCell;
                if (cell!.IsUnderwater) continue;

                ClimateData data = _climate[i];
                float weight =
                    data.Moisture * (cell.Elevation - WaterLevel) /
                    (ElevationMaximum - WaterLevel);
                if (weight > 0.75f)
                {
                    riverOrigins.Add(cell);
                    riverOrigins.Add(cell);
                }

                if (weight > 0.5f) riverOrigins.Add(cell);

                if (weight > 0.25f) riverOrigins.Add(cell);
            }

            int riverBudget = Mathf.RoundToInt(_landCells * RiverPercentage * 0.01f);
            while (riverBudget > 0 && riverOrigins.Count > 0)
            {
                int index = Random.Range(0, riverOrigins.Count);
                int lastIndex = riverOrigins.Count - 1;
                UgfCell origin = riverOrigins[index];
                riverOrigins[index] = riverOrigins[lastIndex];
                riverOrigins.RemoveAt(lastIndex);

                if (!origin.HasRiver)
                {
                    bool isValidOrigin = true;
                    for (HexDirection d = HexDirection.Ne; d <= HexDirection.Nw; d++)
                    {
                        UgfCell neighbor = origin.GetNeighbor(d);
                        if (neighbor && 
                            (neighbor.HasRiver || neighbor.IsUnderwater))
                        {
                            isValidOrigin = false;
                            break;
                        }
                    }

                    if (isValidOrigin) riverBudget -= CreateRiver(origin);
                }
            }

            if (riverBudget > 0)
                Debug.LogWarning("Failed to use up river budget.");

            ListPool<UgfCell>.Add(riverOrigins);
        }

        private int CreateRiver(UgfCell origin)
        {
            int length = 1;
            UgfCell cell = origin;
            HexDirection direction = HexDirection.Ne;
            while (!cell.IsUnderwater)
            {
                int minNeighborElevation = short.MaxValue;
                _flowDirections.Clear();
                for (HexDirection d = HexDirection.Ne; d <= HexDirection.Nw; d++)
                {
                    UgfCell neighbor = cell.GetNeighbor(d);
                    if (!neighbor) continue;

                    if (neighbor.Elevation < minNeighborElevation) minNeighborElevation = neighbor.Elevation;

                    if (neighbor == origin || neighbor.HasIncomingRiver) continue;

                    int delta = neighbor.Elevation - cell.Elevation;
                    if (delta > 0) continue;

                    if (neighbor.HasOutgoingRiver)
                    {
                        cell.SetOutgoingRiver(d);
                        return length;
                    }

                    if (delta < 0)
                    {
                        _flowDirections.Add(d);
                        _flowDirections.Add(d);
                        _flowDirections.Add(d);
                    }

                    if (
                        length == 1 ||
                        (d != direction.Next2() && d != direction.Previous2())
                    )
                        _flowDirections.Add(d);

                    _flowDirections.Add(d);
                }

                if (_flowDirections.Count == 0)
                {
                    if (length == 1) return 0;

                    if (minNeighborElevation >= cell.Elevation)
                    {
                        cell.WaterLevel = (short)minNeighborElevation;
                        if (minNeighborElevation == cell.Elevation) 
                            cell.Elevation = (short)(minNeighborElevation - 1);
                    }

                    break;
                }

                direction = _flowDirections[Random.Range(0, _flowDirections.Count)];
                cell.SetOutgoingRiver(direction);
                length += 1;

                if (
                    minNeighborElevation >= cell.Elevation &&
                    Random.value < ExtraLakeProbability
                )
                {
                    cell.WaterLevel = cell.Elevation;
                    cell.Elevation -= 1;
                }

                cell = cell.GetNeighbor(direction);
            }

            return length;
        }

        private void SetTerrainType()
        {
            _temperatureJitterChannel = Random.Range(0, 4);

            for (int i = 0; i < CellCount; i++)
            {
                UgfCell cell = Grid.GetCell(i) as UgfCell;
                float temperature = DetermineTemperature(cell);
                float moisture = _climate[i].Moisture;
                if (!cell!.IsUnderwater)
                {
                    byte t = 0;
                    for (; t < TemperatureBands.Length; t++)
                        if (temperature < TemperatureBands[t])
                            break;

                    byte m = 0;
                    for (; m < MoistureBands.Length; m++)
                        if (moisture < MoistureBands[m])
                            break;


                    cell.TerrainType = (byte)(12 - t * 4 + m);
                }
                else
                {
                    byte terrain;
                    if (cell.Elevation == WaterLevel - 1)
                    {
                        int cliffs = 0, slopes = 0;
                        for (
                            HexDirection d = HexDirection.Ne;
                            d <= HexDirection.Nw;
                            d++
                        )
                        {
                            UgfCell neighbor = cell.GetNeighbor(d);
                            if (!neighbor) continue;

                            int delta = neighbor.Elevation - cell.WaterLevel;
                            if (delta == 0)
                            {
                                slopes += 1;
                            }
                            else if (delta > 0)
                            {
                                cliffs += 1;
                            }
                        }

                        if (cliffs + slopes > 3)
                            terrain = 0;
                        else if (cliffs > 0)
                            terrain = 4;
                        else if (slopes > 0)
                            terrain = 8;
                        else
                            terrain = 12;
                    }
                    else if (cell.Elevation >= WaterLevel)
                    {
                        terrain = 0;
                    }
                    else if (cell.Elevation < 2)
                    {
                        terrain = 4;
                    }
                    else
                    {
                        terrain = 8;
                    }

                    if (terrain == 0 && temperature < TemperatureBands[0])
                        terrain = 12;

                    cell.TerrainType = terrain;
                }
            }
        }

        private float DetermineTemperature(UgfCell cell)
        {
            float latitude = (float)cell.Coordinates.Z / CellCountZ;
            if (Hemisphere == HemisphereMode.Both)
            {
                latitude *= 2f;
                if (latitude > 1f) latitude = 2f - latitude;
            }
            else if (Hemisphere == HemisphereMode.North)
            {
                latitude = 1f - latitude;
            }

            float temperature =
                Mathf.LerpUnclamped(LowTemperature, HighTemperature, latitude);

            temperature *= 1f - (cell.ViewElevation - WaterLevel) /
                (ElevationMaximum - WaterLevel + 1f);

            float jitter =
                HexMetrics.SampleNoise(cell.Position * 0.1f)[_temperatureJitterChannel];

            temperature += (jitter * 2f - 1f) * TemperatureJitter;

            return temperature;
        }

        private UgfCell GetRandomCell(MapRegion region)
        {
            int x = Random.Range(region.XMin, region.XMax);
            int z = Random.Range(region.ZMin, region.ZMax);

            return Grid.GetCell(x, z) as UgfCell;
        }

        private struct MapRegion
        {
            public int XMin, XMax, ZMin, ZMax;
        }

        private struct ClimateData
        {
            public float Clouds, Moisture;
        }
    }
}