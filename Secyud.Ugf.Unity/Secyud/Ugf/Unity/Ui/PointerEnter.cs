using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Secyud.Ugf.Unity.Ui
{
    public class PointerEnter: MonoBehaviour, IPointerEnterHandler
    {
        [field:SerializeField] public UnityEvent OnPointEnter { get; private set; }= new();
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            OnPointEnter.Invoke();
        }
    }
}