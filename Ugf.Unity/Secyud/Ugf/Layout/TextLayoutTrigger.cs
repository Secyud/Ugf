using Secyud.Ugf.BasicComponents;
using UnityEngine;

namespace Secyud.Ugf.Layout
{
	[RequireComponent(typeof(SText))]
	public class TextLayoutTrigger:LayoutTrigger<SText>
	{
		protected override void UnEnableOperation()
		{
			ContentSizeFitter.enabled = false;
		}

		protected override void EnableOperation()
		{
			LayoutElement.enabled = true;
			if (ContentSizeFitter)
				ContentSizeFitter.enabled = true;
		}
	}
}