using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Secyud.Ugf.Unity.Ui
{
    public class PointerExit : MonoBehaviour, IPointerExitHandler
    {
        [field:SerializeField]public UnityEvent OnPointExit { get; private set; }= new();

        public void OnPointerExit(PointerEventData eventData)
        {
            OnPointExit.Invoke();
        }
    }
}