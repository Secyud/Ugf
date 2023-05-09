using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Secyud.Ugf.Layout
{
	public abstract class LayoutTrigger<TLayoutElement> : MonoBehaviour
		where TLayoutElement : UIBehaviour, ILayoutElement
	{
		[SerializeField] protected bool Float;
		protected ContentSizeFitter ContentSizeFitter;
		protected TLayoutElement LayoutElement;
		private const int RecordMax = 1;

		public RectTransform RectTransform { get; private set; }
		public int Record { get; set; }

		public TLayoutElement Element => LayoutElement;

		protected virtual void Awake()
		{
			TryGetComponent(out ContentSizeFitter);
			LayoutElement = GetComponent<TLayoutElement>();
			RectTransform = GetComponent<RectTransform>();
		}

		protected virtual void OnEnable()
		{
			LayoutElement.enabled = true;
			if (ContentSizeFitter)
				ContentSizeFitter.enabled = true;
			if (Float) RectTransform.CheckBoundary();
			Record = Math.Max(1, Record);
		}

		protected virtual void LateUpdate()
		{
			if (Record < 0)
			{
				enabled = false;
			}
			else
			{
				Record--;
			}
		}

		protected virtual void OnDisable()
		{
			if (ContentSizeFitter)
				ContentSizeFitter.enabled = false;
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