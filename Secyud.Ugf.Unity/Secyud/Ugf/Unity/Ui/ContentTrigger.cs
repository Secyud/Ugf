﻿#region

using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.Unity.Ui
{
    public class ContentTrigger : MonoBehaviour
    {
        private ContentSizeFitter _contentSizeFitter;
        private RectTransform _rectTransform;
        private int _record;
        private bool _checkBoundary;
        public RectTransform RectTransform { get; private set; }


        protected virtual void Awake()
        {
            TryGetComponent(out _contentSizeFitter);
            RectTransform = GetComponent<RectTransform>();
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
                RectTransform.CheckBoundary();
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