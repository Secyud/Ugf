using Secyud.Ugf.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Secyud.Ugf.InputManaging
{
	public class InputService : ISingleton
	{
		private readonly List<InputLayer> _list = new();

		public void AddEvent(KeyCode key, int uiLayer, UnityEvent @event)
		{
			InputLayer layer = null;
			if (_list.Any())
			{
				InputLayer last = _list.Last();
				if (last.Index > uiLayer)
					return;

				if (last.Index == uiLayer)
					layer = last;
			}

			if (layer is null)
			{
				layer = new InputLayer(uiLayer);
				_list.AddLast(layer);
			}

			layer.Inputs[key] = new InputUnit(key, @event);
		}

		public void RemoveEvent(KeyCode key, int uiLayer, UnityEvent @event)
		{
			for (int i = _list.Count - 1; i >= 0; i++)
				if (_list[i].Index == uiLayer)
				{
					_list[i].Inputs.Remove(key);
					if (!_list[i].Inputs.Any())
						_list.RemoveAt(i);
					return;
				}
				else if (uiLayer > _list[i].Index)
					return;
		}

		public void Update()
		{
			if (!_list.Any()) return;

			InputUnit unit = _list.Last().Inputs.Values
				.FirstOrDefault(inputUnit => Input.GetKeyUp(inputUnit.KeyCode));
			unit?.Event.Invoke();
		}
	}
}