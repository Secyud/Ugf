using System;
using UnityEngine.UI;

namespace UnityEngine
{
    public static class UgfRectTransformExtensions
    {
        public static void CheckBoundary(this RectTransform transform)
        {
            Vector2 lb = transform.GetLeftBottomBias();
            Vector2 position = transform.anchoredPosition;
            Rect rect = transform.rect;
            float x = Math.Min(position.x, Screen.currentResolution.width - lb.x - rect.width);
            float y = Math.Max(position.y, -lb.y - Screen.currentResolution.height);
            if (x < 0) x = 0;
            if (y > 0) y = 0;
            transform.anchoredPosition = new Vector2(x, y);
        }

        public static bool MouseIn(this RectTransform transform)
        {
            Vector2 lb = transform.GetLeftBottomBias() + transform.anchoredPosition;
            Rect rect = transform.rect;

            float x = Input.mousePosition.x / Screen.width * Screen.currentResolution.width;
            float y = Input.mousePosition.y / Screen.height * Screen.currentResolution.height -
                      Screen.currentResolution.height;

            return x < lb.x + rect.width &&
                   x > lb.x &&
                   y > lb.y &&
                   y < lb.y + rect.height;
        }

        private static Vector2 GetLeftBottomBias(this RectTransform transform)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform);
            RectTransform pTransform = transform.parent.GetComponent<RectTransform>();
            Rect rect = transform.rect;
            Rect pRect = pTransform.rect;
            Vector2 pivot = transform.pivot;
            Vector2 anchorMin = transform.anchorMin;
            Vector2 anchorMax = transform.anchorMax;
            float x = anchorMin.x * pRect.width +
                      (Mathf.Approximately(anchorMax.x, anchorMin.x)
                          ? -pivot.x * rect.width
                          : (anchorMax.x - anchorMin.x) * pivot.x * pRect.width - rect.width / 2);
            float y = anchorMin.y * pRect.height - Screen.currentResolution.height +
                      (Mathf.Approximately(anchorMax.y, anchorMin.y)
                          ? -pivot.y * rect.height
                          : (anchorMax.y - anchorMin.y) * pivot.y * pRect.height - rect.height / 2);
            return new Vector2(x, y);
        }
    }
}