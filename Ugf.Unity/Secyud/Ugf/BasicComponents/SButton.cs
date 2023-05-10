#region

using UnityEngine.Events;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.BasicComponents
{
	public class SButton : Button
	{
		public void Bind(UnityAction action)
		{
			Clear();
			onClick.AddListener(action);
		}

		public void SetSelect(bool isSelect)
		{
			if (isSelect)
				OnSelect(null);
			else
				OnDeselect(null);
		}

		public void Clear()
		{
			onClick.RemoveAllListeners();
		}
	}
}