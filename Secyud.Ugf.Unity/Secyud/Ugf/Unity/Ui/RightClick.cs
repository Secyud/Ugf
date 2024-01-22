using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Secyud.Ugf.Unity.Ui
{
    public class RightClick : MonoBehaviour, IPointerClickHandler
    {
         [SerializeField] private UnityEvent _clickEvent;

        public UnityEvent OnClick => _clickEvent;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                _clickEvent.Invoke();
            }
        }
    }
}