using System.Collections.Generic;
using UnityEngine;

namespace Secyud.Ugf.Unity.InputManagement
{
    public class InputComponent : MonoBehaviour
    {
        public InputService Service { get; set; }


        private void Update()
        {
            List<IInputEvent> inputEvents = Service.ValidEvents;

            FunctionKey function = InputService.GetFunctionKey();
            foreach (IInputEvent inputEvent in inputEvents)
            {
                if (Input.GetKeyUp(inputEvent.KeyCode) &&
                    function.HasFlag(inputEvent.FunctionKey))
                {
                    inputEvent.Invoke();
                }
            }
        }
    }
}