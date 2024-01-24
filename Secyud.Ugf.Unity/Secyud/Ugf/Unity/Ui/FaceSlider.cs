using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Secyud.Ugf.Unity.Ui
{
    public class FaceSlider : Selectable, IDragHandler
    {
        [Serializable]
        public class FaceSliderEvent : UnityEvent<Vector2>
        {
        }

        [SerializeField] private RectTransform _handleRect;
        [SerializeField] private RectTransform _handleContainerRect;
        [SerializeField] private Vector2 _minValue;
        [SerializeField] private Vector2 _maxValue = Vector2.one;
        [SerializeField] private Vector2 _value;
        [SerializeField] private FaceSliderEvent _onValueChanged = new();

        public Vector2 Value
        {
            get => _value;
            set => Set(value);
        }

        public Vector2 MaxValue
        {
            get => _maxValue;
            set => _maxValue = value;
        }

        public Vector2 MinValue
        {
            get => _minValue;
            set => _minValue = value;
        }

        public Vector2 NormalizedValue
        {
            get => new(
                Mathf.Approximately(_minValue.x, _maxValue.x)
                    ? 0
                    : Mathf.InverseLerp(_minValue.x, _maxValue.x, _value.x),
                Mathf.Approximately(_minValue.y, _maxValue.y)
                    ? 0
                    : Mathf.InverseLerp(_minValue.y, _maxValue.y, _value.y)
            );
            set => Value = new Vector2(
                Mathf.Lerp(_minValue.x, _maxValue.x, value.x),
                Mathf.Lerp(_minValue.y, _maxValue.y, value.y)
            );
        }

        public FaceSliderEvent OnValueChanged
        {
            get => _onValueChanged;
            set => _onValueChanged = value;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Set(Value, false);
            UpdateVisuals();
        }

        private Vector2 ClampValue(Vector2 input)
        {
            return new Vector2(
                Mathf.Clamp(input.x, _minValue.x, _maxValue.x),
                Mathf.Clamp(input.y, _minValue.y, _maxValue.y)
            );
        }

        public virtual void SetValueWithoutNotification(Vector2 input)
        {
            Set(input, false);
        }

        protected virtual void Set(Vector2 input, bool sendCallback = true)
        {
            Vector2 newValue = ClampValue(input);

            if (_value == newValue)
                return;

            _value = newValue;
            UpdateVisuals();
            if (sendCallback)
                _onValueChanged.Invoke(newValue);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (!IsActive())
                return;

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            _handleRect.anchorMin = _handleRect.anchorMax = NormalizedValue;
            _handleRect.anchoredPosition = Vector2.zero;
        }

        // Update the slider's position based on the mouse.
        private void UpdateDrag(PointerEventData eventData, Camera cam)
        {
            if (_handleContainerRect)
            {
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        _handleContainerRect, Input.mousePosition, cam,
                        out Vector2 localCursor
                    ))
                    return;

                Rect rect = _handleContainerRect.rect;

                localCursor += rect.size / 2;
                Vector2 size = rect.size;

                NormalizedValue = new Vector2(
                    Mathf.Clamp01(localCursor.x / size.x),
                    Mathf.Clamp01(localCursor.y / size.y)
                );
            }
        }

        private bool MayDrag(PointerEventData eventData)
        {
            return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
        }


        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            base.OnPointerDown(eventData);


            UpdateDrag(eventData, eventData.pressEventCamera);
        }


        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            UpdateDrag(eventData, eventData.pressEventCamera);
        }
    }
}