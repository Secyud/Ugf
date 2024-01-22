#region

using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;
using UnityEngine;

#endregion

namespace Secyud.Ugf.Unity.InputtingComponents
{
    public class InputService : IRegistry
    {
        internal readonly List<InputComponent> List = new();

        public InputService()
        {
            UgfGameManager.Instance.gameObject.GetOrAddComponent<InputManager>();
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