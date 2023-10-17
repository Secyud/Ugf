#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.HexUtilities
{
	/// <summary>
	///     Constant metrics and utility methods for the hex map.
	/// </summary>
	public static class HexMetrics
	{
		public const float Srt = 1.732050807568877f;

		/// <summary>
		///     Ratio of outer to inner radius of a hex cell.
		/// </summary>
		public const float OuterToInner = 0.866025404f;

		/// <summary>
		///     Ratio of inner to outer radius of a hex cell.
		/// </summary>
		public const float InnerToOuter = 1f / OuterToInner;

		/// <summary>
		///     Outer radius of a hex cell.
		/// </summary>
		public const float OuterRadius = 10f;

		/// <summary>
		///     Inner radius of a hex cell.
		/// </summary>
		public const float InnerRadius = OuterRadius * OuterToInner;

		public const float InnerDiameter = InnerRadius * 2;

		/// <summary>
		///     Factor of the solid uniform region inside a hex cell.
		/// </summary>
		public const float SolidFactor = 0.8f;

		/// <summary>
		///     Factor of the blending region inside a hex cell.
		/// </summary>
		public const float BlendFactor = 1f - SolidFactor;

		/// <summary>
		///     Factor of the solid uniform water region inside a hex cell.
		/// </summary>
		public const float WaterFactor = 0.6f;

		/// <summary>
		///     Factor of the water blending region inside a hex cell.
		/// </summary>
		public const float WaterBlendFactor = 1f - WaterFactor;

		/// <summary>
		///     Height of a single elevation step.
		/// </summary>
		public const float ElevationStep = 3f;

		/// <summary>
		///     Amount of terrace levels per slope.
		/// </summary>
		public const int TerracesPerSlope = 2;

		/// <summary>
		///     Amount of terraces steps per slope needed for <see cref="TerracesPerSlope" />.
		/// </summary>
		public const int TerraceSteps = TerracesPerSlope * 2 + 1;

		/// <summary>
		///     Amount of horizontal terrace steps per slope.
		/// </summary>
		public const float HorizontalTerraceStepSize = 1f / TerraceSteps;

		/// <summary>
		///     Amount of vertical terrace steps per slope.
		/// </summary>
		public const float VerticalTerraceStepSize = 1f / (TerracesPerSlope + 1);

		/// <summary>
		///     Strength of cell position perturbation.
		/// </summary>
		public const float CellPerturbStrength = 4f;

		/// <summary>
		///     Strength of vertical elevation perturbation.
		/// </summary>
		public const float ElevationPerturbStrength = 1.5f;

		/// <summary>
		///     Offset for stream bed elevation.
		/// </summary>
		public const float StreamBedElevationOffset = -1.75f;

		/// <summary>
		///     Offset for water elevation.
		/// </summary>
		public const float WaterElevationOffset = -0.5f;

		/// <summary>
		///     Height of walls.
		/// </summary>
		public const float WallHeight = 4f;

		/// <summary>
		///     Vertical wall offset, negative to prevent them from floating above the surface.
		/// </summary>
		public const float WallYOffset = -1f;

		/// <summary>
		///     Wall thickness.
		/// </summary>
		public const float WallThickness = 0.75f;

		/// <summary>
		///     Wall elevation offset, matching <see cref="VerticalTerraceStepSize" />.
		/// </summary>
		public const float WallElevationOffset = VerticalTerraceStepSize;

		/// <summary>
		///     Probability threshold for wall towers.
		/// </summary>
		public const float WallTowerThreshold = 0.5f;

		/// <summary>
		///     Length at which the bridge model is designed.
		/// </summary>
		public const float BridgeDesignLength = 7f;

		/// <summary>
		///     World scale of the noise.
		/// </summary>
		public const float NoiseScale = 0.003f;

		/// <summary>
		///     Hex grid chunk size in the X dimension.
		/// </summary>
		public const int ChunkSizeX = 4;

		/// <summary>
		///     Hex grid chunk size in the Z dimension.
		/// </summary>
		public const int ChunkSizeZ = 4;

		/// <summary>
		///     Size of the hash grid.
		/// </summary>
		public const int HashGridSize = 256;

		/// <summary>
		///     World scale of the hash grid.
		/// </summary>
		public const float HashGridScale = 0.25f;

		private static HexHash[] _hashGrid;

		private static readonly Vector3[] Corners =
		{
			new(0f, 0f, OuterRadius),
			new(InnerRadius, 0f, 0.5f * OuterRadius),
			new(InnerRadius, 0f, -0.5f * OuterRadius),
			new(0f, 0f, -OuterRadius),
			new(-InnerRadius, 0f, -0.5f * OuterRadius),
			new(-InnerRadius, 0f, 0.5f * OuterRadius),
			new(0f, 0f, OuterRadius)
		};

		private static readonly float[][] FeatureThresholds =
		{
			new[] {0.0f, 0.0f, 0.4f},
			new[] {0.0f, 0.4f, 0.6f},
			new[] {0.4f, 0.6f, 0.8f}
		};

		/// <summary>
		///     Texture used for sampling noise.
		/// </summary>
		public static Texture2D NoiseSource;

		/// <summary>
		///     Sample the noise texture.
		/// </summary>
		/// <param name="position">Sample position.</param>
		/// <returns>Four-component noise sample.</returns>
		public static Vector4 SampleNoise(Vector3 position)
		{
			Vector4 sample = NoiseSource.GetPixelBilinear(
				position.x * NoiseScale,
				position.z * NoiseScale
			);

			return sample;
		}

		/// <summary>
		///     Initialize the hash grid.
		/// </summary>
		/// <param name="seed">Seed to use for initialization.</param>
		public static void InitializeHashGrid(int seed)
		{
			_hashGrid = new HexHash[HashGridSize * HashGridSize];
			Random.State currentState = Random.state;
			Random.InitState(seed);
			for (int i = 0; i < _hashGrid.Length; i++) _hashGrid[i] = HexHash.Create();

			Random.state = currentState;
		}

		/// <summary>
		///     Sample the hash grid.
		/// </summary>
		/// <param name="position">Sample position</param>
		/// <returns>Sampled <see cref="HexHash" />.</returns>
		public static HexHash SampleHashGrid(Vector3 position)
		{
			int x = (int)(position.x * HashGridScale) % HashGridSize;
			if (x < 0) x += HashGridSize;

			int z = (int)(position.z * HashGridScale) % HashGridSize;
			if (z < 0) z += HashGridSize;

			return _hashGrid[x + z * HashGridSize];
		}

		/// <summary>
		///     Get the feature threshold levels.
		/// </summary>
		/// <param name="level">Feature level.</param>
		/// <returns>Array containing the thresholds.</returns>
		public static float[] GetFeatureThresholds(int level)
		{
			return FeatureThresholds[level];
		}

		/// <summary>
		///     Get the first outer cell corner for a direction.
		/// </summary>
		/// <param name="direction">The desired direction.</param>
		/// <returns>The corner on the counter-clockwise side.</returns>
		public static Vector3 GetFirstCorner(HexDirection direction)
		{
			return Corners[(int)direction];
		}

		/// <summary>
		///     Get the second outer cell corner for a direction.
		/// </summary>
		/// <param name="direction">The desired direction.</param>
		/// <returns>The corner on the clockwise side.</returns>
		public static Vector3 GetSecondCorner(HexDirection direction)
		{
			return Corners[(int)direction + 1];
		}

		/// <summary>
		///     Get the first inner solid cell corner for a direction.
		/// </summary>
		/// <param name="direction">The desired direction.</param>
		/// <returns>The corner on the counter-clockwise side.</returns>
		public static Vector3 GetFirstSolidCorner(HexDirection direction)
		{
			return Corners[(int)direction] * SolidFactor;
		}

		/// <summary>
		///     Get the second inner solid cell corner for a direction.
		/// </summary>
		/// <param name="direction">The desired direction.</param>
		/// <returns>The corner on the clockwise side.</returns>
		public static Vector3 GetSecondSolidCorner(HexDirection direction)
		{
			return Corners[(int)direction + 1] * SolidFactor;
		}

		/// <summary>
		///     Get the middle of the inner solid cell edge for a direction.
		/// </summary>
		/// <param name="direction">The desired direction.</param>
		/// <returns>The position in between the two inner solid cell corners.</returns>
		public static Vector3 GetSolidEdgeMiddle(HexDirection direction)
		{
			return (Corners[(int)direction] + Corners[(int)direction + 1]) *
				(0.5f * SolidFactor);
		}

		/// <summary>
		///     Get the first inner water cell corner for a direction.
		/// </summary>
		/// <param name="direction">The desired direction.</param>
		/// <returns>The corner on the counter-clockwise side.</returns>
		public static Vector3 GetFirstWaterCorner(HexDirection direction)
		{
			return Corners[(int)direction] * WaterFactor;
		}

		/// <summary>
		///     Get the second inner water cell corner for a direction.
		/// </summary>
		/// <param name="direction">The desired direction.</param>
		/// <returns>The corner on the clockwise side.</returns>
		public static Vector3 GetSecondWaterCorner(HexDirection direction)
		{
			return Corners[(int)direction + 1] * WaterFactor;
		}

		/// <summary>
		///     Get the vector needed to bridge to the next cell for a given direction.
		/// </summary>
		/// <param name="direction">The desired direction.</param>
		/// <returns>The bridge vector.</returns>
		public static Vector3 GetBridge(HexDirection direction)
		{
			return (Corners[(int)direction] + Corners[(int)direction + 1]) * BlendFactor;
		}

		/// <summary>
		///     Get the vector needed to bridge to the next water cell for a given direction.
		/// </summary>
		/// <param name="direction">The desired direction.</param>
		/// <returns>The bridge vector.</returns>
		public static Vector3 GetWaterBridge(HexDirection direction)
		{
			return (Corners[(int)direction] + Corners[(int)direction + 1]) * WaterBlendFactor;
		}

		/// <summary>
		///     Interpolate a position along a terraced edge.
		/// </summary>
		/// <param name="a">Start position.</param>
		/// <param name="b">End position.</param>
		/// <param name="step">Terrace interpolation step.</param>
		/// <returns>The position found by applying terrace interpolation.</returns>
		public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
		{
			float h = step * HorizontalTerraceStepSize;
			a.x += (b.x - a.x) * h;
			a.z += (b.z - a.z) * h;
			// ReSharper disable once PossibleLossOfFraction
			float v = (step + 1) / 2 * VerticalTerraceStepSize;
			a.y += (b.y - a.y) * v;
			return a;
		}

		/// <summary>
		///     Interpolate a color along a terraced edge.
		/// </summary>
		/// <param name="a">Start color.</param>
		/// <param name="b">End color.</param>
		/// <param name="step">Terrace interpolation step.</param>
		/// <returns>The color found by applying terrace interpolation.</returns>
		public static Color TerraceLerp(Color a, Color b, int step)
		{
			float h = step * HorizontalTerraceStepSize;
			return Color.Lerp(a, b, h);
		}

		/// <summary>
		///     Interpolate a position along a wall.
		/// </summary>
		/// <param name="near">Near position.</param>
		/// <param name="far">Far position.</param>
		/// <returns>The middle position with appropriate Y coordinate.</returns>
		public static Vector3 WallLerp(Vector3 near, Vector3 far)
		{
			near.x += (far.x - near.x) * 0.5f;
			near.z += (far.z - near.z) * 0.5f;
			float v =
				near.y < far.y ? WallElevationOffset : 1f - WallElevationOffset;
			near.y += (far.y - near.y) * v + WallYOffset;
			return near;
		}

		/// <summary>
		///     Apply wall thickness.
		/// </summary>
		/// <param name="near">Near position.</param>
		/// <param name="far">Far position.</param>
		/// <returns>Position taking wall thickness into account.</returns>
		public static Vector3 WallThicknessOffset(Vector3 near, Vector3 far)
		{
			Vector3 offset;
			offset.x = far.x - near.x;
			offset.y = 0f;
			offset.z = far.z - near.z;
			return offset.normalized * (WallThickness * 0.5f);
		}

		/// <summary>
		///     Determine the <see cref="HexEdgeType" /> based on two elevations.
		/// </summary>
		/// <param name="elevation1">First elevation.</param>
		/// <param name="elevation2">Second elevation.</param>
		/// <returns>Matching <see cref="HexEdgeType" />.</returns>
		public static HexEdgeType GetEdgeType(int elevation1, int elevation2)
		{
			if (elevation1 == elevation2) return HexEdgeType.Flat;

			int delta = elevation2 - elevation1;
			if (delta == 1 || delta == -1) return HexEdgeType.Slope;

			return HexEdgeType.Cliff;
		}

		/// <summary>
		///     Perturb a position.
		/// </summary>
		/// <param name="position">A position.</param>
		/// <returns>The positions with noise applied to its XZ components.</returns>
		public static Vector3 Perturb(Vector3 position)
		{
			Vector4 sample = SampleNoise(position);
			position.x += (sample.x * 2f - 1f) * CellPerturbStrength;
			position.z += (sample.z * 2f - 1f) * CellPerturbStrength;
			return position;
		}
	}
}