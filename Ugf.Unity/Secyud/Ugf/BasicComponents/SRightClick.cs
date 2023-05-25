using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Secyud.Ugf.BasicComponents
{
	public class SRightClick:MonoBehaviour,IPointerClickHandler
	{
		public UnityEvent RightClick = new();

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Right)
				RightClick.Invoke();
		}

		public void Bind(UnityAction action)
		{
			Clear();
			RightClick.AddListener(action);
		}

		public void Clear()
		{
			RightClick.RemoveAllListeners();
		}
	}
}