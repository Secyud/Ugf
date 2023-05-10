#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Secyud.Ugf.InputManaging
{
	public class InputLayer
	{
		internal InputLayer(RectTransform layer)
		{
			Layer = layer;
		}

		public RectTransform Layer { get; }

		public Dictionary<KeyCode, InputUnit> Inputs { get; } = new();
	}
}