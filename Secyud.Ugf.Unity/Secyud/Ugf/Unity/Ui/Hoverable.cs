#region

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

#endregion

namespace Secyud.Ugf.Unity.Ui
{
    public class Hoverable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private UnityEvent _hoverEvent = new();
        [SerializeField] private float _delay = 0.8f;
        private float _hoverTime;
        private Vector2 _mouseRecord;
        private bool _isHovered;

        public UnityEvent OnHover => _hoverEvent;

        protected virtual void Update()
        {
            if (!_isHovered) return;

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

                if (_hoverTime > _delay)
                {
                    _hoverEvent.Invoke();
                    OnPointerExit(null);
                }
            }
            else
            {
                _hoverTime = 0;
            }

            _mouseRecord = mousePosition;
        }

        public  void OnPointerEnter(PointerEventData eventData)
        {
            _isHovered = true;
            _hoverTime = 0;
        }

        public  void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
            _hoverTime = 0;
        }
    }
}