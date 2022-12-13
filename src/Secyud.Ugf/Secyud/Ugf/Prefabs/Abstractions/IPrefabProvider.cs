using System;
using UnityEngine;

namespace Secyud.Ugf.Prefabs
{
    public interface IPrefabProvider
    {
        TPrefab CreatePrefab<TPrefab>(GameObject parent = null) where TPrefab : PrefabBase;
        PrefabBase CreatePrefab(Type prefabType, GameObject parent = null);
    }
}