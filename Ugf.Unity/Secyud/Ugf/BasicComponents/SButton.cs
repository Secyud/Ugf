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
		
		public void Clear()
		{
			onClick.RemoveAllListeners();
		}

		// ReSharper disable once InconsistentNaming
		public bool disabled
		{
			get => !enabled;
			set => enabled = !value;
		}
	}
}