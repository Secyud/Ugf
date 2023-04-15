#region

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

#endregion

namespace Secyud.Ugf.Unity.Components
{
    public sealed class Hoverable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float Delay = 0.5f;
        [SerializeField] private UnityEvent OnHoverEvent = new();

        private bool _isHovering;
        private float _hoverTime;

        public UnityEvent OnHover
        {
            get => OnHoverEvent;
            set => OnHoverEvent = value;
        }

        private void Update()
        {
            if (!_isHovering) return;

            if (Input.GetMouseButtonDown(0))
            {
                ResetHover();
                return;
            }

            _hoverTime += Time.deltaTime;

            if (!(_hoverTime > Delay)) return;

            OnHoverEvent.Invoke();

            ResetHover();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovering = true;
            _hoverTime = 0;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ResetHover();
        }

        private void ResetHover()
        {
            _isHovering = false;
            _hoverTime = 0;
        }

        public void Bind(UnityAction action)
        {
            OnHoverEvent.AddListener(action);
        }
    }
}