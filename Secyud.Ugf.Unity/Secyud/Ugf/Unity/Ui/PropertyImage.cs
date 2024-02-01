using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.Unity.Ui
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class PropertyImage : Image
    {
        [SerializeField] private float[] _values;
        [SerializeField] private Color _reverseColor;
        [SerializeField] private float _startDegree;

        public float[] Values => _values;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (_values is null) return;
            int length = _values.Length;
            if (length < 3) return;

            Rect r = GetPixelAdjustedRect();

            Color colorF = color;
            Color colorR = _reverseColor;


            Vector2 center = new(r.x + r.width / 2, r.y + r.height / 2);
            Vector2 centerUv = new(0.5f, 0.5f);
            vh.AddVert(center,
                new Color(1, 1, 1, (colorF.a + colorR.a) / 2),
                centerUv);
            float maxValue = _values.Max(Mathf.Abs);
            if (Mathf.Approximately(maxValue, 0)) maxValue = 1;
            
            float normalizedFactor = 1 / maxValue / 2;
            float radius = Mathf.Min(r.width, r.height);
            float degreeDelta = Mathf.PI * 2 / length;
            float start = _startDegree / 180 * Mathf.PI;
            for (int i = 0; i < length; i++)
            {
                float d = start + degreeDelta * i;
                Vector2 direction = normalizedFactor * Mathf.Abs(_values[i]) *
                                    new Vector2(Mathf.Cos(d), Mathf.Sin(d));
                vh.AddVert(center + direction * radius,
                    _values[i] < 0 ? colorR : colorF,
                    centerUv + direction);
            }

            for (int i = 0; i < length; i++)
            {
                vh.AddTriangle(0, i + 1, (i + 1) % length + 1);
            }
        }
    }
}