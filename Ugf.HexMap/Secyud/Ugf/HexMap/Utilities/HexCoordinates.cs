#region

using System;
using System.IO;
using UnityEngine;

#endregion

namespace Secyud.Ugf.HexMap.Utilities
{
	/// <summary>
	///     Immutable three-component hexagonal coordinates.
	/// </summary>
	[Serializable]
	public struct HexCoordinates
	{
		[SerializeField]
		// ReSharper disable InconsistentNaming
		private int x, z;
		// ReSharper restore InconsistentNaming

		/// <summary>
		///     Create hex coordinates.
		/// </summary>
		/// <param name="x">X coordinate.</param>
		/// <param name="z">Z coordinate.</param>
		public HexCoordinates(int x, int z)
		{
			this.x = x;
			this.z = z;
		}

		/// <summary>
		///     X coordinate.
		/// </summary>
		public int X => x;

		/// <summary>
		///     Z coordinate.
		/// </summary>
		public int Z => z;

		/// <summary>
		///     Y coordinate, derived from X and Z.
		/// </summary>
		public int Y => -X - Z;

		/// <summary>
		///     X position in hex space,
		///     where the distance between cell centers of east-west neighbors is one unit.
		/// </summary>
		// ReSharper disable once PossibleLossOfFraction
		public float HexX => X + Dx(Z) + ((Z & 1) == 0 ? 0f : 0.5f);

		/// <summary>
		///     Z position in hex space,
		///     where the distance between cell centers of east-west neighbors is one unit.
		/// </summary>
		public float HexZ => Z * HexMetrics.OuterToInner;

		/// <summary>
		///     Determine distance between this and another set of coordinates.
		/// </summary>
		/// <param name="other">Coordinate to determine distance to.</param>
		/// <returns>Distance in cells.</returns>
		public int DistanceTo(HexCoordinates other)
		{
			var xy =
				(x < other.x ? other.x - x : x - other.x) +
				(Y < other.Y ? other.Y - Y : Y - other.Y);
			return (xy + (z < other.z ? other.z - z : z - other.z)) / 2;
		}

		/// <summary>
		///     Create hex coordinates from array offset coordinates.
		/// </summary>
		/// <param name="x">X offset coordinate.</param>
		/// <param name="z">Z offset coordinate.</param>
		/// <returns>Hex coordinates.</returns>
		public static HexCoordinates FromOffsetCoordinates(int x, int z)
		{
			return new HexCoordinates(x - Dx(z), z);
		}

		public static int Dx(int z)
		{
			return z % 2 < 0 ? z / 2 - 1 : z / 2;
		}

		/// <summary>
		///     Create hex coordinates for the cell that contains a position.
		/// </summary>
		/// <param name="position">A 3D position assumed to lie inside the map.</param>
		/// <returns>Hex coordinates.</returns>
		public static HexCoordinates FromPosition(Vector3 position)
		{
			var x = position.x / HexMetrics.InnerDiameter;
			var y = -x;

			var offset = position.z / (HexMetrics.OuterRadius * 3f);
			x -= offset;
			y -= offset;

			var iX = Mathf.RoundToInt(x);
			var iY = Mathf.RoundToInt(y);
			var iZ = Mathf.RoundToInt(-x - y);

			if (iX + iY + iZ != 0)
			{
				var dX = Mathf.Abs(x - iX);
				var dY = Mathf.Abs(y - iY);
				var dZ = Mathf.Abs(-x - y - iZ);

				if (dX > dY && dX > dZ)
					iX = -iY - iZ;
				else if (dZ > dY) iZ = -iX - iY;
			}

			return new HexCoordinates(iX, iZ);
		}

		/// <summary>
		///     Create a string representation of the coordinates.
		/// </summary>
		/// <returns>A string of the form (X, Y, Z).</returns>
		public override string ToString()
		{
			return "(" + X + ", " + Y + ", " + Z + ")";
		}

		/// <summary>
		///     Create a multi-line string representation of the coordinates.
		/// </summary>
		/// <returns>A string of the form X\nY\nZ\n.</returns>
		public string ToStringOnSeparateLines()
		{
			return X + "\n" + Y + "\n" + Z;
		}

