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


		public static bool MouseIn(this RectTransform transform)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(transform);
			RectTransform pTransform = transform.parent.GetComponent<RectTransform>();
			Rect rect = transform.rect;
			Rect pRect = pTransform.rect;
			Vector2 pivot = transform.pivot;
			Vector2 anchorMin = transform.anchorMin;
			Vector2 anchorMax = transform.anchorMax;
			Vector2 anchoredPosition = transform.anchoredPosition;
			Vector3 mouse = Input.mousePosition;
			float x;
			float y;
			if (Mathf.Approximately(anchorMax.x, anchorMin.x))
				x = anchorMin.x * pRect.width - pivot.x * rect.width + anchoredPosition.x;
			else
				x = anchorMin.x * pRect.width + (anchorMax.x - anchorMin.x) * pivot.x * pRect.width +
					anchoredPosition.x - rect.width / 2;
			if (Mathf.Approximately(anchorMax.y, anchorMin.y))
				y = anchorMin.y * pRect.height - pivot.y * rect.height + anchoredPosition.y;
			else
				y = anchorMin.y * pRect.height + (anchorMax.y - anchorMin.y) * pivot.y * pRect.height +
					anchoredPosition.y - rect.height / 2;
			return mouse.x < x + rect.width &&
				mouse.x > x &&
				mouse.y < y + rect.height &&
				mouse.y > y;
		}
	}
}