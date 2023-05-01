#region

using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.HexMap.Utilities;
using UnityEngine;

#endregion

namespace Secyud.Ugf.HexMap.Generator
{
    /// <summary>
    ///     Component that generates hex maps.
    /// </summary>
    public class HexMapGenerator : ITransient, IHexMapGenerator
    {
        private static readonly float[] TemperatureBands = { 0.1f, 0.3f, 0.6f };

        private static readonly float[] MoistureBands = { 0.12f, 0.28f, 0.85f };

        private readonly List<HexDirection> _flowDirections = new();

        private int _cellCount, _landCells;

        private List<ClimateData> _climate = new();

        private HexGrid _grid;
        private List<ClimateData> _nextClimate = new();

        private List<MapRegion> _regions;

        private HexCellPriorityQueue _searchFrontier;

        private int _searchFrontierPhase;

        private int _temperatureJitterChannel;
        public HexMapGeneratorParameter Parameter { get; set; }

        /// <summary>
        ///     Generate a random hex map.
        /// </summary>
        /// <param name="grid">grid to generate map</param>
        /// <param name="x">X size of the map.</param>
        /// <param name="z">Z size of the map.</param>
        /// <param name="wrapping">Whether east-west wrapping is enabled.</param>
        public void GenerateMap(HexGrid grid, int x, int z, bool wrapping)
        {
            Parameter ??= new HexMapGeneratorParameter();
            _grid = grid;

            var originalRandomState = Random.state;

            Random.InitState(Parameter.Seed);

            _cellCount = x * z;
            _grid.CreateMap(x, z, wrapping);
            _searchFrontier ??= new HexCellPriorityQueue();

            for (var i = 0; i < _cellCount; i++) _grid.GetCell(i).WaterLevel = Parameter.WaterLevel;

            CreateRegions();
            CreateLand();
            ErodeLand();
            CreateClimate();
            CreateRivers();
            SetTerrainType();
            for (var i = 0; i < _cellCount; i++)
                _grid.GetCell(i).SearchPhase = 0;

            Random.state = originalRandomState;
        }

