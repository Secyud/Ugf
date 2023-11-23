#region

using System.Collections.Generic;
using System.Ugf;
using UnityEngine;

#endregion

namespace Secyud.Ugf.InputManager
{
	public class InputComponent : MonoBehaviour
	{
		[SerializeField] private string Name;
		[SerializeField] private InputEvent[] Events;
		public IEnumerable<InputEvent> Actions => Events;
		private InputService _service;
		private void Awake()
		{
			if (Name.IsNullOrEmpty())
			{
				Name = name;
			}
			_service = U.Get<InputService>();
			_service.AddInput(this);
		}

		private void OnDestroy()
		{
			_service.RemoveInput(this);
		}
	}
}