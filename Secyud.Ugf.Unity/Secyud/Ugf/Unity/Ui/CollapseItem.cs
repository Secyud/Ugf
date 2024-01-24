using UnityEngine;

namespace Secyud.Ugf.Unity.Ui
{
	public class CollapseItem : MonoBehaviour
	{
		[SerializeField] private LayoutTrigger _trigger;
		[SerializeField] private GameObject _gameObject;

		public void Collapse(bool b)
		{
			_gameObject.SetActive(!b);
			_trigger.Refresh();
		}
	}
}