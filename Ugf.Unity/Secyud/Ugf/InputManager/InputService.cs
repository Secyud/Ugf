#region

using System.Collections.Generic;
using System.Ugf.Collections.Generic;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.UpdateComponents;
using UnityEngine;

#endregion

namespace Secyud.Ugf.InputManager
{
    public class InputService :IRegistry
    {
        private readonly List<InputComponent> _list = new();
        private readonly List<InputEvent> _globalInput = new();

        public InputService(IUpdateService service)
        {
            service.UpdateAction += Update;
        }
        
        public void AddInput(InputComponent component)
        {
            _list.AddLast(component);
        }

        public void RemoveInput(InputComponent component)
        {
            _list.Remove(component);
        }

        public void AddInput(InputEvent component)
        {
            _globalInput.AddLast(component);
        }

        public void RemoveInput(InputEvent component)
        {
            _globalInput.Remove(component);
        }

        public void Update()
        {
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                InputComponent inputComponent = _list[i];

                if (inputComponent)
                {
                    foreach (InputEvent input in inputComponent.Actions)
                    {
                        if (Input.GetKeyUp(input.KeyCode))
                        {
                            input.Action.Invoke();
                            return;
                        }
                    }
                }
                else
                {
                    _list.RemoveAt(i);
                }
            }

            foreach (InputEvent input in _globalInput)
            {
                if (Input.GetKeyUp(input.KeyCode))
                {
                    input.Action.Invoke();
                    return;
                }
            }
        }
    }
}