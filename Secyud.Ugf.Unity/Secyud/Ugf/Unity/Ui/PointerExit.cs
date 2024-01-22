using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Secyud.Ugf.Unity.Ui
{
    public class PointerExit : MonoBehaviour, IPointerExitHandler
    {
        [SerializeField] private UnityEvent _pointExitEvent;
        public UnityEvent OnPointExit => _pointExitEvent;

        public void OnPointerExit(PointerEventData eventData)
        {
            OnPointExit.Invoke();
        }
    }
}