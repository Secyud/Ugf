using System;
using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.UserInterface;

namespace Secyud.Ugf.Prefab
{
    public class PrefabControllerManager:IPrefabControllerManager,ISingleton
    {
        private readonly Dictionary<Type,PrefabControllerBase> _panels = new(); 
        private readonly PrefabManager _prefabManager;
        private readonly IDependencyProvider _dependencyProvider;

        public PrefabControllerManager(PrefabManager prefabManager,IDependencyProvider dependencyProvider)
        {
            _prefabManager = prefabManager;
            _dependencyProvider = dependencyProvider;
        }

        public TController Add<TController>()
            where TController : PrefabControllerBase
        {
            var uiController = _dependencyProvider.GetDependency<TController>();
            
            _panels.Add(typeof(TController),uiController);

            uiController.ParentFactory ??=  GetParentController;
            
            var descriptor = _prefabManager.GetDescriptor(uiController.Name);
            
            descriptor.CreateSingleton(uiController.Parent?.PrefabDescriptor?.Instance);
            
            uiController.PrefabDescriptor = descriptor;
            uiController.OnInitialize();

            return uiController;
        }

        public PrefabControllerBase Remove<TController>()
            where TController: PrefabControllerBase
        {
            if (!_panels.ContainsKey(typeof(TController)))
                return null;
            
            var prefabController = _panels[typeof(TController)]; 
            prefabController.OnShutDown();
            prefabController.PrefabDescriptor.Destroy();
            prefabController.PrefabDescriptor = null;
            return prefabController;
        }


        private PrefabControllerBase GetParentController(PrefabControllerBase child)
        {
            return child.ParentType is null ? null : _panels[child.ParentType];
        }
    }
}