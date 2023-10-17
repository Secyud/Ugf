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
		[SerializeField] protected int ClearStart;
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
			if (Float)
			{
				RectTransform.CheckBoundary();
			}
		}

		public virtual void RefreshContent(IHasContent content)
		{
			ClearContent();
			content?.SetContent(PrepareLayout());
		}

		public virtual RectTransform PrepareLayout()
		{
			ClearContent();
			enabled = true;
			return RectTransform;
		}

		public virtual void ClearContent()
		{
			for (int i = ClearStart; i < RectTransform.childCount; i++)
				Destroy(RectTransform.GetChild(i).gameObject);
		}
		
		
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
			Initialize(UgfUnityExtensions.GetMousePosition() - new Vector2(1,1), 
				Vector2.zero);
		}
	}
}