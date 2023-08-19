#region

using System;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.LayoutComponents
{
	[RequireComponent(typeof(LayoutGroup))]
	public class LayoutGroupTrigger : LayoutTrigger
	{
		[SerializeField] protected bool Float;
		protected LayoutGroup LayoutElement;
		private const int RecordMax = 1;

		public LayoutGroup Element => LayoutElement;

		public bool Disabled
		{
			get => !enabled;
			set => enabled = !value;
		}
		
		protected override void Awake()
		{
			base.Awake();
			LayoutElement = GetComponent<LayoutGroup>();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			LayoutElement.enabled = true;
			if (Float) RectTransform.CheckBoundary();
			Record = Math.Max(1, Record);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			LayoutElement.enabled = false;
			if (Float) RectTransform.CheckBoundary();
		}

		public virtual void RefreshContent(IHasContent content)
		{
			for (int i = 0; i < transform.childCount; i++)
				Destroy(transform.GetChild(i).gameObject);
			content?.SetContent(PrepareLayout());
		}

		public virtual RectTransform PrepareLayout()
		{
			for (int i = 0; i < RectTransform.childCount; i++)
				Destroy(RectTransform.GetChild(i).gameObject);
			enabled = true;
			return RectTransform;
		}
	}
}