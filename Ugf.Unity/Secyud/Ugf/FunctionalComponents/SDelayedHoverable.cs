#region

using Secyud.Ugf.BasicComponents;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

#endregion

namespace Secyud.Ugf.FunctionalComponents
{
    public class SDelayedHoverable : SHoverable
    {
        [SerializeField] private float Delay = 0.8f;
        [SerializeField] private VoidEvent Trigger = new();
        private float _hoverTime;
        private Vector2 _mouseRecord;

        public VoidEvent OnHoverTrig
        {
            get => Trigger;
            set => Trigger = value;
        }
        
        protected virtual void Update()
        {
            if (!IsHovered)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                OnPointerExit(null);
                return;
            }

            Vector2 mousePosition = Input.mousePosition;

            if (Mathf.Approximately(mousePosition.x, _mouseRecord.x) &&
                Mathf.Approximately(mousePosition.y, _mouseRecord.y))
            {
                _hoverTime += Time.deltaTime;

                if (_hoverTime > Delay)
                {
                    Trigger.Invoke();
                    OnPointerExit(null);
                }
            }
            else
            {
                _hoverTime = 0;
            }

            _mouseRecord = mousePosition;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            _hoverTime = 0;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            _hoverTime = 0;
        }

        public void Bind(UnityAction action)
        {
            Trigger.RemoveAllListeners();
            Trigger.AddListener(action);
        }
    }
}