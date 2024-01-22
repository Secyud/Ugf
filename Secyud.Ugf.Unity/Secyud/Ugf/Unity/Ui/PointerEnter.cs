using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Secyud.Ugf.Unity.Ui
{
    public class PointerEnter: MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] private UnityEvent _pointEnterEvent;
        public UnityEvent OnPointEnter => _pointEnterEvent;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            OnPointEnter.Invoke();
        }
    }
}