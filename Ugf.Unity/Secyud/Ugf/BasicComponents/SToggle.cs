#region

using UnityEngine.Events;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.BasicComponents
{
	public class SToggle : Toggle
	{
		public void Bind(UnityAction<bool> action)
		{
			Clear();
			onValueChanged.AddListener(action);
		}

		private void Clear()
		{
			onValueChanged.RemoveAllListeners();
		}
	}
}