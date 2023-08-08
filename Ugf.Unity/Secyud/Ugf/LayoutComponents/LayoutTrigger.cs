#region

using System;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.LayoutComponents
{
	public class LayoutTrigger : MonoBehaviour
	{

		private const int RecordMax = 1;
		protected ContentSizeFitter ContentSizeFitter;

		public RectTransform RectTransform { get; private set; }

		public int Record { get; set; } = 1;

		protected virtual void Awake()
		{
			TryGetComponent(out ContentSizeFitter);
			RectTransform = GetComponent<RectTransform>();
		}

		public virtual void LateUpdate()
		{
			if (Record < 0)
				enabled = false;
			else
				Record--;
		}

		protected virtual void OnDisable()
		{
			if (ContentSizeFitter)
				ContentSizeFitter.enabled = false;
		}

		protected virtual void OnEnable()
		{
			if (ContentSizeFitter)
				ContentSizeFitter.enabled = true;
			Record = Math.Max(1, Record);
		}
	}
}