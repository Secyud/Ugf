using System;
using Secyud.Ugf.Prefab.Extension;
using UnityEngine;

namespace Secyud.Ugf.Prefab
{
    public interface IPrefabManager
    {
        TController GetOrAdd<TController>(GameObject parent = null) where TController : PrefabBase;
        PrefabBase GetOrAdd(Type prefabType,GameObject parent = null);

        void Remove<TController>() where TController : PrefabBase;
        void Remove(Type prefabType);
    }
}