        private void CreateRegions()
        {
            if (_regions == null)
                _regions = new List<MapRegion>();
            else
                _regions.Clear();

            var borderX = _grid.Wrapping ? Parameter.RegionBorder : Parameter.MapBorderX;
            MapRegion region;
            switch (Parameter.RegionCount)
            {
                default:
                    if (_grid.Wrapping) borderX = 0;

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
                        if (_grid.Wrapping) borderX = 0;

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
            var landBudget = Mathf.RoundToInt(_cellCount * Parameter.LandPercentage * 0.01f);
            _landCells = landBudget;
            for (var guard = 0; guard < 10000; guard++)
            {
                var sink = Random.value < Parameter.SinkProbability;
                foreach (var region in _regions)
                {
                    var chunkSize = Random.Range(Parameter.ChunkSizeMin, Parameter.ChunkSizeMax - 1);
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
            var firstCell = GetRandomCell(region);
            firstCell.SearchPhase = _searchFrontierPhase;
            firstCell.Distance = 0;
            firstCell.SearchHeuristic = 0;
            _searchFrontier.Enqueue(firstCell);
            var center = firstCell.Coordinates;

            var rise = Random.value < Parameter.HighRiseProbability ? 2 : 1;
            var size = 0;
            while (size < chunkSize && _searchFrontier.Count > 0)
            {
                var current = _searchFrontier.Dequeue();
                var originalElevation = current.Elevation;
                var newElevation = originalElevation + rise;
                if (newElevation > Parameter.ElevationMaximum) continue;

                current.Elevation = newElevation;
                if (
                    originalElevation < Parameter.WaterLevel &&
                    newElevation >= Parameter.WaterLevel && --budget == 0
                )
                    break;

                size += 1;

                for (var d = HexDirection.Ne; d <= HexDirection.Nw; d++)
                {
                    var neighbor = current.GetNeighbor(d);
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
            var firstCell = GetRandomCell(region);
            firstCell.SearchPhase = _searchFrontierPhase;
            firstCell.Distance = 0;
            firstCell.SearchHeuristic = 0;
            _searchFrontier.Enqueue(firstCell);
            var center = firstCell.Coordinates;

            var sink = Random.value < Parameter.HighRiseProbability ? 2 : 1;
            var size = 0;
            while (size < chunkSize && _searchFrontier.Count > 0)
            {
                var current = _searchFrontier.Dequeue();
                var originalElevation = current.Elevation;
                var newElevation = current.Elevation - sink;
                if (newElevation < Parameter.ElevationMinimum) continue;

                current.Elevation = newElevation;
                if (
                    originalElevation >= Parameter.WaterLevel &&
                    newElevation < Parameter.WaterLevel
                )
                    budget += 1;

                size += 1;

                for (var d = HexDirection.Ne; d <= HexDirection.Nw; d++)
                {
                    var neighbor = current.GetNeighbor(d);
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
            var erodibleCells = ListPool<HexCell>.Get();
            for (var i = 0; i < _cellCount; i++)
            {
                var cell = _grid.GetCell(i);
                if (IsErodible(cell)) erodibleCells.Add(cell);
            }

            var targetErodibleCount =
                (int)(erodibleCells.Count * (100 - Parameter.ErosionPercentage) * 0.01f);

            while (erodibleCells.Count > targetErodibleCount)
            {
                var index = Random.Range(0, erodibleCells.Count);
                var cell = erodibleCells[index];
                var targetCell = GetErosionTarget(cell);

                cell.Elevation -= 1;
                targetCell.Elevation += 1;

                if (!IsErodible(cell))
                {
                    erodibleCells[index] = erodibleCells[^1];
                    erodibleCells.RemoveAt(erodibleCells.Count - 1);
                }

                for (var d = HexDirection.Ne; d <= HexDirection.Nw; d++)
                {
                    var neighbor = cell.GetNeighbor(d);
                    if (
                        neighbor && neighbor.Elevation == cell.Elevation + 2 &&
                        !erodibleCells.Contains(neighbor)
                    )
                        erodibleCells.Add(neighbor);
                }

                if (IsErodible(targetCell) && !erodibleCells.Contains(targetCell)) erodibleCells.Add(targetCell);

                for (var d = HexDirection.Ne; d <= HexDirection.Nw; d++)
                {
                    var neighbor = targetCell.GetNeighbor(d);
                    if (
                        neighbor && neighbor != cell &&
                        neighbor.Elevation == targetCell.Elevation + 1 &&
                        !IsErodible(neighbor)
                    )
                        erodibleCells.Remove(neighbor);
                }
            }

            ListPool<HexCell>.Add(erodibleCells);
        }

        private bool IsErodible(HexCell cell)
        {
            var erodibleElevation = cell.Elevation - 2;
            for (var d = HexDirection.Ne; d <= HexDirection.Nw; d++)
            {
                var neighbor = cell.GetNeighbor(d);
                if (neighbor && neighbor.Elevation <= erodibleElevation) return true;
            }

            return false;
        }

        private HexCell GetErosionTarget(HexCell cell)
        {
            var candidates = ListPool<HexCell>.Get();
            var erodibleElevation = cell.Elevation - 2;
            for (var d = HexDirection.Ne; d <= HexDirection.Nw; d++)
            {
                var neighbor = cell.GetNeighbor(d);
                if (neighbor && neighbor.Elevation <= erodibleElevation) candidates.Add(neighbor);
            }

            var target = candidates[Random.Range(0, candidates.Count)];
            ListPool<HexCell>.Add(candidates);
            return target;
        }

        private void CreateClimate()
        {
            _climate.Clear();
            _nextClimate.Clear();
            var initialData = new ClimateData
            {
                Moisture = Parameter.StartingMoisture
            };
            var clearData = new ClimateData();
            for (var i = 0; i < _cellCount; i++)
            {
                _climate.Add(initialData);
                _nextClimate.Add(clearData);
            }

            for (var cycle = 0; cycle < 40; cycle++)
            {
                for (var i = 0; i < _cellCount; i++) EvolveClimate(i);

                (_climate, _nextClimate) = (_nextClimate, _climate);
            }
        }

        private void EvolveClimate(int cellIndex)
        {
            var cell = _grid.GetCell(cellIndex);
            var cellClimate = _climate[cellIndex];

            if (cell.IsUnderwater)
            {
                cellClimate.Moisture = 1f;
                cellClimate.Clouds += Parameter.EvaporationFactor;
            }
            else
            {
                var evaporation = cellClimate.Moisture * Parameter.EvaporationFactor;
                cellClimate.Moisture -= evaporation;
                cellClimate.Clouds += evaporation;
            }

            var precipitation = cellClimate.Clouds * Parameter.PrecipitationFactor;
            cellClimate.Clouds -= precipitation;
            cellClimate.Moisture += precipitation;

            var cloudMaximum = 1f - cell.ViewElevation / (Parameter.ElevationMaximum + 1f);
            if (cellClimate.Clouds > cloudMaximum)
            {
                cellClimate.Moisture += cellClimate.Clouds - cloudMaximum;
                cellClimate.Clouds = cloudMaximum;
            }

            var mainDispersalDirection = Parameter.WindDirection.Opposite();
            var cloudDispersal = cellClimate.Clouds * (1f / (5f + Parameter.WindStrength));
            var runoff = cellClimate.Moisture * Parameter.RunoffFactor * (1f / 6f);
            var seepage = cellClimate.Moisture * Parameter.SeepageFactor * (1f / 6f);
            for (var d = HexDirection.Ne; d <= HexDirection.Nw; d++)
            {
                var neighbor = cell.GetNeighbor(d);
                if (!neighbor) continue;

                var neighborClimate = _nextClimate[neighbor.Index];
                if (d == mainDispersalDirection)
                    neighborClimate.Clouds += cloudDispersal * Parameter.WindStrength;
                else
                    neighborClimate.Clouds += cloudDispersal;

                var elevationDelta = neighbor.ViewElevation - cell.ViewElevation;
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

            var nextCellClimate = _nextClimate[cellIndex];
            nextCellClimate.Moisture += cellClimate.Moisture;
            if (nextCellClimate.Moisture > 1f) nextCellClimate.Moisture = 1f;

            _nextClimate[cellIndex] = nextCellClimate;
            _climate[cellIndex] = new ClimateData();
        }

        private void CreateRivers()
        {
            var riverOrigins = ListPool<HexCell>.Get();
            for (var i = 0; i < _cellCount; i++)
            {
                var cell = _grid.GetCell(i);
                if (cell.IsUnderwater) continue;

                var data = _climate[i];
                var weight =
                    data.Moisture * (cell.Elevation - Parameter.WaterLevel) /
                    (Parameter.ElevationMaximum - Parameter.WaterLevel);
                if (weight > 0.75f)
                {
                    riverOrigins.Add(cell);
                    riverOrigins.Add(cell);
                }

                if (weight > 0.5f) riverOrigins.Add(cell);

                if (weight > 0.25f) riverOrigins.Add(cell);
            }

            var riverBudget = Mathf.RoundToInt(_landCells * Parameter.RiverPercentage * 0.01f);
            while (riverBudget > 0 && riverOrigins.Count > 0)
            {
                var index = Random.Range(0, riverOrigins.Count);
                var lastIndex = riverOrigins.Count - 1;
                var origin = riverOrigins[index];
                riverOrigins[index] = riverOrigins[lastIndex];
                riverOrigins.RemoveAt(lastIndex);

                if (!origin.HasRiver)
                {
                    var isValidOrigin = true;
                    for (var d = HexDirection.Ne; d <= HexDirection.Nw; d++)
                    {
                        var neighbor = origin.GetNeighbor(d);
                        if (neighbor && (neighbor.HasRiver || neighbor.IsUnderwater))
                        {
                            isValidOrigin = false;
                            break;
                        }
                    }

                    if (isValidOrigin) riverBudget -= CreateRiver(origin);
                }
            }

            if (riverBudget > 0) Debug.LogWarning("Failed to use up river budget.");

            ListPool<HexCell>.Add(riverOrigins);
        }

        private int CreateRiver(HexCell origin)
        {
            var length = 1;
            var cell = origin;
            var direction = HexDirection.Ne;
            while (!cell.IsUnderwater)
            {
                var minNeighborElevation = int.MaxValue;
                _flowDirections.Clear();
                for (var d = HexDirection.Ne; d <= HexDirection.Nw; d++)
                {
                    var neighbor = cell.GetNeighbor(d);
                    if (!neighbor) continue;

                    if (neighbor.Elevation < minNeighborElevation) minNeighborElevation = neighbor.Elevation;

                    if (neighbor == origin || neighbor.HasIncomingRiver) continue;

                    var delta = neighbor.Elevation - cell.Elevation;
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
                        cell.WaterLevel = minNeighborElevation;
                        if (minNeighborElevation == cell.Elevation) cell.Elevation = minNeighborElevation - 1;
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
            var rockDesertElevation =
                Parameter.ElevationMaximum - (Parameter.ElevationMaximum - Parameter.WaterLevel) / 2;

            for (var i = 0; i < _cellCount; i++)
            {
                var cell = _grid.GetCell(i);
                var temperature = DetermineTemperature(cell);
                var moisture = _climate[i].Moisture;
                if (!cell.IsUnderwater)
                {
                    byte t = 0;
                    for (; t < TemperatureBands.Length; t++)
                        if (temperature < TemperatureBands[t])
                            break;

                    byte m = 0;
                    for (; m < MoistureBands.Length; m++)
                        if (moisture < MoistureBands[m])
                            break;


                    cell.TerrainTypeIndex = (byte)(12 - t * 4 + m);
                }
                else
                {
                    byte terrain;
                    if (cell.Elevation == Parameter.WaterLevel - 1)
                    {
                        int cliffs = 0, slopes = 0;
                        for (
                            var d = HexDirection.Ne;
                            d <= HexDirection.Nw;
                            d++
                        )
                        {
                            var neighbor = cell.GetNeighbor(d);
                            if (!neighbor) continue;

                            var delta = neighbor.Elevation - cell.WaterLevel;
                            if (delta == 0)
                                slopes += 1;
                            else if (delta > 0) cliffs += 1;
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
                    else if (cell.Elevation >= Parameter.WaterLevel)
                    {
                        terrain = 0;
                    }
                    else if (cell.Elevation < 0)
                    {
                        terrain = 4;
                    }
                    else
                    {
                        terrain = 8;
                    }

                    if (terrain == 0 && temperature < TemperatureBands[0]) terrain = 12;

                    cell.TerrainTypeIndex = terrain;
                }
            }
        }

        private float DetermineTemperature(HexCell cell)
        {
            var latitude = (float)cell.Coordinates.Z / _grid.CellCountZ;
            if (Parameter.Hemisphere == HemisphereMode.Both)
            {
                latitude *= 2f;
                if (latitude > 1f) latitude = 2f - latitude;
            }
            else if (Parameter.Hemisphere == HemisphereMode.North)
            {
                latitude = 1f - latitude;
            }

            var temperature =
                Mathf.LerpUnclamped(Parameter.LowTemperature, Parameter.HighTemperature, latitude);

            temperature *= 1f - (cell.ViewElevation - Parameter.WaterLevel) /
                (Parameter.ElevationMaximum - Parameter.WaterLevel + 1f);

            var jitter =
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