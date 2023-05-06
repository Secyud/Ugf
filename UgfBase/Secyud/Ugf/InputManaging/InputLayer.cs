using System.Collections.Generic;
using UnityEngine;

namespace Secyud.Ugf.InputManaging
{
	public class InputLayer
	{
		internal InputLayer(int index)
		{
			Index = index;
		}
		
		public int Index { get; }

		public Dictionary<KeyCode, InputUnit> Inputs { get; } = new();
	}
}