		/// <summary>
		///     Save the coordinates.
		/// </summary>
		/// <param name="writer"><see cref="BinaryWriter" /> to use.</param>
		public void Save(BinaryWriter writer)
		{
			writer.Write(x);
			writer.Write(z);
		}

		/// <summary>
		///     Load coordinates.
		/// </summary>
		/// <param name="reader"><see cref="BinaryReader" /> to use.</param>
		/// <returns>The coordinates.</returns>
		public static HexCoordinates Load(BinaryReader reader)
		{
			HexCoordinates c;
			c.x = reader.ReadInt32();
			c.z = reader.ReadInt32();
			return c;
		}

		public static bool operator ==(HexCoordinates lft, HexCoordinates rht)
		{
			return lft.x == rht.x && lft.z == rht.z;
		}

		public static bool operator !=(HexCoordinates lft, HexCoordinates rht)
		{
			return !(lft == rht);
		}

		public override int GetHashCode()
		{
			return x * 32768 + z;
		}
		
		public bool Equals(HexCoordinates other) => x == other.x && z == other.z;

		public override bool Equals(object obj) => obj is HexCoordinates other && Equals(other);


		public static HexCoordinates operator +(HexCoordinates lft, HexCoordinates rht)
		{
			return new HexCoordinates(lft.x + rht.x, lft.z + rht.z);
		}

		public static HexCoordinates operator -(HexCoordinates lft, HexCoordinates rht)
		{
			return new HexCoordinates(lft.x - rht.x, lft.z - rht.z);
		}

		public static HexCoordinates operator +(HexCoordinates lft, HexDirection rht)
		{
			return rht switch
			{
				HexDirection.Ne => new HexCoordinates(lft.x, lft.z + 1),
				HexDirection.E => new HexCoordinates(lft.x + 1, lft.z),
				HexDirection.Se => new HexCoordinates(lft.x + 1, lft.z - 1),
				HexDirection.SW => new HexCoordinates(lft.x, lft.z - 1),
				HexDirection.W => new HexCoordinates(lft.x - 1, lft.z),
				HexDirection.Nw => new HexCoordinates(lft.x - 1, lft.z + 1),
				_ => throw new ArgumentOutOfRangeException(nameof(rht), rht, null)
			};
		}

		public static HexCoordinates operator -(HexCoordinates lft, HexDirection rht)
		{
			return rht switch
			{
				HexDirection.Ne => new HexCoordinates(lft.x, lft.z - 1),
				HexDirection.E => new HexCoordinates(lft.x - 1, lft.z),
				HexDirection.Se => new HexCoordinates(lft.x - 1, lft.z + 1),
				HexDirection.SW => new HexCoordinates(lft.x, lft.z + 1),
				HexDirection.W => new HexCoordinates(lft.x + 1, lft.z),
				HexDirection.Nw => new HexCoordinates(lft.x + 1, lft.z - 1),
				_ => throw new ArgumentOutOfRangeException(nameof(rht), rht, null)
			};
		}

		private float XPosition => (x + z * 0.5f) * HexMetrics.InnerDiameter;


		private float ZPosition => z * (HexMetrics.OuterRadius * 1.5f);


		public Vector2 Position2D()
		{
			return new Vector2(XPosition, ZPosition);
		}

		public Vector3 Position3D()
		{
			return new Vector3(XPosition, 0, ZPosition);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="coordinates">origin</param>
		/// <returns></returns>
		public HexDirection DirectionTo(HexCoordinates coordinates)
		{
			Vector2 c = (this - coordinates).Position2D();

			uint record = 0;
			if (c.y > -c.x / HexMetrics.Srt) record += 0b001;
			if (c.y > c.x / HexMetrics.Srt) record += 0b010;
			if (c.x < 0) record += 0b100;

			Debug.Log(record);

			return record switch
			{
				0 => HexDirection.Se,
				1 => HexDirection.E,
				3 => HexDirection.Ne,
				4 => HexDirection.SW,
				6 => HexDirection.W,
				7 => HexDirection.Nw,
				_ => HexDirection.Ne
			};
		}
	}
}