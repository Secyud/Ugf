#region

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.BasicComponents
{
    public class SDualToggle : Selectable,IPointerClickHandler
    {
        [SerializeField] private DualToggleEvent LeftClickEvent = new();
        [SerializeField] private DualToggleEvent RightClickEvent = new();
        [SerializeField] private Graphic Graphic;

        private UnityEvent<bool> _onValueChanged;

        private bool _isOn;


        public bool IsOn
        {
            get => _isOn;
            set => Set(value);
        }
        

        public void SetIsOnWithoutNotify(bool getEnabled)
        {
            Set(getEnabled, false);
        }

        private void Set(bool isOn, bool callBack = true)
        {
            _isOn = isOn;
            Graphic.enabled = _isOn;
            if (callBack)
                _onValueChanged.Invoke(_isOn);
        }

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

            LeftClickEvent.Invoke();
        }

        private void PressRight()
        {
            if (!IsActive() || !IsInteractable())
                return;

            RightClickEvent.Invoke();
        }


        public  void OnPointerClick(PointerEventData eventData)
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
        public void Bind(UnityAction<bool> action)
        {
            Clear();
            _onValueChanged.AddListener(action);
        }

        
        private void Clear()
        {
            _onValueChanged.RemoveAllListeners();
        }

        [Serializable]
        public class DualToggleEvent : UnityEvent
        {
        }
    }
}