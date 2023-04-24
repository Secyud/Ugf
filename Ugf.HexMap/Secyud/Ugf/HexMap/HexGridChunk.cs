#region

using Secyud.Ugf.HexMap.Utilities;
using UnityEngine;

#endregion

namespace Secyud.Ugf.HexMap
{
    /// <summary>
    ///     Component that manages a single chunk of <see cref="HexGrid" />.
    /// </summary>
    public class HexGridChunk : MonoBehaviour
    {
        private static readonly Color Weights1 = new(1f, 0f, 0f);
        private static readonly Color Weights2 = new(0f, 1f, 0f);
        private static readonly Color Weights3 = new(0f, 0f, 1f);
        [SerializeField] private HexMesh Terrain;
        [SerializeField] private HexMesh Rivers;
        [SerializeField] private HexMesh Roads;
        [SerializeField] private HexMesh Water;
        [SerializeField] private HexMesh WaterShore;
        [SerializeField] private HexMesh Estuaries;
        [SerializeField] private HexFeatureManager Features;

        private HexCell[] _cells;
        private Canvas _gridCanvas;

        public HexGrid Grid
        {
            get => Features.Grid;
            set => Features.Grid = value;
        }

        private void Awake()
        {
            _gridCanvas = GetComponentInChildren<Canvas>();

            _cells = new HexCell[HexMetrics.ChunkSizeX * HexMetrics.ChunkSizeZ];
        }

        private void LateUpdate()
        {
            Triangulate();
            enabled = false;
        }

        /// <summary>
        ///     Add a cell to the chunk.
        /// </summary>
        /// <param name="index">Index of the cell for the chunk.</param>
        /// <param name="cell">Cell to add.</param>
        public void AddCell(int index, HexCell cell)
        {
            _cells[index] = cell;
            cell.Chunk = this;
            cell.transform.SetParent(transform, false);
            cell.UIRect.SetParent(_gridCanvas.transform, false);
        }

        /// <summary>
        ///     Refresh the chunk.
        /// </summary>
        public void Refresh()
        {
            enabled = true;
        }

        /// <summary>
        ///     Control whether the map UI is visible or hidden for the chunk.
        /// </summary>
        /// <param name="visible">Whether the UI should be visible.</param>
        public void ShowUI(bool visible)
        {
            _gridCanvas.gameObject.SetActive(visible);
        }

        /// <summary>
        ///     Triangulate everything in the chunk.
        /// </summary>
        public void Triangulate()
        {
            Terrain.Clear();
            Rivers.Clear();
            Roads.Clear();
            Water.Clear();
            WaterShore.Clear();
            Estuaries.Clear();
            Features.Clear();
            foreach (var cell in _cells) Triangulate(cell);

            Terrain.Apply();
            Rivers.Apply();
            Roads.Apply();
            Water.Apply();
            WaterShore.Apply();
            Estuaries.Apply();
            Features.Apply();
        }

        private void Triangulate(HexCell cell)
        {
            for (var d = HexDirection.Ne; d <= HexDirection.Nw; d++) Triangulate(d, cell);

            if (!cell.IsUnderwater)
            {
                if (cell.IsSpecial)
                    Features.AddSpecialFeature(cell, cell.Position);
                else if (!cell.HasRiver && !cell.HasRoads)
                    Features.AddFeature(cell, cell.Position);
            }
        }

        private void Triangulate(HexDirection direction, HexCell cell)
        {
            var center = cell.Position;
            var e = new EdgeVertices(
                center + HexMetrics.GetFirstSolidCorner(direction),
                center + HexMetrics.GetSecondSolidCorner(direction)
            );

            if (cell.HasRiver)
            {
                if (cell.HasRiverThroughEdge(direction))
                {
                    e.V3.y = cell.StreamBedY;
                    if (cell.HasRiverBeginOrEnd)
                        TriangulateWithRiverBeginOrEnd(direction, cell, center, e);
                    else
                        TriangulateWithRiver(direction, cell, center, e);
                }
                else
                {
                    TriangulateAdjacentToRiver(direction, cell, center, e);
                }
            }
            else
            {
                TriangulateWithoutRiver(direction, cell, center, e);

                if (!cell.IsUnderwater && !cell.HasRoadThroughEdge(direction))
                    Features.AddFeature(cell, (center + e.V1 + e.V5) * (1f / 3f));
            }

            if (direction <= HexDirection.Se) TriangulateConnection(direction, cell, e);

            if (cell.IsUnderwater) TriangulateWater(direction, cell, center);
        }

        private void TriangulateWater(
            HexDirection direction, HexCell cell, Vector3 center
        )
        {
            center.y = cell.WaterSurfaceY;

            var neighbor = cell.GetNeighbor(direction);
            if (neighbor != null && !neighbor.IsUnderwater)
                TriangulateWaterShore(direction, cell, neighbor, center);
            else
                TriangulateOpenWater(direction, cell, neighbor, center);
        }

