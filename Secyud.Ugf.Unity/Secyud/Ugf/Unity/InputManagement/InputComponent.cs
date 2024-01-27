using System;
using UnityEngine;

namespace Secyud.Ugf.Unity.InputManagement
{
    public class InputComponent : MonoBehaviour
    {
        [SerializeField] private string _name;
        [field: SerializeField] public InputEvent[] Events { get; private set; }

        private void Awake()
        {
            if (_name.IsNullOrEmpty())
            {
                _name = name;
            }

            U.Get<InputService>().AddInput(this);
        }

        private void OnDestroy()
        {
            U.Get<InputService>().RemoveInput(this);
        }
    }
}