using System;
using UnityEngine;
using UnityEngine.Events;

namespace Secyud.Ugf.EditorComponents
{
    [Serializable]
    public class EditorEvent<TValue>
    {
        [SerializeField] private UnityEvent<TValue> Event = new();

        public void Invoke(TValue value)
        {
            Event.Invoke(value);
        }
    }
}