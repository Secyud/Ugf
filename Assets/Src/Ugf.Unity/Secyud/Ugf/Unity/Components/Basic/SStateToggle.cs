#region

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.Unity.Components
{
    public class SStateToggle : Selectable, IPointerClickHandler
    {
        [Serializable]
        public class StateToggleEvent : UnityEvent<int>
        {
        }

        [SerializeField] private StateToggleEvent ClickEvent = new();

        public StateToggleEvent OnClick
        {
            get => ClickEvent;
            set => ClickEvent = value;
        }

        public int StateValue { get; private set; }
        protected virtual int MaxValue => 3;

        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            StateValue = (StateValue + 1) % MaxValue;
            ClickEvent.Invoke(StateValue);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            Press();
        }

        public void Bind(UnityAction<int> action)
        {
            ClickEvent.AddListener(action);
        }
    }
}