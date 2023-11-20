#region

using System;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.LayoutComponents
{
    public class LayoutTrigger : MonoBehaviour
    {
        [SerializeField] private int RecordMax = 1;
        protected ContentSizeFitter ContentSizeFitter;

        private RectTransform _rectTransform;
        public RectTransform RectTransform => _rectTransform ??= GetComponent<RectTransform>();

        public int Record { get; set; } = 1;

        protected virtual void Awake()
        {
            TryGetComponent(out ContentSizeFitter);
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
            Record = Math.Max(1, RecordMax);
        }
    }
}