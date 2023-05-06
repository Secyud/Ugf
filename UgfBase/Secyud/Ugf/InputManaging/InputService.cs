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
		private readonly Dictionary<string, InputUnit> _dictionary = new();
		private readonly List<InputUnit> _list = new();
		private int _currentLayer;

		public void RegisterKey(string name, KeyCode key)
		{
			var unit = new InputUnit(name, key);
			_dictionary[name] = unit;
			_list.AddLast(unit);
		}

		public void AddEvent(string key, int uiLayer, UnityEvent @event)
		{
			if (_dictionary.TryGetValue(key, out InputUnit unit))
			{
				if (!unit.Events.IsNullOrEmpty())
				{
					Pair<int, UnityEvent> last = unit.Events.Last();

					if (last.First >= uiLayer)
						return;
				}

				unit.Events.AddLast(
					new Pair<int, UnityEvent>
					{
						First = uiLayer,
						Second = @event
					}
				);
				if (uiLayer > _currentLayer)
					_currentLayer = uiLayer;
			}
		}

		public void RemoveEvent(string key, int uiLayer, UnityEvent @event)
		{
			if (_dictionary.TryGetValue(key, out InputUnit unit))
				for (int i = unit.Events.Count - 1; i >= 0; i++)
					if (unit.Events[i].Second == @event)
					{
						unit.Events.RemoveAt(i);
						if (uiLayer >= _currentLayer)
							RefreshLayer();
						return;
					}
					else if (uiLayer > unit.Events[i].First)
					{
						return;
					}
		}

		private void RefreshLayer()
		{
			int max = 0;
			foreach (var unit in _dictionary.Values)
			{
				if (unit.Events.IsNullOrEmpty())
					continue;

				Pair<int, UnityEvent> e = unit.Events.Last();

				if (_currentLayer == e.First)
					return;

				if (max < e.First)
					max = e.First;
			}
			_currentLayer = max;
		}

		public void Update()
		{
			foreach (Pair<int, UnityEvent> last in
				from unit in _list 
				where !unit.Events.IsNullOrEmpty() 
				let last = unit.Events.Last() 
				where last.First == _currentLayer && Input.GetKeyUp(unit.KeyCode) 
				select last)
			{
				last.Second.Invoke();
				return;
			}
		}
	}
}