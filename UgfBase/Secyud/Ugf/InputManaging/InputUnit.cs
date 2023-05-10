#region

using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Secyud.Ugf.InputManaging
{
	public class InputUnit
	{
		internal InputUnit(KeyCode code, UnityEvent @event)
		{
			KeyCode = code;
			Event = @event;
		}

		public KeyCode KeyCode { get; }

		public UnityEvent Event { get; }
	}
}