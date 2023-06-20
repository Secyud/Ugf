#region

using Secyud.Ugf.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Ugf.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Secyud.Ugf.InputManaging
{
    [Registry]
    public class InputService
    {
        private readonly List<InputLayer> _list = new();

        public void AddEvent(KeyCode key, RectTransform uiLayer, UnityEvent @event)
        {
            InputLayer layer = _list.FirstOrDefault(u => u.Layer == uiLayer);

            if (layer is null)
            {
                layer = new InputLayer(uiLayer);
                _list.AddLast(layer);
            }

            layer.Inputs[key] = new InputUnit(key, @event);
        }

        public void RemoveEvent(KeyCode key, RectTransform uiLayer)
        {
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                InputLayer input = _list[i];
                if (input.Layer == uiLayer)
                {
                    input.Inputs.Remove(key);
                    if (!input.Inputs.Any())
                        _list.RemoveAt(i);
                    return;
                }
            }

            Debug.LogWarning(($"Cannot find layer {uiLayer} for key {key}. Maybe conflicted key!"));
            ;
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