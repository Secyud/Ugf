using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.Unity.Ui
{
    [RequireComponent(typeof(RectTransform))]
    public class ContentTrigger : MonoBehaviour
    {
        private ContentSizeFitter _contentSizeFitter;
        private int _record;
        private bool _checkBoundary;
        private RectTransform _rectTransform;
        public RectTransform RectTransform => _rectTransform ? _rectTransform : GetComponent<RectTransform>();


        protected virtual void Awake()
        {
            TryGetComponent(out _contentSizeFitter);
        }

        public virtual void LateUpdate()
        {
            if (_record < 0)
            {
                enabled = false;
            }
            else
            {
                _record--;
            }
        }

        public void Refresh(int last = 3, bool checkBoundary = false)
        {
            _record = last;
            _checkBoundary = checkBoundary;
            enabled = true;
        }

        protected virtual void OnDisable()
        {
            if (_contentSizeFitter)
            {
                _contentSizeFitter.enabled = false;
            }

            if (_checkBoundary)
            {
                // TODO fix position change
                //RectTransform.CheckBoundary();
            }
        }

        protected virtual void OnEnable()
        {
            if (_contentSizeFitter)
            {
                _contentSizeFitter.enabled = true;
            }
        }
    }
}