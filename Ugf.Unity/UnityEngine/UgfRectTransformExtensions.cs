using System;
using UnityEngine.UI;

namespace UnityEngine
{
	public static class UgfRectTransformExtensions
	{
		public static void SetRectPosition(this RectTransform transform, Vector2 position, Vector2 bias)
		{
			Vector2 lb = transform.GetLeftBottomBias();
			Rect rect = transform.rect;
			transform.anchoredPosition = position - lb +
				new Vector2(bias.x * rect.width, bias.y * rect.height);
		}


		public static void CheckBoundary(this RectTransform transform)
		{
			Vector2 lb = transform.GetLeftBottomBias();
			Vector2 position = transform.anchoredPosition;
			Rect rect = transform.rect;
			float x = Math.Min(position.x, Screen.currentResolution.width - lb.x - rect.width);
			float y = Math.Max(position.y, -lb.y - Screen.currentResolution.height);

			transform.anchoredPosition = new Vector2(x, y);
		}

		public static bool MouseIn(this RectTransform transform)
		{
			Vector2 lb = transform.GetLeftBottomBias() + transform.anchoredPosition;
			Rect rect = transform.rect;
			Vector3 mouse = UgfUnityExtensions.GetMousePosition();
			return mouse.x < lb.x + rect.width &&
				mouse.x > lb.x &&
				mouse.y > lb.y &&
				mouse.y < lb.y + rect.height;
		}

		public static Vector2 GetLeftBottomBias(this RectTransform transform)
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