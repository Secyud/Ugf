#region

using UnityEngine;
using UnityEngine.UI;

#endregion

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
            for (int i = 0; i < RectTransform.childCount; i++)
            {
                Destroy(RectTransform.GetChild(i).gameObject);
            }
        }
    }
}