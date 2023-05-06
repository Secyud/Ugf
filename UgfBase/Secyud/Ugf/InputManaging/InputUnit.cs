using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Secyud.Ugf.InputManaging
{
	public class InputUnit
	{
		internal InputUnit(string name,KeyCode code)
		{
			Name = name;
			KeyCode = code;
		}

		public string Name { get; }
		
		public KeyCode KeyCode { get; set; }

		public List<Pair<int,UnityEvent>> Events { get; } = new();
	}
}