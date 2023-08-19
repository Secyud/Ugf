using Secyud.Ugf.BasicComponents;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Secyud.Ugf.FunctionalComponents
{
    public class SHoverable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private BoolEvent Event = new();

        public BoolEvent OnHoverChanged
        {
            get => Event;
            set => Event = value;
        }

        private bool _isHovered;

        public bool IsHovered
        {
            get => _isHovered;
            set
            {
                _isHovered = value;
                OnHoverChanged.Invoke(value);
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            IsHovered = true;
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            IsHovered = false;
        }
        
        
        public void Bind(UnityAction<bool> action)
        {
            Event.RemoveAllListeners();
            Event.AddListener(action);
        }
    }
}