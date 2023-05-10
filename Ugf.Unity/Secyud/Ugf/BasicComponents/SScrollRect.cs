﻿#region

using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.BasicComponents
{
	public class SScrollRect : ScrollRect
	{
		public void SetPosition(RectTransform position)
		{
			var rect = content.rect;
			var anchoredPosition = position.anchoredPosition;
			var viewPortRect = viewRect.rect;
			var positionRect = position.rect;
			float x = Mathf.Abs(rect.width - viewPortRect.width) < 48 ?
				0.5f : (anchoredPosition.x - 48) / (rect.width - viewPortRect.width);
			float y = Mathf.Abs(rect.height - viewPortRect.height) < 48 ?
				0.5f : 1 + (anchoredPosition.y + positionRect.height / 2 + 48) /
				(rect.height - viewPortRect.height);

			normalizedPosition = new Vector2(x, y);
		}
	}
}