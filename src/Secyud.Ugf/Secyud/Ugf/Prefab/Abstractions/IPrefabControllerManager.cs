using System;
using Secyud.Ugf.UserInterface;

namespace Secyud.Ugf.Prefab
{
    public interface IPrefabControllerManager
    {
        TController Add<TController>() where TController : PrefabControllerBase;
        PrefabControllerBase Add(Type controllerType);

        TController Remove<TController>() where TController : PrefabControllerBase;
        PrefabControllerBase Remove(Type controllerType);
    }
}