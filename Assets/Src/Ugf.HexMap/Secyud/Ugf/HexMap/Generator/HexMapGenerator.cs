#region

using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;
using UnityEngine;

#endregion

namespace Secyud.Ugf.HexMap.Generator
{
    /// <summary>
    /// Component that generates hex maps.
    /// </summary>
    public class HexMapGenerator : ITransient, IHexMapGenerator
    {
        public HexMapGeneratorParameter Parameter { get; set; }

        private HexGrid _grid;

        private HexCellPriorityQueue _searchFrontier;

        private int _searchFrontierPhase;

        private int _cellCount, _landCells;

        private int _temperatureJitterChannel;

        private struct MapRegion
        {
            public int XMin, XMax, ZMin, ZMax;
        }

        private List<MapRegion> _regions;

        private struct ClimateData
        {
            public float Clouds, Moisture;
        }

        private List<ClimateData> _climate = new();
        private List<ClimateData> _nextClimate = new();

        private readonly List<HexDirection> _flowDirections = new();


        private static readonly float[] TemperatureBands = { 0.1f, 0.3f, 0.6f };

        private static readonly float[] MoistureBands = { 0.12f, 0.28f, 0.85f };

        private static readonly int[] Biomes =
        {
            0, 4, 4, 4,
            0, 2, 2, 2,
            0, 1, 1, 1,
            0, 1, 1, 1
        };

        /// <summary>
        /// Generate a random hex map.
        /// </summary>
        /// <param name="grid">grid to generate map</param>
        /// <param name="x">X size of the map.</param>
        /// <param name="z">Z size of the map.</param>
        /// <param name="wrapping">Whether east-west wrapping is enabled.</param>
        public void GenerateMap(HexGrid grid, int x, int z, bool wrapping)
        {
            Parameter ??= new();
            _grid = grid;

            Random.State originalRandomState = Random.state;

            Random.InitState(Parameter.Seed);

            _cellCount = x * z;
            _grid.CreateMap(x, z, wrapping);
            _searchFrontier ??= new HexCellPriorityQueue();

            for (int i = 0; i < _cellCount; i++)
            {
                _grid.GetCell(i).WaterLevel = Parameter.WaterLevel;
            }

            CreateRegions();
            CreateLand();
            ErodeLand();
            CreateClimate();
            CreateRivers();
            SetTerrainType();
            for (int i = 0; i < _cellCount; i++)
            {
                _grid.GetCell(i).SearchPhase = 0;
            }

            Random.state = originalRandomState;
        }

        private void CreateRegions()
        {
            if (_regions == null)
            {
                _regions = new List<MapRegion>();
            }
            else
            {
                _regions.Clear();
            }

            int borderX = _grid.Wrapping ? Parameter.RegionBorder : Parameter.MapBorderX;
            MapRegion region;
            switch (Parameter.RegionCount)
            {
                default:
                    if (_grid.Wrapping)
                    {
                        borderX = 0;
                    }

                    region.XMin = borderX;
                    region.XMax = _grid.CellCountX - borderX;
                    region.ZMin = Parameter.MapBorderZ;
                    region.ZMax = _grid.CellCountZ - Parameter.MapBorderZ;
                    _regions.Add(region);
                    break;
                case 2:
                    if (Random.value < 0.5f)
                    {
                        region.XMin = borderX;
                        region.XMax = _grid.CellCountX / 2 - Parameter.RegionBorder;
                        region.ZMin = Parameter.MapBorderZ;
                        region.ZMax = _grid.CellCountZ - Parameter.MapBorderZ;
                        _regions.Add(region);
                        region.XMin = _grid.CellCountX / 2 + Parameter.RegionBorder;
                        region.XMax = _grid.CellCountX - borderX;
                        _regions.Add(region);
                    }
                    else
                    {
                        if (_grid.Wrapping)
                        {
                            borderX = 0;
                        }

                        region.XMin = borderX;
                        region.XMax = _grid.CellCountX - borderX;
                        region.ZMin = Parameter.MapBorderZ;
                        region.ZMax = _grid.CellCountZ / 2 - Parameter.RegionBorder;
                        _regions.Add(region);
                        region.ZMin = _grid.CellCountZ / 2 + Parameter.RegionBorder;
                        region.ZMax = _grid.CellCountZ - Parameter.MapBorderZ;
                        _regions.Add(region);
                    }

                    break;
                case 3:
                    region.XMin = borderX;
                    region.XMax = _grid.CellCountX / 3 - Parameter.RegionBorder;
                    region.ZMin = Parameter.MapBorderZ;
                    region.ZMax = _grid.CellCountZ - Parameter.MapBorderZ;
                    _regions.Add(region);
                    region.XMin = _grid.CellCountX / 3 + Parameter.RegionBorder;
                    region.XMax = _grid.CellCountX * 2 / 3 - Parameter.RegionBorder;
                    _regions.Add(region);
                    region.XMin = _grid.CellCountX * 2 / 3 + Parameter.RegionBorder;
                    region.XMax = _grid.CellCountX - borderX;
                    _regions.Add(region);
                    break;
                case 4:
                    region.XMin = borderX;
                    region.XMax = _grid.CellCountX / 2 - Parameter.RegionBorder;
                    region.ZMin = Parameter.MapBorderZ;
                    region.ZMax = _grid.CellCountZ / 2 - Parameter.RegionBorder;
                    _regions.Add(region);
                    region.XMin = _grid.CellCountX / 2 + Parameter.RegionBorder;
                    region.XMax = _grid.CellCountX - borderX;
                    _regions.Add(region);
                    region.ZMin = _grid.CellCountZ / 2 + Parameter.RegionBorder;
                    region.ZMax = _grid.CellCountZ - Parameter.MapBorderZ;
                    _regions.Add(region);
                    region.XMin = borderX;
                    region.XMax = _grid.CellCountX / 2 - Parameter.RegionBorder;
                    _regions.Add(region);
                    break;
            }
        }

