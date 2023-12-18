using System;
using UnityEngine;

namespace Secyud.Ugf.UgfHexMap
{
    public sealed class UgfUnitEffect : MonoBehaviour
    {
        public UgfUnitEffectDelegate Delegate { get; set; }


        private void Update()
        {
            Delegate?.OnUpdate();
        }

        private void OnDestroy()
        {
            Delegate?.OnDestroy();
        }
    }
}