        private void TriangulateOpenWater(
            HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center
        )
        {
            var c1 = center + HexMetrics.GetFirstWaterCorner(direction);
            var c2 = center + HexMetrics.GetSecondWaterCorner(direction);

            Water.AddTriangle(center, c1, c2);
            Vector3 indices;
            indices.x = indices.y = indices.z = cell.Index;
            Water.AddTriangleCellData(indices, Weights1);

            if (direction <= HexDirection.Se && neighbor != null)
            {
                var bridge = HexMetrics.GetWaterBridge(direction);
                var e1 = c1 + bridge;
                var e2 = c2 + bridge;

                Water.AddQuad(c1, c2, e1, e2);
                indices.y = neighbor.Index;
                Water.AddQuadCellData(indices, Weights1, Weights2);

                if (direction <= HexDirection.E)
                {
                    var nextNeighbor = cell.GetNeighbor(direction.Next());
                    if (nextNeighbor == null || !nextNeighbor.IsUnderwater) return;

                    Water.AddTriangle(
                        c2, e2, c2 + HexMetrics.GetWaterBridge(direction.Next())
                    );
                    indices.z = nextNeighbor.Index;
                    Water.AddTriangleCellData(
                        indices, Weights1, Weights2, Weights3
                    );
                }
            }
        }

        private void TriangulateWaterShore(
            HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center
        )
        {
            var e1 = new EdgeVertices(
                center + HexMetrics.GetFirstWaterCorner(direction),
                center + HexMetrics.GetSecondWaterCorner(direction)
            );
            Water.AddTriangle(center, e1.V1, e1.V2);
            Water.AddTriangle(center, e1.V2, e1.V3);
            Water.AddTriangle(center, e1.V3, e1.V4);
            Water.AddTriangle(center, e1.V4, e1.V5);
            Vector3 indices;
            indices.x = indices.z = cell.Index;
            indices.y = neighbor.Index;
            Water.AddTriangleCellData(indices, Weights1);
            Water.AddTriangleCellData(indices, Weights1);
            Water.AddTriangleCellData(indices, Weights1);
            Water.AddTriangleCellData(indices, Weights1);

            var center2 = neighbor.Position;
            if (neighbor.ColumnIndex < cell.ColumnIndex - 1)
                center2.x += HexMetrics.WrapSize * HexMetrics.InnerDiameter;
            else if (neighbor.ColumnIndex > cell.ColumnIndex + 1)
                center2.x -= HexMetrics.WrapSize * HexMetrics.InnerDiameter;

            center2.y = center.y;
            var e2 = new EdgeVertices(
                center2 + HexMetrics.GetSecondSolidCorner(direction.Opposite()),
                center2 + HexMetrics.GetFirstSolidCorner(direction.Opposite())
            );

            if (cell.HasRiverThroughEdge(direction))
            {
                TriangulateEstuary(
                    e1, e2,
                    cell.HasIncomingRiver && cell.IncomingRiver == direction, indices
                );
            }
            else
            {
                WaterShore.AddQuad(e1.V1, e1.V2, e2.V1, e2.V2);
                WaterShore.AddQuad(e1.V2, e1.V3, e2.V2, e2.V3);
                WaterShore.AddQuad(e1.V3, e1.V4, e2.V3, e2.V4);
                WaterShore.AddQuad(e1.V4, e1.V5, e2.V4, e2.V5);
                WaterShore.AddQuadUV(0f, 0f, 0f, 1f);
                WaterShore.AddQuadUV(0f, 0f, 0f, 1f);
                WaterShore.AddQuadUV(0f, 0f, 0f, 1f);
                WaterShore.AddQuadUV(0f, 0f, 0f, 1f);
                WaterShore.AddQuadCellData(indices, Weights1, Weights2);
                WaterShore.AddQuadCellData(indices, Weights1, Weights2);
                WaterShore.AddQuadCellData(indices, Weights1, Weights2);
                WaterShore.AddQuadCellData(indices, Weights1, Weights2);
            }

            var nextNeighbor = cell.GetNeighbor(direction.Next());
            if (nextNeighbor != null)
            {
                var center3 = nextNeighbor.Position;
                if (nextNeighbor.ColumnIndex < cell.ColumnIndex - 1)
                    center3.x += HexMetrics.WrapSize * HexMetrics.InnerDiameter;
                else if (nextNeighbor.ColumnIndex > cell.ColumnIndex + 1)
                    center3.x -= HexMetrics.WrapSize * HexMetrics.InnerDiameter;

                var v3 = center3 + (nextNeighbor.IsUnderwater
                    ? HexMetrics.GetFirstWaterCorner(direction.Previous())
                    : HexMetrics.GetFirstSolidCorner(direction.Previous()));
                v3.y = center.y;
                WaterShore.AddTriangle(e1.V5, e2.V5, v3);
                WaterShore.AddTriangleUV(
                    new Vector2(0f, 0f),
                    new Vector2(0f, 1f),
                    new Vector2(0f, nextNeighbor.IsUnderwater ? 0f : 1f)
                );
                indices.z = nextNeighbor.Index;
                WaterShore.AddTriangleCellData(
                    indices, Weights1, Weights2, Weights3
                );
            }
        }

