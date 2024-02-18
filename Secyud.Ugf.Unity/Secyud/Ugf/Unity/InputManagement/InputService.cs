using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;
using UnityEngine;

namespace Secyud.Ugf.Unity.InputManagement
{
    public class InputService : IRegistry
    {
        public Dictionary<int, IInputEvent> AllEvents { get; } = new();

        public List<IInputEvent> ValidEvents { get; } = new();

        public void AddEvent(int id)
        {
            ValidEvents.AddIfNotContains(AllEvents[id]);
        }

        public void RemoveEvent(int id)
        {
            ValidEvents.Remove(AllEvents[id]);
        }

        public InputService()
        {
            UgfGameManager.Instance
                .GetOrAddComponent<InputComponent>()
                .Service = this;
        }

        public static FunctionKey GetFunctionKey()
        {
            FunctionKey function = 0;
            if (Input.GetKey(KeyCode.LeftShift)) function |= FunctionKey.LeftShift;
            if (Input.GetKey(KeyCode.RightShift)) function |= FunctionKey.RightShift;
            if (Input.GetKey(KeyCode.LeftControl)) function |= FunctionKey.LeftControl;
            if (Input.GetKey(KeyCode.RightControl)) function |= FunctionKey.RightControl;
            if (Input.GetKey(KeyCode.LeftAlt)) function |= FunctionKey.LeftAlt;
            if (Input.GetKey(KeyCode.RightAlt)) function |= FunctionKey.RightAlt;
            return function;
        }
    }
}