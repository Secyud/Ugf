using System;
using UnityEngine.UI;

namespace UnityEngine
{
	public static class UgfRectTransformExtensions
	{
		public static void CheckBoundary(this RectTransform transform)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(transform);

			Rect rect = transform.rect;
			Vector2 lb = GetLeftBottom(transform);
			float dx = Math.Min(0, Screen.currentResolution.width - lb.x - rect.width);
			float dy = Math.Max(0, -lb.y);

			transform.anchoredPosition += new Vector2(dx, dy);
		}


		public static bool MouseIn(this RectTransform transform)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(transform);
			RectTransform pTransform = transform.parent.GetComponent<RectTransform>();
			Rect rect = transform.rect;
			Vector3 mouse = Input.mousePosition;
			Vector2 lb = GetLeftBottom(transform);
			return mouse.x < lb.x + rect.width &&
				mouse.x > lb.x &&
				mouse.y < lb.y + rect.height &&
				mouse.y > lb.y;
		}

		private static Vector2 GetLeftBottom(RectTransform transform)
		{
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
			return new Vector2(x, y);
		}
	}
}