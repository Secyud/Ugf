#region

using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.Unity.Ui
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollRectEx : MonoBehaviour
    {
        private ScrollRect _scrollRect;

        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();
        }

        public void SetPosition(RectTransform position)
        {
            Rect rect = _scrollRect.content.rect;
            Vector2 anchoredPosition = position.anchoredPosition;
            Rect viewPortRect = _scrollRect.viewport 
                ? _scrollRect.viewport.rect 
                : ((RectTransform)_scrollRect.transform).rect;
            Rect positionRect = position.rect;
            float x = Mathf.Abs(rect.width - viewPortRect.width) < 48
                ? 0.5f
                : (anchoredPosition.x - 48) / (rect.width - viewPortRect.width);
            float y = Mathf.Abs(rect.height - viewPortRect.height) < 48
                ? 0.5f
                : 1 + (anchoredPosition.y + positionRect.height / 2 + 48) /
                (rect.height - viewPortRect.height);

            _scrollRect.normalizedPosition = new Vector2(x, y);
        }
    }
}