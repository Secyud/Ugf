using System;
using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Prefab;

public class PrefabControllerManager : IPrefabControllerManager, ISingleton
{
    private readonly IDependencyProvider _dependencyProvider;
    private readonly Dictionary<Type, PrefabControllerBase> _panels = new();
    private readonly PrefabManager _prefabManager;

    public PrefabControllerManager(PrefabManager prefabManager, IDependencyProvider dependencyProvider)
    {
        _prefabManager = prefabManager;
        _dependencyProvider = dependencyProvider;
    }

    public TController Add<TController>()
        where TController : PrefabControllerBase
    {
        var uiController = _dependencyProvider.GetDependency<TController>();

        Add(uiController);

        return uiController;
    }

    public PrefabControllerBase Add(Type controllerType)
    {
        var uiController = _dependencyProvider.GetDependency(controllerType)
            as PrefabControllerBase;

        Add(uiController);

        return uiController;
    }

    public TController Remove<TController>()
        where TController : PrefabControllerBase
    {
        return Remove(typeof(TController)) as TController;
    }

    public PrefabControllerBase Remove(Type controllerType)
    {
        if (!_panels.ContainsKey(controllerType))
            return null;

        var prefabController = _panels[controllerType];
        prefabController.OnShutDown();
        prefabController.PrefabDescriptor.Destroy();
        prefabController.PrefabDescriptor = null;
        return prefabController;
    }

    private void Add(PrefabControllerBase uiController)
    {
        _panels.Add(uiController.GetType(), uiController);

        uiController.ParentFactory ??= GetParentController;

        var descriptor = _prefabManager.GetDescriptor(uiController.Name);

        descriptor.CreateSingleton(uiController.Parent?.PrefabDescriptor?.Instance);

        uiController.PrefabDescriptor = descriptor;
        uiController.OnInitialize();
    }

    private PrefabControllerBase GetParentController(PrefabControllerBase child)
    {
        return child.ParentType is null ? null : _panels[child.ParentType];
    }
}