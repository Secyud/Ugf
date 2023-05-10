#region

using UnityEngine;
using UnityEngine.EventSystems;

#endregion

namespace Secyud.Ugf.FunctionalComponents
{
	public class DestroyOnPointerExit : MonoBehaviour, IPointerExitHandler
	{
		public void OnPointerExit(PointerEventData eventData)
		{
			Destroy(gameObject);
		}
	}
}