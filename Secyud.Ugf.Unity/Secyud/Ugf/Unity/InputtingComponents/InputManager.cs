using System.Linq;
using Secyud.Ugf.Logging;
using UnityEngine;

namespace Secyud.Ugf.Unity.InputtingComponents
{
    public class InputManager : MonoBehaviour
    {
        public InputService Service;

        private void Awake()
        {
            Service = U.Get<InputService>();
        }

        private void Update()
        {
            InputComponent inputComponent = Service.List.Last();

            if (!inputComponent)
            {
                UgfLogger.LogError("Last input component is not available!");
                return;
            }

            int length = inputComponent.Events.Length;
            for (int i = 0; i < length; i++)
            {
                InputEvent input = inputComponent.Events[i];
                if (Input.GetKeyUp(input.KeyCode))
                {
                    input.Action.Invoke();
                    return;
                }
            }
        }
    }
}