        private void TriangulateEstuary(
            EdgeVertices e1, EdgeVertices e2, bool incomingRiver, Vector3 indices
        )
        {
            WaterShore.AddTriangle(e2.V1, e1.V2, e1.V1);
            WaterShore.AddTriangle(e2.V5, e1.V5, e1.V4);
            WaterShore.AddTriangleUV(
                new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(0f, 0f)
            );
            WaterShore.AddTriangleUV(
                new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(0f, 0f)
            );
            WaterShore.AddTriangleCellData(indices, Weights2, Weights1, Weights1);
            WaterShore.AddTriangleCellData(indices, Weights2, Weights1, Weights1);

            Estuaries.AddQuad(e2.V1, e1.V2, e2.V2, e1.V3);
            Estuaries.AddTriangle(e1.V3, e2.V2, e2.V4);
            Estuaries.AddQuad(e1.V3, e1.V4, e2.V4, e2.V5);

            Estuaries.AddQuadUV(
                new Vector2(0f, 1f), new Vector2(0f, 0f),
                new Vector2(1f, 1f), new Vector2(0f, 0f)
            );
            Estuaries.AddTriangleUV(
                new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(1f, 1f)
            );
            Estuaries.AddQuadUV(
                new Vector2(0f, 0f), new Vector2(0f, 0f),
                new Vector2(1f, 1f), new Vector2(0f, 1f)
            );
            Estuaries.AddQuadCellData(
                indices, Weights2, Weights1, Weights2, Weights1
            );
            Estuaries.AddTriangleCellData(indices, Weights1, Weights2, Weights2);
            Estuaries.AddQuadCellData(indices, Weights1, Weights2);

            if (incomingRiver)
            {
                Estuaries.AddQuadUV2(
                    new Vector2(1.5f, 1f), new Vector2(0.7f, 1.15f),
                    new Vector2(1f, 0.8f), new Vector2(0.5f, 1.1f)
                );
                Estuaries.AddTriangleUV2(
                    new Vector2(0.5f, 1.1f),
                    new Vector2(1f, 0.8f),
                    new Vector2(0f, 0.8f)
                );
                Estuaries.AddQuadUV2(
                    new Vector2(0.5f, 1.1f), new Vector2(0.3f, 1.15f),
                    new Vector2(0f, 0.8f), new Vector2(-0.5f, 1f)
                );
            }
            else
            {
                Estuaries.AddQuadUV2(
                    new Vector2(-0.5f, -0.2f), new Vector2(0.3f, -0.35f),
                    new Vector2(0f, 0f), new Vector2(0.5f, -0.3f)
                );
                Estuaries.AddTriangleUV2(
                    new Vector2(0.5f, -0.3f),
                    new Vector2(0f, 0f),
                    new Vector2(1f, 0f)
                );
                Estuaries.AddQuadUV2(
                    new Vector2(0.5f, -0.3f), new Vector2(0.7f, -0.35f),
                    new Vector2(1f, 0f), new Vector2(1.5f, -0.2f)
                );
            }
        }

        private void TriangulateWithoutRiver(
            HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
        )
        {
            TriangulateEdgeFan(center, e, cell.Index);

            if (cell.HasRoads)
            {
                var interpolators = GetRoadInterpolators(direction, cell);
                TriangulateRoad(
                    center,
                    Vector3.Lerp(center, e.V1, interpolators.x),
                    Vector3.Lerp(center, e.V5, interpolators.y),
                    e, cell.HasRoadThroughEdge(direction), cell.Index
                );
            }
        }

        private Vector2 GetRoadInterpolators(HexDirection direction, HexCell cell)
        {
            Vector2 interpolators;
            if (cell.HasRoadThroughEdge(direction))
            {
                interpolators.x = interpolators.y = 0.5f;
            }
            else
            {
                interpolators.x =
                    cell.HasRoadThroughEdge(direction.Previous()) ? 0.5f : 0.25f;
                interpolators.y =
                    cell.HasRoadThroughEdge(direction.Next()) ? 0.5f : 0.25f;
            }

            return interpolators;
        }

        private void TriangulateAdjacentToRiver(
            HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
        )
        {
            if (cell.HasRoads) TriangulateRoadAdjacentToRiver(direction, cell, center, e);

            if (cell.HasRiverThroughEdge(direction.Next()))
            {
                if (cell.HasRiverThroughEdge(direction.Previous()))
                    center += HexMetrics.GetSolidEdgeMiddle(direction) *
                              (HexMetrics.InnerToOuter * 0.5f);
                else if (
                    cell.HasRiverThroughEdge(direction.Previous2())
                )
                    center += HexMetrics.GetFirstSolidCorner(direction) * 0.25f;
            }
            else if (
                cell.HasRiverThroughEdge(direction.Previous()) &&
                cell.HasRiverThroughEdge(direction.Next2())
            )
            {
                center += HexMetrics.GetSecondSolidCorner(direction) * 0.25f;
            }

            var m = new EdgeVertices(
                Vector3.Lerp(center, e.V1, 0.5f),
                Vector3.Lerp(center, e.V5, 0.5f)
            );

            TriangulateEdgeStrip(
                m, Weights1, cell.Index,
                e, Weights1, cell.Index
            );
            TriangulateEdgeFan(center, m, cell.Index);

            if (!cell.IsUnderwater && !cell.HasRoadThroughEdge(direction))
                Features.AddFeature(cell, (center + e.V1 + e.V5) * (1f / 3f));
        }

