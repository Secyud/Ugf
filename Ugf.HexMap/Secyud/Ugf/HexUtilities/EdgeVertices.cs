#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.HexMap
{
    /// <summary>
    ///     Set of five vertex positions describing a cell edge.
    /// </summary>
    public struct EdgeVertices
    {
        public Vector3 V1;
        public Vector3 V2;
        public Vector3 V3;
        public Vector3 V4;
        public Vector3 V5;

        public EdgeVertices(Vector3 v1, Vector3 v2,
            Vector3 v3, Vector3 v4, Vector3 v5)
        {
            V1 = v1;
            V2 = v2;
            V3 = v3;
            V4 = v4;
            V5 = v5;
        }

        /// <summary>
        ///     Create a straight edge between two corner positions, with configurable outer step.
        /// </summary>
        /// <param name="corner1">First corner.</param>
        /// <param name="corner2">Second corner.</param>
        /// <param name="outerStep">First step away from corners, as fraction of edge.</param>
        public static EdgeVertices Create(
            Vector3 corner1, Vector3 corner2,
            float outerStep = 0.25f)
        {
            return new EdgeVertices(
                corner1,
                Vector3.Lerp(corner1, corner2, outerStep),
                Vector3.Lerp(corner1, corner2, 0.5f),
                Vector3.Lerp(corner1, corner2, 1f - outerStep),
                corner2
            );
        }

        /// <summary>
        ///     Create edge vertices for a specific terrace step.
        /// </summary>
        /// <param name="a">Edge on first side of the terrace.</param>
        /// <param name="b">Edge on second side of the terrace.</param>
        /// <param name="step">Terrace interpolation step, 0-<see cref="HexMetrics.TerraceSteps" /> inclusive.</param>
        /// <returns>Edge vertices interpolated along terrace.</returns>
        public static EdgeVertices TerraceLerp(
            EdgeVertices a, EdgeVertices b, int step)
        {
            return new EdgeVertices(
                HexMetrics.TerraceLerp(a.V1, b.V1, step),
                HexMetrics.TerraceLerp(a.V2, b.V2, step),
                HexMetrics.TerraceLerp(a.V3, b.V3, step),
                HexMetrics.TerraceLerp(a.V4, b.V4, step),
                HexMetrics.TerraceLerp(a.V5, b.V5, step)
            );
        }


        public static EdgeVertices operator +(EdgeVertices lft, Vector3 rht)
        {
            return new EdgeVertices(
                lft.V1 + rht,
                lft.V2 + rht,
                lft.V3 + rht,
                lft.V4 + rht,
                lft.V5 + rht);
        }
    }
}