#region

using Secyud.Ugf.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Secyud.Ugf.InputManaging
{
	public class InputService : ISingleton
	{
		private readonly List<InputLayer> _list = new();

		public void AddEvent(KeyCode key, RectTransform uiLayer, UnityEvent @event)
		{
			InputLayer layer = null;
			if (_list.Any())
			{
				InputLayer last = _list.Last();
				if (last.Layer == uiLayer)
					layer = last;
			}

			if (layer is null)
			{
				layer = new InputLayer(uiLayer);
				_list.AddLast(layer);
			}

			layer.Inputs[key] = new InputUnit(key, @event);
		}

		public void RemoveEvent(KeyCode key, RectTransform uiLayer, UnityEvent @event)
		{
			for (int i = _list.Count - 1; i >= 0; i--)
			{
				InputLayer input = _list[i];
				if (input.Layer == uiLayer)
				{
					input.Inputs.Remove(key);
					if (!input.Inputs.Any())
						_list.RemoveAt(_list.Count - 1);
					return;
				}
			}

			throw new UgfException($"Cannot find layer {uiLayer} for key {key}.");
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