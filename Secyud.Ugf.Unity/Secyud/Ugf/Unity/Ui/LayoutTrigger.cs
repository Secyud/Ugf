﻿using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.Unity.Ui
{
    [RequireComponent(typeof(LayoutGroup))]
    public class LayoutTrigger : ContentTrigger
    {
        public LayoutGroup LayoutElement { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            LayoutElement = GetComponent<LayoutGroup>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            LayoutElement.enabled = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            LayoutElement.enabled = false;
        }

        public void ClearContent()
        {
            gameObject.SetActive(true);

            for (int i = 0; i < RectTransform.childCount; i++)
            {
                Transform child = RectTransform.GetChild(i);
                if (child.TryGetComponent(out LayoutElement element) &&
                    element.ignoreLayout) continue;
                Destroy(child.gameObject);
            }
        }

        public void ActivateFloating(RectTransform target)
        {
            gameObject.SetActive(true);
            RectTransform.position = target.position;

            Refresh(checkBoundary: true);
        }
    }
}