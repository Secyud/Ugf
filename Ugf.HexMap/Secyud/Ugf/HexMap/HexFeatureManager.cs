#region

using Secyud.Ugf.HexMap.Utilities;
using UnityEngine;

#endregion

namespace Secyud.Ugf.HexMap
{
    /// <summary>
    ///     Component that manages the map feature visualizations for a hex grid chunk.
    /// </summary>
    public class HexFeatureManager : MonoBehaviour
    {
        [SerializeField] private HexMesh Walls;
        [SerializeField] private Transform WallTower;
        [SerializeField] private Transform Bridge;

        private Transform _container;

        public HexGrid Grid { get; set; }

        /// <summary>
        ///     Clear all features.
        /// </summary>
        public void Clear()
        {
            if (_container) Destroy(_container.gameObject);

            _container = new GameObject("Features Container").transform;
            _container.SetParent(transform, false);
            Walls.Clear();
        }

        /// <summary>
        ///     Apply triangulation.
        /// </summary>
        public void Apply()
        {
            Walls.Apply();
        }

        // private Transform PickPrefab(
        //     List<Transform>[,] collection,
        //     HexCell cell,float pickValue,float choice)
        // {
        //     int resourceTypeCount = cell.ResourceTypeCount;
        //     int[] level = new int[resourceTypeCount];
        //     level[0] = cell.GetResourceLevel(0);
        //     for (int i = 1; i < resourceTypeCount; i++)
        //         level[i] = cell.GetResourceLevel(i) + level[i - 1];
        //     pickValue *= level.Last();
        //
        //     if (level.Last() == 0)
        //         return null;
        //         
        //     for ( int i = 0; i < resourceTypeCount; i++)
        //         if (level[i] > pickValue)
        //             return collection[i,level[i]].Pick(choice);
        //     return collection[collection.GetLength(0)-1,collection.GetLength(1)-1].Pick(choice);
        // }

        /// <summary>
        ///     Add a bridge between two road centers.
        /// </summary>
        /// <param name="roadCenter1">Center position of first road.</param>
        /// <param name="roadCenter2">Center position of second road.</param>
        public void AddBridge(Vector3 roadCenter1, Vector3 roadCenter2)
        {
            roadCenter1 = HexMetrics.Perturb(roadCenter1);
            roadCenter2 = HexMetrics.Perturb(roadCenter2);
            var instance = Instantiate(Bridge, _container, false);
            instance.localPosition = (roadCenter1 + roadCenter2) * 0.5f;
            instance.forward = roadCenter2 - roadCenter1;
            var length = Vector3.Distance(roadCenter1, roadCenter2);
            instance.localScale = new Vector3(
                1f, 1f, length * (1f / HexMetrics.BridgeDesignLength)
            );
        }

        /// <summary>
        ///     Add a special feature for a cell.
        /// </summary>
        /// <param name="cell">Add cell feature</param>
        /// <param name="position">Feature position.</param>
        public void AddFeature(HexCell cell, Vector3 position)
        {
            TryAddFeature(Grid.HexMapManager.GetFeature(cell), position);
        }

        /// <summary>
        ///     Add a special feature for a cell.
        /// </summary>
        /// <param name="cell">Add cell feature</param>
        /// <param name="position">Feature position.</param>
        public void AddSpecialFeature(HexCell cell, Vector3 position)
        {
            TryAddFeature(Grid.HexMapManager.GetSpecialFeature(cell), position);
        }

        private void TryAddFeature(Transform prefab, Vector3 position)
        {
            if (!prefab) return;
            var hash = HexMetrics.SampleHashGrid(position);
            var instance = Instantiate(prefab, _container, false);
            instance.localPosition = HexMetrics.Perturb(position);
            instance.localRotation = Quaternion.Euler(0f, 360f * hash.E, 0f);
        }

