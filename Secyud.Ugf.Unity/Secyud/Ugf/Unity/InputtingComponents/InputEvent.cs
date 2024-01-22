using System;
using UnityEngine;
using UnityEngine.Events;

namespace Secyud.Ugf.Unity.InputtingComponents
{
    [Serializable]
    public class InputEvent
    {
        public KeyCode KeyCode;

        public UnityEvent Action;
    }
}