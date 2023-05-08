using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Secyud.Ugf.BasicComponents
{
	public class SFaceSlider : Selectable, IDragHandler
	{

		[Serializable] public class FaceSliderEvent : UnityEvent<Vector2>
		{
		}

		[SerializeField] private RectTransform HandleRect;
		[SerializeField] private RectTransform HandleContainerRect;
		[SerializeField] private Vector2 SMinValue;
		[SerializeField] private Vector2 SMaxValue = Vector2.one;
		[SerializeField] private Vector2 SValue;

		public Vector2 Value
		{
			get => SValue;
			set => Set(value);
		}

		public Vector2 NormalizedValue
		{
			get => new(
				Mathf.Approximately(SMinValue.x, SMaxValue.x) ? 0 :
					Mathf.InverseLerp(SMinValue.x, SMaxValue.x, SValue.x),
				Mathf.Approximately(SMinValue.y, SMaxValue.y) ? 0 :
					Mathf.InverseLerp(SMinValue.y, SMaxValue.y, SValue.y)
			);
			set => Value = new Vector2(
				Mathf.Lerp(SMinValue.x, SMaxValue.x, value.x),
				Mathf.Lerp(SMinValue.y, SMaxValue.y, value.y)
			);
		}

		[SerializeField] private FaceSliderEvent SOnValueChanged = new();


		public FaceSliderEvent OnValueChanged
		{
			get => SOnValueChanged;
			set => SOnValueChanged = value;
		}


		protected SFaceSlider()
		{
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
				Mathf.Clamp(input.x, SMinValue.x, SMaxValue.x),
				Mathf.Clamp(input.y, SMinValue.y, SMaxValue.y)
			);
		}

		protected virtual void Set(Vector2 input, bool sendCallback = true)
		{
			Vector2 newValue = ClampValue(input);

			if (SValue == newValue)
				return;

			SValue = newValue;
			UpdateVisuals();
			if (sendCallback)
				SOnValueChanged.Invoke(newValue);
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
			HandleRect.anchorMin = HandleRect.anchorMax = NormalizedValue;
			HandleRect.anchoredPosition = Vector2.zero;
		}

		// Update the slider's position based on the mouse.
		private void UpdateDrag(PointerEventData eventData, Camera cam)
		{
			if (HandleContainerRect)
			{
				if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
					HandleContainerRect, Input.mousePosition, cam,
					out Vector2 localCursor
				))
					return;

				Rect rect = HandleContainerRect.rect;

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