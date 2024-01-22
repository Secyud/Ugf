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
            }
            else
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
        }
    }
}