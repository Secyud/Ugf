using System;
using UnityEngine;
using UnityEngine.Events;

namespace Secyud.Ugf.Unity.EditorComponents
{
    /// <summary>
    /// When can't foresee the type of ui component
    /// but know the params need. Use this component
    /// to run a callback to sync data and ui.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class EditorCallback<TValue>
    {
        [SerializeField] private UnityEvent<TValue> _event = new();

        public void Invoke(TValue value)
        {
            _event.Invoke(value);
        }
    }
}