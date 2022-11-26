using System;
using UnityEngine;

namespace Secyud.Ugf.Prefab.Extension
{
    public abstract class PrefabBase : MonoBehaviour ,IDisposable
    {
        internal PrefabDescriptor PrefabDescriptor;

        public IPrefabManager PrefabManager { get; internal set; }

        public virtual void OnInitialize()
        {
        }

        public virtual void Dispose()
        {
            PrefabManager.Remove(GetType());
        }
    }
}