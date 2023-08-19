#region

using Secyud.Ugf.LayoutComponents;
using UnityEngine;

#endregion

namespace Secyud.Ugf.BasicComponents
{
	[RequireComponent(typeof(RectTransform))]
	public class SPopup : LayoutGroupTrigger
	{
		[SerializeField] public LayoutGroupTrigger SubLayoutGroupTrigger;

		public void Initialize(Vector2 position, Vector2 bias)
		{
			RectTransform.SetRectPosition(position, bias);
		}
		public void InitializeOnCenter()
		{
			Vector2 position = new(
				Screen.currentResolution.width / 2f,
				-Screen.currentResolution.height / 2f
			);
			Initialize(position, new Vector2(-0.5f, -0.5f));
		}

		public void InitializeOnMouse()
		{
			Initialize(UgfUnityExtensions.GetMousePosition() - new Vector2(8,8), 
				Vector2.zero);
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

		protected override void OnEnable()
		{
			base.OnEnable();
			Record = 2;
		}

		public override RectTransform PrepareLayout()
		{
			enabled = true;
			RectTransform trans = SubLayoutGroupTrigger == this 
				? base.PrepareLayout() 
				: SubLayoutGroupTrigger.PrepareLayout();
			return trans;
		}
	}
}