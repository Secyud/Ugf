using System;

namespace Secyud.Ugf.Prefab;

public interface IPrefabControllerManager
{
    TController Add<TController>() where TController : PrefabControllerBase;
    PrefabControllerBase Add(Type controllerType);

    TController Remove<TController>() where TController : PrefabControllerBase;
    PrefabControllerBase Remove(Type controllerType);
}