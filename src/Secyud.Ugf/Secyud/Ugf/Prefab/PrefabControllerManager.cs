using System;
using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;
using UnityEngine;

namespace Secyud.Ugf.Prefab
{
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

        public TController GetOrAdd<TController>(GameObject parent = null)
            where TController : PrefabControllerBase
        {
            return GetOrAdd(typeof(TController),parent) as TController;
        }

        public PrefabControllerBase GetOrAdd(Type controllerType,GameObject parent = null)
        {
            if (_panels.ContainsKey(controllerType))
                return _panels[controllerType];

            var uiController = _dependencyProvider.GetDependency(controllerType)
                as PrefabControllerBase;
            
            var descriptor = _prefabManager.GetDescriptor(uiController!.Name);

            descriptor.CreateSingleton(parent);
            
            uiController.PrefabDescriptor = descriptor;
            
            uiController.OnInitialize();

            _panels.Add(controllerType, uiController);
            
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

            _panels.Remove(controllerType);
            
            return prefabController;
        }
    }
}