using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace Secyud.Ugf.Unity.Ui
{
    public class CircleImage : Image
    {
        [SerializeField, Range(1, 512)] private float _radius;
        [SerializeField, Range(5, 16)] private int _triangleNum = 5;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            Rect rect = GetPixelAdjustedRect();
            Vector4 ov = new(rect.x, rect.y,
                rect.x + rect.width, rect.y + rect.height);
            Vector4 ouv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;

            Color color32 = color;

            vh.Clear();

            float radius = Mathf.Min((ov.z - ov.x) / 2,
                (ov.w - ov.y) / 2, _radius);

            // the ui radius
            float radiusUvX = radius / (ov.z - ov.x);
            float radiusUvY = radius / (ov.w - ov.y);

            Vector4 iv = new(
                ov.x + radius, ov.y + radius,
                ov.z - radius, ov.w - radius);
            Vector4 iuv = new(
                ouv.x + radiusUvX, ouv.y + radiusUvY,
                ouv.z - radiusUvX, ouv.w - radiusUvY);

            const float tolerance = 0.001f;

            bool hor = radiusUvX > tolerance;
            bool ver = radiusUvY > tolerance;

            List<Vector2> positions = ListPool<Vector2>.Get();
            List<Vector2> uvs = ListPool<Vector2>.Get();
            List<int> indices = ListPool<int>.Get();

            // clockwise, start with the left of top; 
            AddVertex(iv.x, ov.w, iuv.x, ouv.w, 0, false);
            AddVertex(iv.z, ov.w, iuv.z, ouv.w, 0, hor);
            AddVertex(ov.z, iv.w, ouv.z, iuv.w, 1, false);
            AddVertex(ov.z, iv.y, ouv.z, iuv.y, 2, ver);
            AddVertex(iv.z, ov.y, iuv.z, ouv.y, 3, false);
            AddVertex(iv.x, ov.y, iuv.x, ouv.y, 4, hor);
            AddVertex(ov.x, iv.y, ouv.x, iuv.y, 5, false);
            AddVertex(ov.x, iv.w, ouv.x, iuv.w, 6, ver);

            // the center rect, start with the left top; 
            AddVertex(iv.x, iv.w, iuv.x, iuv.w, 7, false);
            positions.Add(new Vector2(iv.x, iv.w));
            uvs.Add(new Vector2(iuv.x, iuv.w));
            AddVertex(iv.z, iv.w, iuv.z, iuv.w, 8, hor);
            positions.Add(new Vector2(iv.z, iv.w));
            uvs.Add(new Vector2(iuv.z, iuv.w));
            AddVertex(iv.z, iv.y, iuv.z, iuv.y, 9, ver);
            positions.Add(new Vector2(iv.z, iv.y));
            uvs.Add(new Vector2(iuv.z, iuv.y));
            AddVertex(iv.x, iv.y, iuv.x, iuv.y, 10, hor);
            positions.Add(new Vector2(iv.x, iv.y));
            uvs.Add(new Vector2(iuv.x, iuv.y));

            if (!ver)
            {
                vh.AddRectangle(indices[8], indices[7], indices[6], indices[11]);
                vh.AddRectangle(indices[3], indices[2], indices[9], indices[10]);
            }

            if (!hor)
            {
                vh.AddRectangle(indices[1], indices[0], indices[8], indices[9]);
                vh.AddRectangle(indices[5], indices[4], indices[10], indices[11]);
            }

            if (!ver && !hor)
            {
                vh.AddRectangle(indices[11], indices[10], indices[9], indices[8]);
            }

            float degreeDelta = Mathf.PI / 2 / _triangleNum;

            for (int i = 0; i < 4; i++)
            {
                float startDegree = (1 - i) * Mathf.PI / 2;
                int indexStart = vh.currentVertCount - 1;
                int indexPre = indices[2 * i];

                for (int j = 1; j < _triangleNum; j++)
                {
                    float currentDegree = startDegree + j * degreeDelta;
                    Vector2 rPosition = GetPosition(currentDegree, positions[i]);
                    Vector2 rPositionUv = GetPositionUv(currentDegree, uvs[i]);
                    vh.AddVert(rPosition, color32, rPositionUv);
                    vh.AddTriangle(indices[i + 8], indexPre, indexStart + j);
                    indexPre = indexStart + j;
                }

                vh.AddTriangle(indices[i + 8], indexPre, indices[(2 * i + 7) % 8]);
            }

            ListPool<Vector2>.Release(positions);
            ListPool<Vector2>.Release(uvs);
            ListPool<int>.Release(indices);
            return;

            void AddVertex(float vx, float vy, float uvx, float uvy, int index, bool indexOnly)
            {
                if (indexOnly)
                {
                    indices.Add(indices[index]);
                }
                else
                {
                    indices.Add(vh.currentVertCount);
                    vh.AddVert(new Vector2(vx, vy),
                        color32, new Vector2(uvx, uvy));
                }
            }

            Vector2 GetPosition(float degree, Vector2 center)
            {
                return new Vector2(
                    Mathf.Cos(degree) * radius + center.x,
                    Mathf.Sin(degree) * radius + center.y);
            }

            Vector2 GetPositionUv(float degree, Vector2 centerUv)
            {
                return new Vector2(
                    Mathf.Cos(degree) * radiusUvX + centerUv.x,
                    Mathf.Sin(degree) * radiusUvY + centerUv.y);
            }
        }
    }
}