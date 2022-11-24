using System;
using UnityEngine;

namespace Secyud.Ugf.Prefab
{
    public interface IPrefabControllerManager
    {
        TController GetOrAdd<TController>(GameObject parent = null) where TController : PrefabControllerBase;
        PrefabControllerBase GetOrAdd(Type controllerType,GameObject parent = null);

        TController Remove<TController>() where TController : PrefabControllerBase;
        PrefabControllerBase Remove(Type controllerType);
    }
}