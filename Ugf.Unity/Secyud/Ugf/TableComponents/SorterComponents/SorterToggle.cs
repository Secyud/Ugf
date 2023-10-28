#region

using System;
using Secyud.Ugf.BasicComponents;
using UnityEngine;
using UnityEngine.EventSystems;

#endregion

namespace Secyud.Ugf.TableComponents.SorterComponents
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SorterToggle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private SImage Check;
        [SerializeField] private SText Name;
        private CanvasGroup _canvasGroup;
        private Vector2 _deltaRecord;
        private RectTransform _rectTransform;

        public Sorter Sorter { get; private set; }

        public ICanBeStated Triggerable { get; private set; }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            _canvasGroup.blocksRaycasts = false;
            _rectTransform.SetParent(U.Canvas.gameObject.transform);
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
            int result = 0;
            RectTransform layout = Sorter.Layout.RectTransform;

            for (int i = 0; i < layout.childCount; i++)
            {
                Transform trans = layout.GetChild(i);

                if ((trans.position - _rectTransform.position).x > 0)
                    result = i;
                else
                    break;
            }

            _rectTransform.SetParent(layout);
            transform.SetSiblingIndex(result);
            _canvasGroup.blocksRaycasts = true;
            Sorter.Layout.enabled = true;
            Sorter.RefreshTable();
        }

        private int _record = 0;

        public void OnClick()
        {
            OnClick(_record++ % 3);
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

            Sorter.RefreshTable();
        }

        private void SetSorter(Sorter sorter, ICanBeStated triggerable)
        {
            Sorter = sorter;
            Triggerable = triggerable;
            Name.text = U.T[triggerable.Name];
        }

        public SorterToggle Create(Transform parent, Sorter sorter, ICanBeStated triggerable)
        {
            SorterToggle ret = Instantiate(this, parent);

            ret.SetSorter(sorter, triggerable);

            return ret;
        }
    }
}