        private void TriangulateRoadAdjacentToRiver(
            HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
        )
        {
            var hasRoadThroughEdge = cell.HasRoadThroughEdge(direction);
            var previousHasRiver = cell.HasRiverThroughEdge(direction.Previous());
            var nextHasRiver = cell.HasRiverThroughEdge(direction.Next());
            var interpolators = GetRoadInterpolators(direction, cell);
            var roadCenter = center;

            if (cell.HasRiverBeginOrEnd)
            {
                roadCenter += HexMetrics.GetSolidEdgeMiddle(
                    cell.RiverBeginOrEndDirection.Opposite()
                ) * (1f / 3f);
            }
            else if (cell.IncomingRiver == cell.OutgoingRiver.Opposite())
            {
                Vector3 corner;
                if (previousHasRiver)
                {
                    if (
                        !hasRoadThroughEdge &&
                        !cell.HasRoadThroughEdge(direction.Next())
                    )
                        return;

                    corner = HexMetrics.GetSecondSolidCorner(direction);
                }
                else
                {
                    if (
                        !hasRoadThroughEdge &&
                        !cell.HasRoadThroughEdge(direction.Previous())
                    )
                        return;

                    corner = HexMetrics.GetFirstSolidCorner(direction);
                }

                roadCenter += corner * 0.5f;
                if (cell.IncomingRiver == direction.Next() && (
                        cell.HasRoadThroughEdge(direction.Next2()) ||
                        cell.HasRoadThroughEdge(direction.Opposite())
                    ))
                    Features.AddBridge(roadCenter, center - corner * 0.5f);

                center += corner * 0.25f;
            }
            else if (cell.IncomingRiver == cell.OutgoingRiver.Previous())
            {
                roadCenter -= HexMetrics.GetSecondCorner(cell.IncomingRiver) * 0.2f;
            }
            else if (cell.IncomingRiver == cell.OutgoingRiver.Next())
            {
                roadCenter -= HexMetrics.GetFirstCorner(cell.IncomingRiver) * 0.2f;
            }
            else if (previousHasRiver && nextHasRiver)
            {
                if (!hasRoadThroughEdge) return;

                var offset = HexMetrics.GetSolidEdgeMiddle(direction) *
                             HexMetrics.InnerToOuter;
                roadCenter += offset * 0.7f;
                center += offset * 0.5f;
            }
            else
            {
                HexDirection middle;
                if (previousHasRiver)
                    middle = direction.Next();
                else if (nextHasRiver)
                    middle = direction.Previous();
                else
                    middle = direction;

                if (
                    !cell.HasRoadThroughEdge(middle) &&
                    !cell.HasRoadThroughEdge(middle.Previous()) &&
                    !cell.HasRoadThroughEdge(middle.Next())
                )
                    return;

                var offset = HexMetrics.GetSolidEdgeMiddle(middle);
                roadCenter += offset * 0.25f;
                if (
                    direction == middle &&
                    cell.HasRoadThroughEdge(direction.Opposite())
                )
                    Features.AddBridge(
                        roadCenter,
                        center - offset * (HexMetrics.InnerToOuter * 0.7f)
                    );
            }

            var mL = Vector3.Lerp(roadCenter, e.V1, interpolators.x);
            var mR = Vector3.Lerp(roadCenter, e.V5, interpolators.y);
            TriangulateRoad(roadCenter, mL, mR, e, hasRoadThroughEdge, cell.Index);
            if (previousHasRiver) TriangulateRoadEdge(roadCenter, center, mL, cell.Index);

            if (nextHasRiver) TriangulateRoadEdge(roadCenter, mR, center, cell.Index);
        }

        private void TriangulateWithRiverBeginOrEnd(
            // ReSharper disable once UnusedParameter.Local
            HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
        )
        {
            var m = new EdgeVertices(
                Vector3.Lerp(center, e.V1, 0.5f),
                Vector3.Lerp(center, e.V5, 0.5f)
            );
            m.V3.y = e.V3.y;

            TriangulateEdgeStrip(
                m, Weights1, cell.Index,
                e, Weights1, cell.Index
            );
            TriangulateEdgeFan(center, m, cell.Index);

            if (!cell.IsUnderwater)
            {
                var reversed = cell.HasIncomingRiver;
                Vector3 indices;
                indices.x = indices.y = indices.z = cell.Index;
                TriangulateRiverQuad(
                    m.V2, m.V4, e.V2, e.V4,
                    cell.RiverSurfaceY, 0.6f, reversed, indices
                );
                center.y = m.V2.y = m.V4.y = cell.RiverSurfaceY;
                Rivers.AddTriangle(center, m.V2, m.V4);
                if (reversed)
                    Rivers.AddTriangleUV(
                        new Vector2(0.5f, 0.4f),
                        new Vector2(1f, 0.2f), new Vector2(0f, 0.2f)
                    );
                else
                    Rivers.AddTriangleUV(
                        new Vector2(0.5f, 0.4f),
                        new Vector2(0f, 0.6f), new Vector2(1f, 0.6f)
                    );

                Rivers.AddTriangleCellData(indices, Weights1);
            }
        }

