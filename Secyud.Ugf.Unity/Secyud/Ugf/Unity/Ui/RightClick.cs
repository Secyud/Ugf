using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Secyud.Ugf.Unity.Ui
{
    public class RightClick : MonoBehaviour, IPointerClickHandler
    {
        [field: SerializeField]public UnityEvent OnClick  { get; private set; }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                OnClick.Invoke();
            }
        }
    }
}