using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Secyud.Ugf.Unity.Ui
{
    public class Hoverable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float _delay = 0.8f;
        [field:SerializeField] public UnityEvent OnHover { get; private set; }
        private float _hoverTime;
        private Vector2 _mouseRecord;
        private bool _isHovered;


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
                    OnHover.Invoke();
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