        /// <summary>
        ///     Add a wall along the edge between two cells.
        /// </summary>
        /// <param name="near">Near edge.</param>
        /// <param name="nearCell">Near cell.</param>
        /// <param name="far">Far edge.</param>
        /// <param name="farCell">Far cell.</param>
        /// <param name="hasRiver">Whether a river crosses the edge.</param>
        /// <param name="hasRoad">Whether a road crosses the edge.</param>
        public void AddWall(
            EdgeVertices near, HexCell nearCell,
            EdgeVertices far, HexCell farCell,
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

        /// <summary>
        ///     Add a call though the corner where three cells meet.
        /// </summary>
        /// <param name="c1">First corner position.</param>
        /// <param name="cell1">First corner cell.</param>
        /// <param name="c2">Second corner position.</param>
        /// <param name="cell2">Second corner cell.</param>
        /// <param name="c3">Third corner position.</param>
        /// <param name="cell3">Third corner cell.</param>
        public void AddWall(
            Vector3 c1, HexCell cell1,
            Vector3 c2, HexCell cell2,
            Vector3 c3, HexCell cell3)
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

            var left = HexMetrics.WallLerp(nearLeft, farLeft);
            var right = HexMetrics.WallLerp(nearRight, farRight);

            var leftThicknessOffset =
                HexMetrics.WallThicknessOffset(nearLeft, farLeft);
            var rightThicknessOffset =
                HexMetrics.WallThicknessOffset(nearRight, farRight);

            var leftTop = left.y + HexMetrics.WallHeight;
            var rightTop = right.y + HexMetrics.WallHeight;

            Vector3 v3, v4;
            var v1 = v3 = left - leftThicknessOffset;
            var v2 = v4 = right - rightThicknessOffset;
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
                var towerInstance = Instantiate(WallTower, _container, false);
                var towerTransform = towerInstance.transform;
                towerTransform.localPosition = (left + right) * 0.5f;
                var rightDirection = right - left;
                rightDirection.y = 0f;
                towerTransform.right = rightDirection;
            }
        }

        private void AddWallSegment(
            Vector3 pivot, HexCell pivotCell,
            Vector3 left, HexCell leftCell,
            Vector3 right, HexCell rightCell)
        {
            if (pivotCell.IsUnderwater) return;

            var hasLeftWall = !leftCell.IsUnderwater &&
                              pivotCell.GetEdgeType(leftCell) != HexEdgeType.Cliff;
            var hasRightWall = !rightCell.IsUnderwater &&
                               pivotCell.GetEdgeType(rightCell) != HexEdgeType.Cliff;

            if (hasLeftWall)
            {
                if (hasRightWall)
                {
                    var hasTower = false;
                    if (leftCell.Elevation == rightCell.Elevation)
                    {
                        var hash = HexMetrics.SampleHashGrid(
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

            var center = HexMetrics.WallLerp(near, far);
            var thickness = HexMetrics.WallThicknessOffset(near, far);

            Vector3 v3, v4;

            var v1 = v3 = center - thickness;
            var v2 = v4 = center + thickness;
            v3.y = v4.y = center.y + HexMetrics.WallHeight;
            Walls.AddQuadUnperturbed(v1, v2, v3, v4);
        }

        private void AddWallWedge(Vector3 near, Vector3 far, Vector3 point)
        {
            near = HexMetrics.Perturb(near);
            far = HexMetrics.Perturb(far);
            point = HexMetrics.Perturb(point);

            var center = HexMetrics.WallLerp(near, far);
            var thickness = HexMetrics.WallThicknessOffset(near, far);

            Vector3 v3, v4;
            var pointTop = point;
            point.y = center.y;

            var v1 = v3 = center - thickness;
            var v2 = v4 = center + thickness;
            v3.y = v4.y = pointTop.y = center.y + HexMetrics.WallHeight;

            Walls.AddQuadUnperturbed(v1, point, v3, pointTop);
            Walls.AddQuadUnperturbed(point, v2, pointTop, v4);
            Walls.AddTriangleUnperturbed(pointTop, v3, v4);
        }
    }
}