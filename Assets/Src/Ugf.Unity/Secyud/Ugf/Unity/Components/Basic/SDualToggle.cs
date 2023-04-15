#region

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

#endregion

namespace Secyud.Ugf.Unity.Components
{
    public class SDualToggle : SToggle
    {
        [Serializable]
        public class DualToggleEvent : UnityEvent
        {
        }

        [SerializeField] private DualToggleEvent LeftClickEvent = new();
        [SerializeField] private DualToggleEvent RightClickEvent = new();

        public DualToggleEvent OnLeftClick
        {
            get => LeftClickEvent;
            set => LeftClickEvent = value;
        }

        public DualToggleEvent OnRightClick
        {
            get => RightClickEvent;
            set => RightClickEvent = value;
        }

        private void PressLeft()
        {
            if (!IsActive() || !IsInteractable())
                return;

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            LeftClickEvent.Invoke();
        }

        private void PressRight()
        {
            if (!IsActive() || !IsInteractable())
                return;

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            RightClickEvent.Invoke();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    PressLeft();
                    break;
                case PointerEventData.InputButton.Right:
                    PressRight();
                    break;
                case PointerEventData.InputButton.Middle:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void BindLeft(UnityAction action)
        {
            LeftClickEvent.AddListener(action);
        }

        public void BindRight(UnityAction action)
        {
            RightClickEvent.AddListener(action);
        }
    }
}