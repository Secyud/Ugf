#region

#endregion

using Secyud.Ugf.HexUtilities;

namespace Secyud.Ugf.UgfHexMapGenerator
{
	public class HexMapGeneratorParameter
	{
		public int Seed { get; set; }

		public HemisphereMode Hemisphere { get; set; }

		public HexDirection WindDirection { get; set; } = HexDirection.Nw;


		public int ChunkSizeMin { get; set; } = 30;

		public int ChunkSizeMax { get; set; } = 100;

		public int RiverPercentage { get; set; } = 10;

		public int LandPercentage { get; set; } = 50;

		public int ErosionPercentage { get; set; } = 50;

		public int ElevationMinimum { get; set; } = -2;

		public int WaterLevel => 3;

		public int ElevationMaximum { get; set; } = 8;

		// min 0 max 10
		public int MapBorderX { get; set; } = 5;

		public int MapBorderZ { get; set; } = 5;

		public int RegionBorder { get; set; } = 5;


		public int RegionCount { get; set; } = 1;

		public float WindStrength { get; set; } = 4f;

		public float JitterProbability { get; set; } = 0.25f;

		public float SinkProbability { get; set; } = 0.2f;

		// min 0 max 1
		public float HighRiseProbability { get; set; } = 0.25f;

		public float StartingMoisture { get; set; } = 0.1f;

		public float EvaporationFactor { get; set; } = 0.5f;

		public float PrecipitationFactor { get; set; } = 0.25f;

		public float RunoffFactor { get; set; } = 0.25f;

		public float SeepageFactor { get; set; } = 0.125f;

		public float ExtraLakeProbability { get; set; } = 0.25f;

		public float LowTemperature { get; set; } = 0f;

		public float HighTemperature { get; set; } = 1f;

		public float TemperatureJitter { get; set; } = 0.1f;
	}
}