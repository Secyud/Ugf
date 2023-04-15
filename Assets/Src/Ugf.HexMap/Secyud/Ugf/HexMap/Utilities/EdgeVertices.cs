#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.HexMap
{
    /// <summary>
    /// Set of five vertex positions describing a cell edge.
    /// </summary>
    public struct EdgeVertices
    {
        public Vector3 V1, V2, V3, V4, V5;

        /// <summary>
        /// Create a straight edge with equidistant vertices between two corner positions.
        /// </summary>
        /// <param name="corner1">First corner.</param>
        /// <param name="corner2">Second corner.</param>
        public EdgeVertices(Vector3 corner1, Vector3 corner2)
        {
            V1 = corner1;
            V2 = Vector3.Lerp(corner1, corner2, 0.25f);
            V3 = Vector3.Lerp(corner1, corner2, 0.5f);
            V4 = Vector3.Lerp(corner1, corner2, 0.75f);
            V5 = corner2;
        }

        /// <summary>
        /// Create a straight edge between two corner positions, with configurable outer step.
        /// </summary>
        /// <param name="corner1">First corner.</param>
        /// <param name="corner2">Second corner.</param>
        /// <param name="outerStep">First step away from corners, as fraction of edge.</param>
        public EdgeVertices(Vector3 corner1, Vector3 corner2, float outerStep)
        {
            V1 = corner1;
            V2 = Vector3.Lerp(corner1, corner2, outerStep);
            V3 = Vector3.Lerp(corner1, corner2, 0.5f);
            V4 = Vector3.Lerp(corner1, corner2, 1f - outerStep);
            V5 = corner2;
        }

        /// <summary>
        /// Create edge vertices for a specific terrace step.
        /// </summary>
        /// <param name="a">Edge on first side of the terrace.</param>
        /// <param name="b">Edge on second side of the terrace.</param>
        /// <param name="step">Terrace interpolation step, 0-<see cref="HexMetrics.TerraceSteps"/> inclusive.</param>
        /// <returns>Edge vertices interpolated along terrace.</returns>
        public static EdgeVertices TerraceLerp(
            EdgeVertices a, EdgeVertices b, int step)
        {
            EdgeVertices result;
            result.V1 = HexMetrics.TerraceLerp(a.V1, b.V1, step);
            result.V2 = HexMetrics.TerraceLerp(a.V2, b.V2, step);
            result.V3 = HexMetrics.TerraceLerp(a.V3, b.V3, step);
            result.V4 = HexMetrics.TerraceLerp(a.V4, b.V4, step);
            result.V5 = HexMetrics.TerraceLerp(a.V5, b.V5, step);
            return result;
        }
    }
}