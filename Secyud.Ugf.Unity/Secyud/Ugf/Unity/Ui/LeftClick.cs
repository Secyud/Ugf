using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Secyud.Ugf.Unity.Ui
{
    public class LeftClick : MonoBehaviour, IPointerClickHandler
    {
        [field:SerializeField]public UnityEvent OnClick { get; private set; }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                OnClick.Invoke();
            }
        }
    }
}