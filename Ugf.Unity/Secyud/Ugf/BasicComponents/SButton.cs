#region

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.BasicComponents
{
	public class SButton : Button
	{
		public void Bind(UnityAction action)
		{
			onClick.AddListener(action);
		}

		// ReSharper disable once InconsistentNaming
		public bool disabled
		{
			get => !enabled;
			set => enabled = !value;
		}
	}
}