        private void CreateLand()
        {
            int landBudget = Mathf.RoundToInt(_cellCount * Parameter.LandPercentage * 0.01f);
            _landCells = landBudget;
            for (int guard = 0; guard < 10000; guard++)
            {
                bool sink = Random.value < Parameter.SinkProbability;
                foreach (var region in _regions)
                {
                    int chunkSize = Random.Range(Parameter.ChunkSizeMin, Parameter.ChunkSizeMax - 1);
                    if (sink)
                    {
                        landBudget = SinkTerrain(chunkSize, landBudget, region);
                    }
                    else
                    {
                        landBudget = RaiseTerrain(chunkSize, landBudget, region);
                        if (landBudget == 0)
                        {
                            return;
                        }
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
            HexCell firstCell = GetRandomCell(region);
            firstCell.SearchPhase = _searchFrontierPhase;
            firstCell.Distance = 0;
            firstCell.SearchHeuristic = 0;
            _searchFrontier.Enqueue(firstCell);
            HexCoordinates center = firstCell.Coordinates;

            int rise = Random.value < Parameter.HighRiseProbability ? 2 : 1;
            int size = 0;
            while (size < chunkSize && _searchFrontier.Count > 0)
            {
                HexCell current = _searchFrontier.Dequeue();
                int originalElevation = current.Elevation;
                int newElevation = originalElevation + rise;
                if (newElevation > Parameter.ElevationMaximum)
                {
                    continue;
                }

                current.Elevation = newElevation;
                if (
                    originalElevation < Parameter.WaterLevel &&
                    newElevation >= Parameter.WaterLevel && --budget == 0
                )
                {
                    break;
                }

                size += 1;

                for (HexDirection d = HexDirection.Ne; d <= HexDirection.Nw; d++)
                {
                    HexCell neighbor = current.GetNeighbor(d);
                    if (neighbor && neighbor.SearchPhase < _searchFrontierPhase)
                    {
                        neighbor.SearchPhase = _searchFrontierPhase;
                        neighbor.Distance = neighbor.Coordinates.DistanceTo(center);
                        neighbor.SearchHeuristic =
                            Random.value < Parameter.JitterProbability ? 1 : 0;
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
            HexCell firstCell = GetRandomCell(region);
            firstCell.SearchPhase = _searchFrontierPhase;
            firstCell.Distance = 0;
            firstCell.SearchHeuristic = 0;
            _searchFrontier.Enqueue(firstCell);
            HexCoordinates center = firstCell.Coordinates;

            int sink = Random.value < Parameter.HighRiseProbability ? 2 : 1;
            int size = 0;
            while (size < chunkSize && _searchFrontier.Count > 0)
            {
                HexCell current = _searchFrontier.Dequeue();
                int originalElevation = current.Elevation;
                int newElevation = current.Elevation - sink;
                if (newElevation < Parameter.ElevationMinimum)
                {
                    continue;
                }

                current.Elevation = newElevation;
                if (
                    originalElevation >= Parameter.WaterLevel &&
                    newElevation < Parameter.WaterLevel
                )
                {
                    budget += 1;
                }

                size += 1;

                for (HexDirection d = HexDirection.Ne; d <= HexDirection.Nw; d++)
                {
                    HexCell neighbor = current.GetNeighbor(d);
                    if (neighbor && neighbor.SearchPhase < _searchFrontierPhase)
                    {
                        neighbor.SearchPhase = _searchFrontierPhase;
                        neighbor.Distance = neighbor.Coordinates.DistanceTo(center);
                        neighbor.SearchHeuristic =
                            Random.value < Parameter.JitterProbability ? 1 : 0;
                        _searchFrontier.Enqueue(neighbor);
                    }
                }
            }

            _searchFrontier.Clear();
            return budget;
        }

        private void ErodeLand()
        {
            List<HexCell> erodibleCells = ListPool<HexCell>.Get();
            for (int i = 0; i < _cellCount; i++)
            {
                HexCell cell = _grid.GetCell(i);
                if (IsErodible(cell))
                {
                    erodibleCells.Add(cell);
                }
            }

            int targetErodibleCount =
                (int)(erodibleCells.Count * (100 - Parameter.ErosionPercentage) * 0.01f);

            while (erodibleCells.Count > targetErodibleCount)
            {
                int index = Random.Range(0, erodibleCells.Count);
                HexCell cell = erodibleCells[index];
                HexCell targetCell = GetErosionTarget(cell);

                cell.Elevation -= 1;
                targetCell.Elevation += 1;

                if (!IsErodible(cell))
                {
                    erodibleCells[index] = erodibleCells[^1];
                    erodibleCells.RemoveAt(erodibleCells.Count - 1);
                }

                for (HexDirection d = HexDirection.Ne; d <= HexDirection.Nw; d++)
                {
                    HexCell neighbor = cell.GetNeighbor(d);
                    if (
                        neighbor && neighbor.Elevation == cell.Elevation + 2 &&
                        !erodibleCells.Contains(neighbor)
                    )
                    {
                        erodibleCells.Add(neighbor);
                    }
                }

                if (IsErodible(targetCell) && !erodibleCells.Contains(targetCell))
                {
                    erodibleCells.Add(targetCell);
                }

                for (HexDirection d = HexDirection.Ne; d <= HexDirection.Nw; d++)
                {
                    HexCell neighbor = targetCell.GetNeighbor(d);
                    if (
                        neighbor && neighbor != cell &&
                        neighbor.Elevation == targetCell.Elevation + 1 &&
                        !IsErodible(neighbor)
                    )
                    {
                        erodibleCells.Remove(neighbor);
                    }
                }
            }

            ListPool<HexCell>.Add(erodibleCells);
        }

        private bool IsErodible(HexCell cell)
        {
            int erodibleElevation = cell.Elevation - 2;
            for (HexDirection d = HexDirection.Ne; d <= HexDirection.Nw; d++)
            {
                HexCell neighbor = cell.GetNeighbor(d);
                if (neighbor && neighbor.Elevation <= erodibleElevation)
                {
                    return true;
                }
            }

            return false;
        }

        private HexCell GetErosionTarget(HexCell cell)
        {
            List<HexCell> candidates = ListPool<HexCell>.Get();
            int erodibleElevation = cell.Elevation - 2;
            for (HexDirection d = HexDirection.Ne; d <= HexDirection.Nw; d++)
            {
                HexCell neighbor = cell.GetNeighbor(d);
                if (neighbor && neighbor.Elevation <= erodibleElevation)
                {
                    candidates.Add(neighbor);
                }
            }

            HexCell target = candidates[Random.Range(0, candidates.Count)];
            ListPool<HexCell>.Add(candidates);
            return target;
        }

        private void CreateClimate()
        {
            _climate.Clear();
            _nextClimate.Clear();
            ClimateData initialData = new ClimateData
            {
                Moisture = Parameter.StartingMoisture
            };
            ClimateData clearData = new ClimateData();
            for (int i = 0; i < _cellCount; i++)
            {
                _climate.Add(initialData);
                _nextClimate.Add(clearData);
            }

            for (int cycle = 0; cycle < 40; cycle++)
            {
                for (int i = 0; i < _cellCount; i++)
                {
                    EvolveClimate(i);
                }

                (_climate, _nextClimate) = (_nextClimate, _climate);
            }
        }

        private void EvolveClimate(int cellIndex)
        {
            HexCell cell = _grid.GetCell(cellIndex);
            ClimateData cellClimate = _climate[cellIndex];

            if (cell.IsUnderwater)
            {
                cellClimate.Moisture = 1f;
                cellClimate.Clouds += Parameter.EvaporationFactor;
            }
            else
            {
                float evaporation = cellClimate.Moisture * Parameter.EvaporationFactor;
                cellClimate.Moisture -= evaporation;
                cellClimate.Clouds += evaporation;
            }

            float precipitation = cellClimate.Clouds * Parameter.PrecipitationFactor;
            cellClimate.Clouds -= precipitation;
            cellClimate.Moisture += precipitation;

            float cloudMaximum = 1f - cell.ViewElevation / (Parameter.ElevationMaximum + 1f);
            if (cellClimate.Clouds > cloudMaximum)
            {
                cellClimate.Moisture += cellClimate.Clouds - cloudMaximum;
                cellClimate.Clouds = cloudMaximum;
            }

            HexDirection mainDispersalDirection = Parameter.WindDirection.Opposite();
            float cloudDispersal = cellClimate.Clouds * (1f / (5f + Parameter.WindStrength));
            float runoff = cellClimate.Moisture * Parameter.RunoffFactor * (1f / 6f);
            float seepage = cellClimate.Moisture * Parameter.SeepageFactor * (1f / 6f);
            for (HexDirection d = HexDirection.Ne; d <= HexDirection.Nw; d++)
            {
                HexCell neighbor = cell.GetNeighbor(d);
                if (!neighbor)
                {
                    continue;
                }

                ClimateData neighborClimate = _nextClimate[neighbor.Index];
                if (d == mainDispersalDirection)
                {
                    neighborClimate.Clouds += cloudDispersal * Parameter.WindStrength;
                }
                else
                {
                    neighborClimate.Clouds += cloudDispersal;
                }

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

            ClimateData nextCellClimate = _nextClimate[cellIndex];
            nextCellClimate.Moisture += cellClimate.Moisture;
            if (nextCellClimate.Moisture > 1f)
            {
                nextCellClimate.Moisture = 1f;
            }

            _nextClimate[cellIndex] = nextCellClimate;
            _climate[cellIndex] = new ClimateData();
        }

        private void CreateRivers()
        {
            List<HexCell> riverOrigins = ListPool<HexCell>.Get();
            for (int i = 0; i < _cellCount; i++)
            {
                HexCell cell = _grid.GetCell(i);
                if (cell.IsUnderwater)
                {
                    continue;
                }

                ClimateData data = _climate[i];
                float weight =
                    data.Moisture * (cell.Elevation - Parameter.WaterLevel) /
                    (Parameter.ElevationMaximum - Parameter.WaterLevel);
                if (weight > 0.75f)
                {
                    riverOrigins.Add(cell);
                    riverOrigins.Add(cell);
                }

                if (weight > 0.5f)
                {
                    riverOrigins.Add(cell);
                }

                if (weight > 0.25f)
                {
                    riverOrigins.Add(cell);
                }
            }

            int riverBudget = Mathf.RoundToInt(_landCells * Parameter.RiverPercentage * 0.01f);
            while (riverBudget > 0 && riverOrigins.Count > 0)
            {
                int index = Random.Range(0, riverOrigins.Count);
                int lastIndex = riverOrigins.Count - 1;
                HexCell origin = riverOrigins[index];
                riverOrigins[index] = riverOrigins[lastIndex];
                riverOrigins.RemoveAt(lastIndex);

                if (!origin.HasRiver)
                {
                    bool isValidOrigin = true;
                    for (HexDirection d = HexDirection.Ne; d <= HexDirection.Nw; d++)
                    {
                        HexCell neighbor = origin.GetNeighbor(d);
                        if (neighbor && (neighbor.HasRiver || neighbor.IsUnderwater))
                        {
                            isValidOrigin = false;
                            break;
                        }
                    }

                    if (isValidOrigin)
                    {
                        riverBudget -= CreateRiver(origin);
                    }
                }
            }

            if (riverBudget > 0)
            {
                Debug.LogWarning("Failed to use up river budget.");
            }

            ListPool<HexCell>.Add(riverOrigins);
        }

        private int CreateRiver(HexCell origin)
        {
            int length = 1;
            HexCell cell = origin;
            HexDirection direction = HexDirection.Ne;
            while (!cell.IsUnderwater)
            {
                int minNeighborElevation = int.MaxValue;
                _flowDirections.Clear();
                for (HexDirection d = HexDirection.Ne; d <= HexDirection.Nw; d++)
                {
                    HexCell neighbor = cell.GetNeighbor(d);
                    if (!neighbor)
                    {
                        continue;
                    }

                    if (neighbor.Elevation < minNeighborElevation)
                    {
                        minNeighborElevation = neighbor.Elevation;
                    }

                    if (neighbor == origin || neighbor.HasIncomingRiver)
                    {
                        continue;
                    }

                    int delta = neighbor.Elevation - cell.Elevation;
                    if (delta > 0)
                    {
                        continue;
                    }

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
                    {
                        _flowDirections.Add(d);
                    }

                    _flowDirections.Add(d);
                }

                if (_flowDirections.Count == 0)
                {
                    if (length == 1)
                    {
                        return 0;
                    }

                    if (minNeighborElevation >= cell.Elevation)
                    {
                        cell.WaterLevel = minNeighborElevation;
                        if (minNeighborElevation == cell.Elevation)
                        {
                            cell.Elevation = minNeighborElevation - 1;
                        }
                    }

                    break;
                }

                direction = _flowDirections[Random.Range(0, _flowDirections.Count)];
                cell.SetOutgoingRiver(direction);
                length += 1;

                if (
                    minNeighborElevation >= cell.Elevation &&
                    Random.value < Parameter.ExtraLakeProbability
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
            int rockDesertElevation =
                Parameter.ElevationMaximum - (Parameter.ElevationMaximum - Parameter.WaterLevel) / 2;

            for (int i = 0; i < _cellCount; i++)
            {
                HexCell cell = _grid.GetCell(i);
                float temperature = DetermineTemperature(cell);
                float moisture = _climate[i].Moisture;
                if (!cell.IsUnderwater)
                {
                    int t = 0;
                    for (; t < TemperatureBands.Length; t++)
                    {
                        if (temperature < TemperatureBands[t])
                        {
                            break;
                        }
                    }

                    int m = 0;
                    for (; m < MoistureBands.Length; m++)
                    {
                        if (moisture < MoistureBands[m])
                        {
                            break;
                        }
                    }

                    int cellBiome = Biomes[t * 4 + m];

                    if (cellBiome == 0)
                    {
                        if (cell.Elevation >= rockDesertElevation)
                        {
                            cellBiome = 3;
                        }
                    }
                    else if (cell.Elevation == Parameter.ElevationMaximum)
                    {
                        cellBiome = 4;
                    }

                    cell.TerrainTypeIndex = cellBiome;
                }
                else
                {
                    int terrain;
                    if (cell.Elevation == Parameter.WaterLevel - 1)
                    {
                        int cliffs = 0, slopes = 0;
                        for (
                            HexDirection d = HexDirection.Ne;
                            d <= HexDirection.Nw;
                            d++
                        )
                        {
                            HexCell neighbor = cell.GetNeighbor(d);
                            if (!neighbor)
                            {
                                continue;
                            }

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
                        {
                            terrain = 1;
                        }
                        else if (cliffs > 0)
                        {
                            terrain = 3;
                        }
                        else if (slopes > 0)
                        {
                            terrain = 0;
                        }
                        else
                        {
                            terrain = 1;
                        }
                    }
                    else if (cell.Elevation >= Parameter.WaterLevel)
                    {
                        terrain = 1;
                    }
                    else if (cell.Elevation < 0)
                    {
                        terrain = 3;
                    }
                    else
                    {
                        terrain = 2;
                    }

                    if (terrain == 1 && temperature < TemperatureBands[0])
                    {
                        terrain = 2;
                    }

                    cell.TerrainTypeIndex = terrain;
                }
            }
        }

        private float DetermineTemperature(HexCell cell)
        {
            float latitude = (float)cell.Coordinates.Z / _grid.CellCountZ;
            if (Parameter.Hemisphere == HemisphereMode.Both)
            {
                latitude *= 2f;
                if (latitude > 1f)
                {
                    latitude = 2f - latitude;
                }
            }
            else if (Parameter.Hemisphere == HemisphereMode.North)
            {
                latitude = 1f - latitude;
            }

            float temperature =
                Mathf.LerpUnclamped(Parameter.LowTemperature, Parameter.HighTemperature, latitude);

            temperature *= 1f - (cell.ViewElevation - Parameter.WaterLevel) /
                (Parameter.ElevationMaximum - Parameter.WaterLevel + 1f);

            float jitter =
                HexMetrics.SampleNoise(cell.Position * 0.1f)[_temperatureJitterChannel];

            temperature += (jitter * 2f - 1f) * Parameter.TemperatureJitter;

            return temperature;
        }

        private HexCell GetRandomCell(MapRegion region)
        {
            return _grid.GetCell(
                Random.Range(region.XMin, region.XMax),
                Random.Range(region.ZMin, region.ZMax)
            );
        }
    }
}