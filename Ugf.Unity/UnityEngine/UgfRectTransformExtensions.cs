using UnityEngine.UI;

namespace UnityEngine
{
    public static class UgfRectTransformExtensions
    {
        public static void CheckBoundary(this RectTransform transform)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform);

            Vector2 position = transform.anchoredPosition;

            if (position.x + transform.rect.width > Screen.currentResolution.width)
                position.x = Screen.currentResolution.width - transform.rect.width;

            if (position.y - transform.rect.height < -Screen.currentResolution.height)
                position.y = transform.rect.height - Screen.currentResolution.height;

            transform.anchoredPosition = position;
        }
    }
}