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