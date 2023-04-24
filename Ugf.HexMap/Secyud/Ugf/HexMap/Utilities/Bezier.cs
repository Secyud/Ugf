#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.HexMap.Utilities
{
    /// <summary>
    ///     3D quadratic Bézier functionality.
    /// </summary>
    public static class Bezier
    {
        /// <summary>
        ///     Get a point on a 3D quadratic Bézier curve.
        /// </summary>
        /// <param name="a">First control point.</param>
        /// <param name="b">Second, the middle, control point.</param>
        /// <param name="c">Third control point.</param>
        /// <param name="t">Interpolator, 0-1 inclusive.</param>
        /// <returns>The point found via interpolation.</returns>
        public static Vector3 GetPoint(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            var r = 1f - t;
            return r * r * a + 2f * r * t * b + t * t * c;
        }

        /// <summary>
        ///     Get the derivative at a point on a 3D quadratic Bézier curve.
        /// </summary>
        /// <param name="a">First control point.</param>
        /// <param name="b">Second, the middle, control point.</param>
        /// <param name="c">Third control point.</param>
        /// <param name="t">Interpolator, 0-1 inclusive.</param>
        /// <returns>The derivative found via interpolation.</returns>
        public static Vector3 GetDerivative(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            return 2f * ((1f - t) * (b - a) + t * (c - b));
        }
    }
}