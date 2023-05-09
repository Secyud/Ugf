using Secyud.Ugf.BasicComponents;
using System;
using UnityEngine;

namespace Secyud.Ugf.Layout
{
	[RequireComponent(typeof(SText))]
	public class TextLayoutTrigger:LayoutTrigger<SText>
	{
		protected override void OnDisable()
		{
			ContentSizeFitter.enabled = false;
		}

		protected override void OnEnable()
		{
			LayoutElement.enabled = true;
			if (ContentSizeFitter)
				ContentSizeFitter.enabled = true;
			Record = Math.Max(1, Record);
		}
	}
}