        private void TriangulateWithRiver(
            HexDirection direction, HexCell cell, Vector3 center, EdgeVertices e
        )
        {
            Vector3 centerL, centerR;
            if (cell.HasRiverThroughEdge(direction.Opposite()))
            {
                centerL = center +
                          HexMetrics.GetFirstSolidCorner(direction.Previous()) * 0.25f;
                centerR = center +
                          HexMetrics.GetSecondSolidCorner(direction.Next()) * 0.25f;
            }
            else if (cell.HasRiverThroughEdge(direction.Next()))
            {
                centerL = center;
                centerR = Vector3.Lerp(center, e.V5, 2f / 3f);
            }
            else if (cell.HasRiverThroughEdge(direction.Previous()))
            {
                centerL = Vector3.Lerp(center, e.V1, 2f / 3f);
                centerR = center;
            }
            else if (cell.HasRiverThroughEdge(direction.Next2()))
            {
                centerL = center;
                centerR = center +
                          HexMetrics.GetSolidEdgeMiddle(direction.Next()) *
                          (0.5f * HexMetrics.InnerToOuter);
            }
            else
            {
                centerL = center +
                          HexMetrics.GetSolidEdgeMiddle(direction.Previous()) *
                          (0.5f * HexMetrics.InnerToOuter);
                centerR = center;
            }

            center = Vector3.Lerp(centerL, centerR, 0.5f);

            var m = new EdgeVertices(
                Vector3.Lerp(centerL, e.V1, 0.5f),
                Vector3.Lerp(centerR, e.V5, 0.5f),
                1f / 6f
            );
            m.V3.y = center.y = e.V3.y;

            TriangulateEdgeStrip(
                m, Weights1, cell.Index,
                e, Weights1, cell.Index
            );

            Terrain.AddTriangle(centerL, m.V1, m.V2);
            Terrain.AddQuad(centerL, center, m.V2, m.V3);
            Terrain.AddQuad(center, centerR, m.V3, m.V4);
            Terrain.AddTriangle(centerR, m.V4, m.V5);

            Vector3 indices;
            indices.x = indices.y = indices.z = cell.Index;
            Terrain.AddTriangleCellData(indices, Weights1);
            Terrain.AddQuadCellData(indices, Weights1);
            Terrain.AddQuadCellData(indices, Weights1);
            Terrain.AddTriangleCellData(indices, Weights1);

            if (!cell.IsUnderwater)
            {
                var reversed = cell.IncomingRiver == direction;
                TriangulateRiverQuad(
                    centerL, centerR, m.V2, m.V4,
                    cell.RiverSurfaceY, 0.4f, reversed, indices
                );
                TriangulateRiverQuad(
                    m.V2, m.V4, e.V2, e.V4,
                    cell.RiverSurfaceY, 0.6f, reversed, indices
                );
            }
        }

