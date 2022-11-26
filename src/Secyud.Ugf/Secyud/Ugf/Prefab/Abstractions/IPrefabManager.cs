using System;
using UnityEngine;

namespace Secyud.Ugf.Prefab
{
    public interface IPrefabManager
    {
        GameObject CreatePrefab<TController>(GameObject parent = null) where TController : PrefabBase;
        GameObject CreatePrefab(Type prefabType,GameObject parent = null);
    }
}