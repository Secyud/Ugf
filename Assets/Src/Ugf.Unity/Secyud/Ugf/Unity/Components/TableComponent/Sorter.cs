#region

using System;
using UnityEngine;
using UnityEngine.EventSystems;

#endregion

namespace Secyud.Ugf.Unity.Components
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Sorter : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private SImage Check;
        [SerializeField] private SText Name;

        public FunctionalTable FunctionalTable { get; private set; }
        public ICanBeStated Triggerable { get; private set; }

        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private Vector2 _deltaRecord;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
        }

        public void OnClick(int state)
        {
            Check.transform.rotation = state switch
            {
                0 => Quaternion.Euler(0, 0, 0),
                1 => Quaternion.Euler(0, 0, 90),
                2 => Quaternion.Euler(0, 0, -90),
                _ => throw new ArgumentOutOfRangeException()
            };

            Triggerable.Enabled = state switch
            {
                0 => null,
                1 => false,
                2 => true,
                _ => throw new ArgumentOutOfRangeException()
            };

            FunctionalTable.RefreshSorter();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            _canvasGroup.blocksRaycasts = true;
            _rectTransform.SetParent(Og.Canvas.gameObject.transform);
            _deltaRecord = _rectTransform.anchoredPosition - eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            _rectTransform.anchoredPosition = eventData.position + _deltaRecord;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left ||
                eventData.pointerCurrentRaycast.gameObject.transform.parent != transform.parent)
                return;

            int result = 0;

            for (int i = 0; i < FunctionalTable.SortableContent.childCount; i++)
            {
                var trans = FunctionalTable.SortableContent.GetChild(i);

                if ((trans.position - _rectTransform.position).x > 0)
                    result = i;
                else
                    break;
            }

            _rectTransform.SetParent(FunctionalTable.SortableContent);

            transform.SetSiblingIndex(result);

            _canvasGroup.blocksRaycasts = false;

            FunctionalTable.RefreshSorter();
        }

        private void OnInitialize(FunctionalTable functionalTable, ICanBeStated triggerable)
        {
            FunctionalTable = functionalTable;
            Triggerable = triggerable;
            Name.text = Og.L[triggerable.Name];
        }

        public Sorter Create(Transform parent, FunctionalTable functionalTable, ICanBeStated triggerable)
        {
            var ret = Instantiate(this, parent);

            ret.OnInitialize(functionalTable, triggerable);

            return ret;
        }
    }
}