#region

using Secyud.Ugf.Layout;
using UnityEngine;

#endregion

namespace Secyud.Ugf.BasicComponents
{
	[RequireComponent(typeof(RectTransform))]
	public class SFloating : LayoutGroupTrigger
	{
		[SerializeField] private LayoutGroupTrigger SubLayoutGroupTrigger;


		public void OnInitialize(Vector2 position, Vector2 bias)
		{
			RectTransform.SetRectPosition(position, bias);
		}

		public SFloating Create(Vector2 position, Vector2 bias)
		{
			SFloating floating = Instantiate(this, Og.Canvas.transform);
			floating.OnInitialize(position, bias);
			return floating;
		}

		public SFloating CreateOnCenter()
		{
			Vector2 position = new(
				Screen.currentResolution.width / 2f,
				-Screen.currentResolution.height / 2f
			);
			return Create(position, new Vector2(-0.5f, -0.5f));
		}

		public SFloating CreateOnMouse()
		{
			return Create(UgfUnityExtensions.GetMousePosition(), new Vector2(-8, -8));
		}

		public void Die()
		{
			Destroy(gameObject);
		}

		public override void RefreshContent(IHasContent hasContent)
		{
			if (SubLayoutGroupTrigger == this)
				base.RefreshContent(hasContent);
			else
				SubLayoutGroupTrigger.RefreshContent(hasContent);
			enabled = true;
		}

		public override RectTransform PrepareLayout()
		{
			enabled = true;
			return SubLayoutGroupTrigger == this ? base.PrepareLayout() : SubLayoutGroupTrigger.PrepareLayout();
		}
	}
}