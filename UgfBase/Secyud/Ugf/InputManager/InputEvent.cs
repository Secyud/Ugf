using System;
using UnityEngine;
using UnityEngine.Events;

namespace Secyud.Ugf.InputManager
{
    [Serializable]
    public class InputEvent
    {
        public KeyCode KeyCode;

        public UnityEvent Action;
    }
}