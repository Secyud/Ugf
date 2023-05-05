using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Secyud.Ugf.Layout
{
	public abstract class LayoutTrigger<TLayoutElement> : MonoBehaviour
		where TLayoutElement : UIBehaviour, ILayoutElement
	{
		protected ContentSizeFitter ContentSizeFitter;
		protected TLayoutElement LayoutElement;
		protected RectTransform RectTransform;
		private const int RecordMax = 1;
		private int _record;

		public TLayoutElement Element => LayoutElement;
		
		protected virtual void Awake()
		{
			TryGetComponent(out ContentSizeFitter);
			LayoutElement = GetComponent<TLayoutElement>();
			RectTransform = GetComponent<RectTransform>();
		}

		private void OnEnable()
		{
			EnableOperation();
			_record = RecordMax;
		}

		private void LateUpdate()
		{
			if (_record < 0)
			{
				UnEnableOperation();
				enabled = false;
			}
			else
			{
				_record--;
			}
		}

		protected virtual void UnEnableOperation()
		{
			if (ContentSizeFitter)
				ContentSizeFitter.enabled = false;
			LayoutElement.enabled = false;
			RectTransform.CheckBoundary();
		}
		protected virtual void EnableOperation()
		{
			LayoutElement.enabled = true;
			if (ContentSizeFitter)
				ContentSizeFitter.enabled = true;
			RectTransform.CheckBoundary();
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