        private void TriangulateConnection(
            HexDirection direction, HexCell cell, EdgeVertices e1
        )
        {
            var neighbor = cell.GetNeighbor(direction);
            if (neighbor == null) return;

            var bridge = HexMetrics.GetBridge(direction);
            bridge.y = neighbor.Position.y - cell.Position.y;
            var e2 = new EdgeVertices(
                e1.V1 + bridge,
                e1.V5 + bridge
            );

            var hasRiver = cell.HasRiverThroughEdge(direction);
            var hasRoad = cell.HasRoadThroughEdge(direction);

            if (hasRiver)
            {
                e2.V3.y = neighbor.StreamBedY;
                Vector3 indices;
                indices.x = indices.z = cell.Index;
                indices.y = neighbor.Index;

                if (!cell.IsUnderwater)
                {
                    if (!neighbor.IsUnderwater)
                        TriangulateRiverQuad(
                            e1.V2, e1.V4, e2.V2, e2.V4,
                            cell.RiverSurfaceY, neighbor.RiverSurfaceY, 0.8f,
                            cell.HasIncomingRiver && cell.IncomingRiver == direction,
                            indices
                        );
                    else if (cell.Elevation > neighbor.WaterLevel)
                        TriangulateWaterfallInWater(
                            e1.V2, e1.V4, e2.V2, e2.V4,
                            cell.RiverSurfaceY, neighbor.RiverSurfaceY,
                            neighbor.WaterSurfaceY, indices
                        );
                }
                else if (
                    !neighbor.IsUnderwater &&
                    neighbor.Elevation > cell.WaterLevel
                )
                {
                    TriangulateWaterfallInWater(
                        e2.V4, e2.V2, e1.V4, e1.V2,
                        neighbor.RiverSurfaceY, cell.RiverSurfaceY,
                        cell.WaterSurfaceY, indices
                    );
                }
            }

            if (cell.GetEdgeType(direction) == HexEdgeType.Slope)
                TriangulateEdgeTerraces(e1, cell, e2, neighbor, hasRoad);
            else
                TriangulateEdgeStrip(
                    e1, Weights1, cell.Index,
                    e2, Weights2, neighbor.Index, hasRoad
                );

            Features.AddWall(e1, cell, e2, neighbor, hasRiver, hasRoad);

            var nextNeighbor = cell.GetNeighbor(direction.Next());
            if (direction <= HexDirection.E && nextNeighbor != null)
            {
                var v5 = e1.V5 + HexMetrics.GetBridge(direction.Next());
                v5.y = nextNeighbor.Position.y;

                if (cell.Elevation <= neighbor.Elevation)
                {
                    if (cell.Elevation <= nextNeighbor.Elevation)
                        TriangulateCorner(
                            e1.V5, cell, e2.V5, neighbor, v5, nextNeighbor
                        );
                    else
                        TriangulateCorner(
                            v5, nextNeighbor, e1.V5, cell, e2.V5, neighbor
                        );
                }
                else if (neighbor.Elevation <= nextNeighbor.Elevation)
                {
                    TriangulateCorner(
                        e2.V5, neighbor, v5, nextNeighbor, e1.V5, cell
                    );
                }
                else
                {
                    TriangulateCorner(
                        v5, nextNeighbor, e1.V5, cell, e2.V5, neighbor
                    );
                }
            }
        }

        private void TriangulateWaterfallInWater(
            Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
            float y1, float y2, float waterY, Vector3 indices
        )
        {
            v1.y = v2.y = y1;
            v3.y = v4.y = y2;
            v1 = HexMetrics.Perturb(v1);
            v2 = HexMetrics.Perturb(v2);
            v3 = HexMetrics.Perturb(v3);
            v4 = HexMetrics.Perturb(v4);
            var t = (waterY - y2) / (y1 - y2);
            v3 = Vector3.Lerp(v3, v1, t);
            v4 = Vector3.Lerp(v4, v2, t);
            Rivers.AddQuadUnperturbed(v1, v2, v3, v4);
            Rivers.AddQuadUV(0f, 1f, 0.8f, 1f);
            Rivers.AddQuadCellData(indices, Weights1, Weights2);
        }

        private void TriangulateCorner(
            Vector3 bottom, HexCell bottomCell,
            Vector3 left, HexCell leftCell,
            Vector3 right, HexCell rightCell
        )
        {
            var leftEdgeType = bottomCell.GetEdgeType(leftCell);
            var rightEdgeType = bottomCell.GetEdgeType(rightCell);

            if (leftEdgeType == HexEdgeType.Slope)
            {
                if (rightEdgeType == HexEdgeType.Slope)
                    TriangulateCornerTerraces(
                        bottom, bottomCell, left, leftCell, right, rightCell
                    );
                else if (rightEdgeType == HexEdgeType.Flat)
                    TriangulateCornerTerraces(
                        left, leftCell, right, rightCell, bottom, bottomCell
                    );
                else
                    TriangulateCornerTerracesCliff(
                        bottom, bottomCell, left, leftCell, right, rightCell
                    );
            }
            else if (rightEdgeType == HexEdgeType.Slope)
            {
                if (leftEdgeType == HexEdgeType.Flat)
                    TriangulateCornerTerraces(
                        right, rightCell, bottom, bottomCell, left, leftCell
                    );
                else
                    TriangulateCornerCliffTerraces(
                        bottom, bottomCell, left, leftCell, right, rightCell
                    );
            }
            else if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
            {
                if (leftCell.Elevation < rightCell.Elevation)
                    TriangulateCornerCliffTerraces(
                        right, rightCell, bottom, bottomCell, left, leftCell
                    );
                else
                    TriangulateCornerTerracesCliff(
                        left, leftCell, right, rightCell, bottom, bottomCell
                    );
            }
            else
            {
                Terrain.AddTriangle(bottom, left, right);
                Vector3 indices;
                indices.x = bottomCell.Index;
                indices.y = leftCell.Index;
                indices.z = rightCell.Index;
                Terrain.AddTriangleCellData(indices, Weights1, Weights2, Weights3);
            }

            Features.AddWall(bottom, bottomCell, left, leftCell, right, rightCell);
        }

