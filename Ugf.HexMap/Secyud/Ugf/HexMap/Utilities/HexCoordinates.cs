﻿#region

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
            if (HexMetrics.Wrapping)
            {
                var oX = x + z / 2;
                if (oX < 0)
                    x += HexMetrics.WrapSize;
                else if (oX >= HexMetrics.WrapSize) x -= HexMetrics.WrapSize;
            }

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
        public float HexX => X + Z / 2 + ((Z & 1) == 0 ? 0f : 0.5f);

        /// <summary>
        ///     Z position in hex space,
        ///     where the distance between cell centers of east-west neighbors is one unit.
        /// </summary>
        public float HexZ => Z * HexMetrics.OuterToInner;

        /// <summary>
        ///     Determine distance between this and another set of coordinates.
        ///     Takes <see cref="HexMetrics.Wrapping" /> into account.
        /// </summary>
        /// <param name="other">Coordinate to determine distance to.</param>
        /// <returns>Distance in cells.</returns>
        public int DistanceTo(HexCoordinates other)
        {
            var xy =
                (x < other.x ? other.x - x : x - other.x) +
                (Y < other.Y ? other.Y - Y : Y - other.Y);

            if (HexMetrics.Wrapping)
            {
                other.x += HexMetrics.WrapSize;
                var xyWrapped =
                    (x < other.x ? other.x - x : x - other.x) +
                    (Y < other.Y ? other.Y - Y : Y - other.Y);
                if (xyWrapped < xy)
                {
                    xy = xyWrapped;
                }
                else
                {
                    other.x -= 2 * HexMetrics.WrapSize;
                    xyWrapped =
                        (x < other.x ? other.x - x : x - other.x) +
                        (Y < other.Y ? other.Y - Y : Y - other.Y);
                    if (xyWrapped < xy) xy = xyWrapped;
                }
            }

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
            return new HexCoordinates(x - z / 2, z);
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

        public HexDirection DirectionTo(HexCoordinates coordinates)
        {
            var c = this - coordinates;
            float cY = c.z;
            var cX = 2f * c.x + HexMetrics.Srt * c.z;

            uint record = 0;
            if (cY > -cX / HexMetrics.Srt) record += 0b001;
            if (cY > cX / HexMetrics.Srt) record += 0b010;
            if (cX < 0) record += 0b100;

            return record switch
            {
                0 => HexDirection.Se,
                1 => HexDirection.SW,
                3 => HexDirection.W,
                4 => HexDirection.E,
                6 => HexDirection.Ne,
                7 => HexDirection.Nw,
                _ => HexDirection.Ne
            };
        }
    }
}