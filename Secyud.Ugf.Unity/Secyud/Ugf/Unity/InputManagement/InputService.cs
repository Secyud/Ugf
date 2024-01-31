using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;
using UnityEngine;

namespace Secyud.Ugf.Unity.InputManagement
{
    public class InputService : IRegistry
    {
        internal readonly List<InputComponent> List = new();

        public InputService()
        {
            UgfGameManager.Instance
                .GetOrAddComponent<InputManager>();
        }

        public void AddInput(InputComponent component)
        {
            List.Add(component);
        }

        public void RemoveInput(InputComponent component)
        {
            List.Remove(component);
        }
    }
}