        private void TriangulateEdgeTerraces(
            EdgeVertices begin, HexCell beginCell,
            EdgeVertices end, HexCell endCell,
            bool hasRoad
        )
        {
            var e2 = EdgeVertices.TerraceLerp(begin, end, 1);
            var w2 = HexMetrics.TerraceLerp(Weights1, Weights2, 1);
            float i1 = beginCell.Index;
            float i2 = endCell.Index;

            TriangulateEdgeStrip(begin, Weights1, i1, e2, w2, i2, hasRoad);

            for (var i = 2; i < HexMetrics.TerraceSteps; i++)
            {
                var e1 = e2;
                var w1 = w2;
                e2 = EdgeVertices.TerraceLerp(begin, end, i);
                w2 = HexMetrics.TerraceLerp(Weights1, Weights2, i);
                TriangulateEdgeStrip(e1, w1, i1, e2, w2, i2, hasRoad);
            }

            TriangulateEdgeStrip(e2, w2, i1, end, Weights2, i2, hasRoad);
        }

        private void TriangulateCornerTerraces(
            Vector3 begin, HexCell beginCell,
            Vector3 left, HexCell leftCell,
            Vector3 right, HexCell rightCell
        )
        {
            var v3 = HexMetrics.TerraceLerp(begin, left, 1);
            var v4 = HexMetrics.TerraceLerp(begin, right, 1);
            var w3 = HexMetrics.TerraceLerp(Weights1, Weights2, 1);
            var w4 = HexMetrics.TerraceLerp(Weights1, Weights3, 1);
            Vector3 indices;
            indices.x = beginCell.Index;
            indices.y = leftCell.Index;
            indices.z = rightCell.Index;

            Terrain.AddTriangle(begin, v3, v4);
            Terrain.AddTriangleCellData(indices, Weights1, w3, w4);

            for (var i = 2; i < HexMetrics.TerraceSteps; i++)
            {
                var v1 = v3;
                var v2 = v4;
                var w1 = w3;
                var w2 = w4;
                v3 = HexMetrics.TerraceLerp(begin, left, i);
                v4 = HexMetrics.TerraceLerp(begin, right, i);
                w3 = HexMetrics.TerraceLerp(Weights1, Weights2, i);
                w4 = HexMetrics.TerraceLerp(Weights1, Weights3, i);
                Terrain.AddQuad(v1, v2, v3, v4);
                Terrain.AddQuadCellData(indices, w1, w2, w3, w4);
            }

            Terrain.AddQuad(v3, v4, left, right);
            Terrain.AddQuadCellData(indices, w3, w4, Weights2, Weights3);
        }

        private void TriangulateCornerTerracesCliff(
            Vector3 begin, HexCell beginCell,
            Vector3 left, HexCell leftCell,
            Vector3 right, HexCell rightCell
        )
        {
            var b = 1f / (rightCell.Elevation - beginCell.Elevation);
            if (b < 0) b = -b;

            var boundary = Vector3.Lerp(
                HexMetrics.Perturb(begin), HexMetrics.Perturb(right), b
            );
            var boundaryWeights = Color.Lerp(Weights1, Weights3, b);
            Vector3 indices;
            indices.x = beginCell.Index;
            indices.y = leftCell.Index;
            indices.z = rightCell.Index;

            TriangulateBoundaryTriangle(
                begin, Weights1, left, Weights2, boundary, boundaryWeights, indices
            );

            if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
            {
                TriangulateBoundaryTriangle(
                    left, Weights2, right, Weights3,
                    boundary, boundaryWeights, indices
                );
            }
            else
            {
                Terrain.AddTriangleUnperturbed(
                    HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary
                );
                Terrain.AddTriangleCellData(
                    indices, Weights2, Weights3, boundaryWeights
                );
            }
        }

        private void TriangulateCornerCliffTerraces(
            Vector3 begin, HexCell beginCell,
            Vector3 left, HexCell leftCell,
            Vector3 right, HexCell rightCell
        )
        {
            var b = 1f / (leftCell.Elevation - beginCell.Elevation);
            if (b < 0) b = -b;

            var boundary = Vector3.Lerp(
                HexMetrics.Perturb(begin), HexMetrics.Perturb(left), b
            );
            var boundaryWeights = Color.Lerp(Weights1, Weights2, b);
            Vector3 indices;
            indices.x = beginCell.Index;
            indices.y = leftCell.Index;
            indices.z = rightCell.Index;

            TriangulateBoundaryTriangle(
                right, Weights3, begin, Weights1, boundary, boundaryWeights, indices
            );

            if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
            {
                TriangulateBoundaryTriangle(
                    left, Weights2, right, Weights3,
                    boundary, boundaryWeights, indices
                );
            }
            else
            {
                Terrain.AddTriangleUnperturbed(
                    HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary
                );
                Terrain.AddTriangleCellData(
                    indices, Weights2, Weights3, boundaryWeights
                );
            }
        }

        private void TriangulateBoundaryTriangle(
            Vector3 begin, Color beginWeights,
            Vector3 left, Color leftWeights,
            Vector3 boundary, Color boundaryWeights, Vector3 indices
        )
        {
            var v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, 1));
            var w2 = HexMetrics.TerraceLerp(beginWeights, leftWeights, 1);

            Terrain.AddTriangleUnperturbed(HexMetrics.Perturb(begin), v2, boundary);
            Terrain.AddTriangleCellData(indices, beginWeights, w2, boundaryWeights);

