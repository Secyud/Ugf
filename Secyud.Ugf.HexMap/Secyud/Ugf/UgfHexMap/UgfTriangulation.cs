using Secyud.Ugf.HexMap;
using Secyud.Ugf.HexUtilities;
using UnityEngine;

namespace Secyud.Ugf.UgfHexMap
{
    public readonly struct UgfTriangulation
    {
        private static readonly Vector3 Fh = new(0f, 0.1f, 0f);
        private static readonly Color Weights1 = new(1f, 0f, 0f);
        private static readonly Color Weights2 = new(0f, 1f, 0f);
        private static readonly Color Weights3 = new(0f, 0f, 1f);

        private readonly HexChunk _chunk;

        private HexMesh Terrain => _chunk.Get(0);
        private HexMesh Rivers => _chunk.Get(1);
        private HexMesh Water => _chunk.Get(2);
        private HexMesh WaterShore => _chunk.Get(3);
        private HexMesh Estuaries => _chunk.Get(4);
        private HexMesh Walls => _chunk.Get(5);
        private HexMesh Roads => _chunk.Get(6);
        private Transform Bridge => _chunk.GetFeature(0);
        private Transform WallTower => _chunk.GetFeature(1);

        public UgfTriangulation(HexChunk chunk)
        {
            _chunk = chunk;
        }

        public void TriangulateCell(UgfCell cell)
        {
            for (HexDirection d = HexDirection.Ne; d <= HexDirection.Nw; d++)
            {
                Triangulate(d, cell);
            }

            if (!cell.IsUnderwater)
            {
                AddSpecialFeature(cell, cell.Position);
            }
        }

        private void Triangulate(HexDirection direction, UgfCell cell)
        {
            Vector3 center = cell.Position;
            EdgeVertices e = EdgeVertices.Create(
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
            }

            if (direction <= HexDirection.Se)
            {
                TriangulateConnection(direction, cell, e);
            }

            if (cell.IsUnderwater)
            {
                TriangulateWater(direction, cell, center);
            }
        }

        private void TriangulateWater(
            HexDirection direction, UgfCell cell, Vector3 center)
        {
            center.y = cell.WaterSurfaceY;

            UgfCell neighbor = cell.GetNeighbor(direction);
            if (neighbor is { IsUnderwater: false })
            {
                TriangulateWaterShore(direction, cell, neighbor, center);
            }
            else
            {
                TriangulateOpenWater(direction, cell, neighbor, center);
            }
        }

        private void TriangulateOpenWater(
            HexDirection direction, UgfCell cell, UgfCell neighbor, Vector3 center
        )
        {
            Vector3 c1 = center + HexMetrics.GetFirstWaterCorner(direction);
            Vector3 c2 = center + HexMetrics.GetSecondWaterCorner(direction);

            Water.AddTriangle(center, c1, c2);
            Vector3 indices;
            indices.x = indices.y = indices.z = cell.Index;
            Water.AddTriangleCellData(indices, Weights1);

            if (direction <= HexDirection.Se && neighbor != null)
            {
                Vector3 bridge = HexMetrics.GetWaterBridge(direction);
                Vector3 e1 = c1 + bridge;
                Vector3 e2 = c2 + bridge;

                Water.AddQuad(c1, c2, e1, e2);
                indices.y = neighbor.Index;
                Water.AddQuadCellData(indices, Weights1, Weights2);

                if (direction <= HexDirection.E)
                {
                    UgfCell nextNeighbor = cell.GetNeighbor(direction.Next());
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
            HexDirection direction, UgfCell cell, UgfCell neighbor, Vector3 center
        )
        {
            EdgeVertices e1 = EdgeVertices.Create(
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

            Vector3 center2 = neighbor.Position;
            center2.y = center.y;

            EdgeVertices e2 = EdgeVertices.Create(
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

            UgfCell nextNeighbor = cell.GetNeighbor(direction.Next());
            if (nextNeighbor != null)
            {
                Vector3 center3 = nextNeighbor.Position;
                Vector3 v3 = center3 + (nextNeighbor.IsUnderwater
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
            HexDirection direction, UgfCell cell, Vector3 center, EdgeVertices e
        )
        {
            TriangulateEdgeFan(center, e, cell.Index);

            if (cell.HasRoads)
            {
                Vector2 interpolators = GetRoadInterpolators(direction, cell);
                TriangulateRoad(
                    center,
                    Vector3.Lerp(center, e.V1, interpolators.x),
                    Vector3.Lerp(center, e.V5, interpolators.y),
                    e, cell.HasRoadThroughEdge(direction), cell.Index
                );
            }
        }

        private Vector2 GetRoadInterpolators(HexDirection direction, UgfCell cell)
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
            HexDirection direction, UgfCell cell, Vector3 center, EdgeVertices e
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

            EdgeVertices m = EdgeVertices.Create(
                Vector3.Lerp(center, e.V1, 0.5f),
                Vector3.Lerp(center, e.V5, 0.5f)
            );

            TriangulateEdgeStrip(
                m, Weights1, cell.Index,
                e, Weights1, cell.Index
            );
            TriangulateEdgeFan(center, m, cell.Index);
        }

        private void TriangulateRoadAdjacentToRiver(
            HexDirection direction, UgfCell cell, Vector3 center, EdgeVertices e
        )
        {
            bool hasRoadThroughEdge = cell.HasRoadThroughEdge(direction);
            bool previousHasRiver = cell.HasRiverThroughEdge(direction.Previous());
            bool nextHasRiver = cell.HasRiverThroughEdge(direction.Next());
            Vector2 interpolators = GetRoadInterpolators(direction, cell);
            Vector3 roadCenter = center;

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
                    AddBridge(roadCenter, center - corner * 0.5f);

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

                Vector3 offset = HexMetrics.GetSolidEdgeMiddle(direction) *
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

                Vector3 offset = HexMetrics.GetSolidEdgeMiddle(middle);
                roadCenter += offset * 0.25f;
                if (
                    direction == middle &&
                    cell.HasRoadThroughEdge(direction.Opposite())
                )
                    AddBridge(
                        roadCenter,
                        center - offset * (HexMetrics.InnerToOuter * 0.7f)
                    );
            }

            Vector3 mL = Vector3.Lerp(roadCenter, e.V1, interpolators.x);
            Vector3 mR = Vector3.Lerp(roadCenter, e.V5, interpolators.y);
            TriangulateRoad(roadCenter, mL, mR, e, hasRoadThroughEdge, cell.Index);
            if (previousHasRiver) TriangulateRoadEdge(roadCenter, center, mL, cell.Index);

            if (nextHasRiver) TriangulateRoadEdge(roadCenter, mR, center, cell.Index);
        }

        private void TriangulateWithRiverBeginOrEnd(
            // ReSharper disable once UnusedParameter.Local
            HexDirection direction, UgfCell cell, Vector3 center, EdgeVertices e
        )
        {
            EdgeVertices m = EdgeVertices.Create(
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
                bool reversed = cell.HasIncomingRiver;
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
            HexDirection direction, UgfCell cell, Vector3 center, EdgeVertices e
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

            EdgeVertices m = EdgeVertices.Create(
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
                bool reversed = cell.IncomingRiver == direction;
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
            HexDirection direction, UgfCell cell, EdgeVertices e1
        )
        {
            UgfCell neighbor = cell.GetNeighbor(direction);
            if (neighbor == null) return;

            Vector3 bridge = HexMetrics.GetBridge(direction);
            bridge.y = neighbor.Position.y - cell.Position.y;
            EdgeVertices e2 = EdgeVertices.Create(
                e1.V1 + bridge,
                e1.V5 + bridge
            );

            bool hasRiver = cell.HasRiverThroughEdge(direction);
            bool hasRoad = cell.HasRoadThroughEdge(direction);

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

            AddWall(e1, cell, e2, neighbor, hasRiver, hasRoad);

            UgfCell nextNeighbor = cell.GetNeighbor(direction.Next());
            if (direction <= HexDirection.E && nextNeighbor != null)
            {
                Vector3 v5 = e1.V5 + HexMetrics.GetBridge(direction.Next());
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
            float t = (waterY - y2) / (y1 - y2);
            v3 = Vector3.Lerp(v3, v1, t);
            v4 = Vector3.Lerp(v4, v2, t);
            Rivers.AddQuadUnperturbed(v1, v2, v3, v4);
            Rivers.AddQuadUV(0f, 1f, 0.8f, 1f);
            Rivers.AddQuadCellData(indices, Weights1, Weights2);
        }

        private void TriangulateCorner(
            Vector3 bottom, UgfCell bottomCell,
            Vector3 left, UgfCell leftCell,
            Vector3 right, UgfCell rightCell
        )
        {
            HexEdgeType leftEdgeType = bottomCell.GetEdgeType(leftCell);
            HexEdgeType rightEdgeType = bottomCell.GetEdgeType(rightCell);

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

            AddWall(bottom, bottomCell, left, leftCell, right, rightCell);
        }

        private void TriangulateEdgeTerraces(
            EdgeVertices begin, UgfCell beginCell,
            EdgeVertices end, UgfCell endCell,
            bool hasRoad
        )
        {
            EdgeVertices e2 = EdgeVertices.TerraceLerp(begin, end, 1);
            Color w2 = HexMetrics.TerraceLerp(Weights1, Weights2, 1);
            float i1 = beginCell.Index;
            float i2 = endCell.Index;

            TriangulateEdgeStrip(begin, Weights1, i1, e2, w2, i2, hasRoad);

            for (int i = 2; i < HexMetrics.TerraceSteps; i++)
            {
                EdgeVertices e1 = e2;
                Color w1 = w2;
                e2 = EdgeVertices.TerraceLerp(begin, end, i);
                w2 = HexMetrics.TerraceLerp(Weights1, Weights2, i);
                TriangulateEdgeStrip(e1, w1, i1, e2, w2, i2, hasRoad);
            }

            TriangulateEdgeStrip(e2, w2, i1, end, Weights2, i2, hasRoad);
        }

        private void TriangulateCornerTerraces(
            Vector3 begin, UgfCell beginCell,
            Vector3 left, UgfCell leftCell,
            Vector3 right, UgfCell rightCell
        )
        {
            Vector3 v3 = HexMetrics.TerraceLerp(begin, left, 1);
            Vector3 v4 = HexMetrics.TerraceLerp(begin, right, 1);
            Color w3 = HexMetrics.TerraceLerp(Weights1, Weights2, 1);
            Color w4 = HexMetrics.TerraceLerp(Weights1, Weights3, 1);
            Vector3 indices;
            indices.x = beginCell.Index;
            indices.y = leftCell.Index;
            indices.z = rightCell.Index;

            Terrain.AddTriangle(begin, v3, v4);
            Terrain.AddTriangleCellData(indices, Weights1, w3, w4);

            for (int i = 2; i < HexMetrics.TerraceSteps; i++)
            {
                Vector3 v1 = v3;
                Vector3 v2 = v4;
                Color w1 = w3;
                Color w2 = w4;
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
            Vector3 begin, UgfCell beginCell,
            Vector3 left, UgfCell leftCell,
            Vector3 right, UgfCell rightCell
        )
        {
            float b = 1f / (rightCell.Elevation - beginCell.Elevation);
            if (b < 0) b = -b;

            Vector3 boundary = Vector3.Lerp(
                HexMetrics.Perturb(begin), HexMetrics.Perturb(right), b
            );
            Color boundaryWeights = Color.Lerp(Weights1, Weights3, b);
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
            Vector3 begin, UgfCell beginCell,
            Vector3 left, UgfCell leftCell,
            Vector3 right, UgfCell rightCell
        )
        {
            float b = 1f / (leftCell.Elevation - beginCell.Elevation);
            if (b < 0) b = -b;

            Vector3 boundary = Vector3.Lerp(
                HexMetrics.Perturb(begin), HexMetrics.Perturb(left), b
            );
            Color boundaryWeights = Color.Lerp(Weights1, Weights2, b);
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
            Vector3 v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, 1));
            Color w2 = HexMetrics.TerraceLerp(beginWeights, leftWeights, 1);

            Terrain.AddTriangleUnperturbed(HexMetrics.Perturb(begin), v2, boundary);
            Terrain.AddTriangleCellData(indices, beginWeights, w2, boundaryWeights);

            for (int i = 2; i < HexMetrics.TerraceSteps; i++)
            {
                Vector3 v1 = v2;
                Color w1 = w2;
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
                Vector3 mC = Vector3.Lerp(mL, mR, 0.5f);
                TriangulateRoadSegment(
                    mL, mC, mR, e.V2, e.V3, e.V4,
                    Weights1, Weights1, indices
                );
                TriangulateRoadWideEdge(center, mL, mC, mR, indices);
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
            center += Fh;
            mL += Fh;
            mR += Fh;
            Roads.AddTriangle(center, mL, mR);
            Roads.AddTriangleUV(
                new Vector2(0.5f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f)
            );
            Vector3 indices;
            indices.x = indices.y = indices.z = index;
            Roads.AddTriangleCellData(indices, Weights1);
        }

        private void TriangulateRoadWideEdge(
            Vector3 center, Vector3 mL, Vector3 mC, Vector3 mR, Vector3 indices
        )
        {
            center += Fh;
            mL += Fh;
            mC += Fh;
            mR += Fh;
            Roads.AddTriangle(center, mL, mC);
            Roads.AddTriangle(center, mC, mR);
            Roads.AddTriangleUV(
                new Vector2(1f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f)
            );
            Roads.AddTriangleUV(
                new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f)
            );
            Roads.AddTriangleCellData(indices, Weights1);
            Roads.AddTriangleCellData(indices, Weights1);
        }

        private void TriangulateRoadSegment(
            Vector3 v1, Vector3 v2, Vector3 v3,
            Vector3 v4, Vector3 v5, Vector3 v6,
            Color w1, Color w2, Vector3 indices
        )
        {
            v1 += Fh;
            v2 += Fh;
            v3 += Fh;
            v4 += Fh;
            v5 += Fh;
            v6 += Fh;
            Roads.AddQuad(v1, v2, v4, v5);
            Roads.AddQuad(v2, v3, v5, v6);
            Roads.AddQuadUV(0f, 1f, 0f, 1f);
            Roads.AddQuadUV(1f, 0f, 0f, 1f);
            Roads.AddQuadCellData(indices, w1, w2);
            Roads.AddQuadCellData(indices, w1, w2);
        }


        public void AddWall(
            EdgeVertices near, UgfCell nearCell,
            EdgeVertices far, UgfCell farCell,
            bool hasRiver, bool hasRoad)
        {
            if (nearCell.Walled != farCell.Walled &&
                !nearCell.IsUnderwater && !farCell.IsUnderwater &&
                nearCell.GetEdgeType(farCell) != HexEdgeType.Cliff)
            {
                AddWallSegment(near.V1, far.V1, near.V2, far.V2);
                if (hasRiver || hasRoad)
                {
                    AddWallCap(near.V2, far.V2);
                    AddWallCap(far.V4, near.V4);
                }
                else
                {
                    AddWallSegment(near.V2, far.V2, near.V3, far.V3);
                    AddWallSegment(near.V3, far.V3, near.V4, far.V4);
                }

                AddWallSegment(near.V4, far.V4, near.V5, far.V5);
            }
        }

        public void AddWall(
            Vector3 c1, UgfCell cell1,
            Vector3 c2, UgfCell cell2,
            Vector3 c3, UgfCell cell3)
        {
            if (cell1.Walled)
            {
                if (cell2.Walled)
                {
                    if (!cell3.Walled)
                        AddWallSegment(c3, cell3, c1, cell1, c2, cell2);
                }
                else if (cell3.Walled)
                {
                    AddWallSegment(c2, cell2, c3, cell3, c1, cell1);
                }
                else
                {
                    AddWallSegment(c1, cell1, c2, cell2, c3, cell3);
                }
            }
            else if (cell2.Walled)
            {
                if (cell3.Walled)
                    AddWallSegment(c1, cell1, c2, cell2, c3, cell3);
                else
                    AddWallSegment(c2, cell2, c3, cell3, c1, cell1);
            }
            else if (cell3.Walled)
            {
                AddWallSegment(c3, cell3, c1, cell1, c2, cell2);
            }
        }

        private void AddWallSegment(
            Vector3 nearLeft, Vector3 farLeft, Vector3 nearRight, Vector3 farRight,
            bool addTower = false)
        {
            nearLeft = HexMetrics.Perturb(nearLeft);
            farLeft = HexMetrics.Perturb(farLeft);
            nearRight = HexMetrics.Perturb(nearRight);
            farRight = HexMetrics.Perturb(farRight);

            Vector3 left = HexMetrics.WallLerp(nearLeft, farLeft);
            Vector3 right = HexMetrics.WallLerp(nearRight, farRight);

            Vector3 leftThicknessOffset =
                HexMetrics.WallThicknessOffset(nearLeft, farLeft);
            Vector3 rightThicknessOffset =
                HexMetrics.WallThicknessOffset(nearRight, farRight);

            float leftTop = left.y + HexMetrics.WallHeight;
            float rightTop = right.y + HexMetrics.WallHeight;

            Vector3 v3, v4;
            Vector3 v1 = v3 = left - leftThicknessOffset;
            Vector3 v2 = v4 = right - rightThicknessOffset;
            v3.y = leftTop;
            v4.y = rightTop;
            Walls.AddQuadUnperturbed(v1, v2, v3, v4);

            Vector3 t1 = v3, t2 = v4;

            v1 = v3 = left + leftThicknessOffset;
            v2 = v4 = right + rightThicknessOffset;
            v3.y = leftTop;
            v4.y = rightTop;
            Walls.AddQuadUnperturbed(v2, v1, v4, v3);

            Walls.AddQuadUnperturbed(t1, t2, v3, v4);

            if (addTower)
            {
                Transform towerInstance = WallTower.Instantiate(_chunk.Container);
                Transform towerTransform = towerInstance.transform;
                towerTransform.localPosition = (left + right) * 0.5f;
                Vector3 rightDirection = right - left;
                rightDirection.y = 0f;
                towerTransform.right = rightDirection;
            }
        }

        private void AddWallSegment(
            Vector3 pivot, UgfCell pivotCell,
            Vector3 left, UgfCell leftCell,
            Vector3 right, UgfCell rightCell)
        {
            if (pivotCell.IsUnderwater) return;

            bool hasLeftWall = !leftCell.IsUnderwater &&
                               pivotCell.GetEdgeType(leftCell) != HexEdgeType.Cliff;
            bool hasRightWall = !rightCell.IsUnderwater &&
                                pivotCell.GetEdgeType(rightCell) != HexEdgeType.Cliff;

            if (hasLeftWall)
            {
                if (hasRightWall)
                {
                    bool hasTower = false;
                    if (leftCell.Elevation == rightCell.Elevation)
                    {
                        HexHash hash = HexMetrics.SampleHashGrid(
                            (pivot + left + right) * (1f / 3f)
                        );
                        hasTower = hash.E < HexMetrics.WallTowerThreshold;
                    }

                    AddWallSegment(pivot, left, pivot, right, hasTower);
                }
                else if (leftCell.Elevation < rightCell.Elevation)
                {
                    AddWallWedge(pivot, left, right);
                }
                else
                {
                    AddWallCap(pivot, left);
                }
            }
            else if (hasRightWall)
            {
                if (rightCell.Elevation < leftCell.Elevation)
                    AddWallWedge(right, pivot, left);
                else
                    AddWallCap(right, pivot);
            }
        }

        private void AddWallCap(Vector3 near, Vector3 far)
        {
            near = HexMetrics.Perturb(near);
            far = HexMetrics.Perturb(far);

            Vector3 center = HexMetrics.WallLerp(near, far);
            Vector3 thickness = HexMetrics.WallThicknessOffset(near, far);

            Vector3 v3, v4;

            Vector3 v1 = v3 = center - thickness;
            Vector3 v2 = v4 = center + thickness;
            v3.y = v4.y = center.y + HexMetrics.WallHeight;
            Walls.AddQuadUnperturbed(v1, v2, v3, v4);
        }

        private void AddWallWedge(Vector3 near, Vector3 far, Vector3 point)
        {
            near = HexMetrics.Perturb(near);
            far = HexMetrics.Perturb(far);
            point = HexMetrics.Perturb(point);

            Vector3 center = HexMetrics.WallLerp(near, far);
            Vector3 thickness = HexMetrics.WallThicknessOffset(near, far);

            Vector3 v3, v4;
            Vector3 pointTop = point;
            point.y = center.y;

            Vector3 v1 = v3 = center - thickness;
            Vector3 v2 = v4 = center + thickness;
            v3.y = v4.y = pointTop.y = center.y + HexMetrics.WallHeight;

            Walls.AddQuadUnperturbed(v1, point, v3, pointTop);
            Walls.AddQuadUnperturbed(point, v2, pointTop, v4);
            Walls.AddTriangleUnperturbed(pointTop, v3, v4);
        }


        public void AddBridge(Vector3 roadCenter1, Vector3 roadCenter2)
        {
            roadCenter1 = HexMetrics.Perturb(roadCenter1);
            roadCenter2 = HexMetrics.Perturb(roadCenter2);
            Transform instance = Bridge.Instantiate(_chunk.Container);
            instance.localPosition = (roadCenter1 + roadCenter2) * 0.5f;
            instance.forward = roadCenter2 - roadCenter1;
            float length = Vector3.Distance(roadCenter1, roadCenter2);
            instance.localScale = new Vector3(
                1f, 1f, length * (1f / HexMetrics.BridgeDesignLength)
            );
        }

        public void AddSpecialFeature(UgfCell cell, Vector3 position)
        {
            if (!cell.FeaturePrefab)
            {
                return;
            }

            HexHash hash = HexMetrics.SampleHashGrid(position);
            Transform instance = cell.FeaturePrefab.Instantiate(_chunk.Container);
            instance.localPosition = HexMetrics.Perturb(position);
            instance.localRotation = Quaternion.Euler(0f, 360f * hash.E, 0f);
        }
    }
}