            for (var i = 2; i < HexMetrics.TerraceSteps; i++)
            {
                var v1 = v2;
                var w1 = w2;
                v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, i));
                w2 = HexMetrics.TerraceLerp(beginWeights, leftWeights, i);
                Terrain.AddTriangleUnperturbed(v1, v2, boundary);
                Terrain.AddTriangleCellData(indices, w1, w2, boundaryWeights);
            }

            Terrain.AddTriangleUnperturbed(v2, HexMetrics.Perturb(left), boundary);
            Terrain.AddTriangleCellData(indices, w2, leftWeights, boundaryWeights);
        }

        private void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, float index)
        {
            Terrain.AddTriangle(center, edge.V1, edge.V2);
            Terrain.AddTriangle(center, edge.V2, edge.V3);
            Terrain.AddTriangle(center, edge.V3, edge.V4);
            Terrain.AddTriangle(center, edge.V4, edge.V5);

            Vector3 indices;
            indices.x = indices.y = indices.z = index;
            Terrain.AddTriangleCellData(indices, Weights1);
            Terrain.AddTriangleCellData(indices, Weights1);
            Terrain.AddTriangleCellData(indices, Weights1);
            Terrain.AddTriangleCellData(indices, Weights1);
        }

        private void TriangulateEdgeStrip(
            EdgeVertices e1, Color w1, float index1,
            EdgeVertices e2, Color w2, float index2,
            bool hasRoad = false
        )
        {
            Terrain.AddQuad(e1.V1, e1.V2, e2.V1, e2.V2);
            Terrain.AddQuad(e1.V2, e1.V3, e2.V2, e2.V3);
            Terrain.AddQuad(e1.V3, e1.V4, e2.V3, e2.V4);
            Terrain.AddQuad(e1.V4, e1.V5, e2.V4, e2.V5);

            Vector3 indices;
            indices.x = indices.z = index1;
            indices.y = index2;
            Terrain.AddQuadCellData(indices, w1, w2);
            Terrain.AddQuadCellData(indices, w1, w2);
            Terrain.AddQuadCellData(indices, w1, w2);
            Terrain.AddQuadCellData(indices, w1, w2);

            if (hasRoad)
                TriangulateRoadSegment(
                    e1.V2, e1.V3, e1.V4, e2.V2, e2.V3, e2.V4, w1, w2, indices
                );
        }

        private void TriangulateRiverQuad(
            Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
            float y, float v, bool reversed, Vector3 indices
        )
        {
            TriangulateRiverQuad(v1, v2, v3, v4, y, y, v, reversed, indices);
        }

        private void TriangulateRiverQuad(
            Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
            float y1, float y2, float v, bool reversed, Vector3 indices
        )
        {
            v1.y = v2.y = y1;
            v3.y = v4.y = y2;
            Rivers.AddQuad(v1, v2, v3, v4);
            if (reversed)
                Rivers.AddQuadUV(1f, 0f, 0.8f - v, 0.6f - v);
            else
                Rivers.AddQuadUV(0f, 1f, v, v + 0.2f);

            Rivers.AddQuadCellData(indices, Weights1, Weights2);
        }

        private void TriangulateRoad(
            Vector3 center, Vector3 mL, Vector3 mR,
            EdgeVertices e, bool hasRoadThroughCellEdge, float index
        )
        {
            if (hasRoadThroughCellEdge)
            {
                Vector3 indices;
                indices.x = indices.y = indices.z = index;
                var mC = Vector3.Lerp(mL, mR, 0.5f);
                TriangulateRoadSegment(
                    mL, mC, mR, e.V2, e.V3, e.V4,
                    Weights1, Weights1, indices
                );
                Roads.AddTriangle(center, mL, mC);
                Roads.AddTriangle(center, mC, mR);
                Roads.AddTriangleUV(
                    new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(1f, 0f)
                );
                Roads.AddTriangleUV(
                    new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f)
                );
                Roads.AddTriangleCellData(indices, Weights1);
                Roads.AddTriangleCellData(indices, Weights1);
            }
            else
            {
                TriangulateRoadEdge(center, mL, mR, index);
            }
        }

        private void TriangulateRoadEdge(
            Vector3 center, Vector3 mL, Vector3 mR, float index
        )
        {
            Roads.AddTriangle(center, mL, mR);
            Roads.AddTriangleUV(
                new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f)
            );
            Vector3 indices;
            indices.x = indices.y = indices.z = index;
            Roads.AddTriangleCellData(indices, Weights1);
        }

        private void TriangulateRoadSegment(
            Vector3 v1, Vector3 v2, Vector3 v3,
            Vector3 v4, Vector3 v5, Vector3 v6,
            Color w1, Color w2, Vector3 indices
        )
        {
            Roads.AddQuad(v1, v2, v4, v5);
            Roads.AddQuad(v2, v3, v5, v6);
            Roads.AddQuadUV(0f, 1f, 0f, 0f);
            Roads.AddQuadUV(1f, 0f, 0f, 0f);
            Roads.AddQuadCellData(indices, w1, w2);
            Roads.AddQuadCellData(indices, w1, w2);
        }
    }
}