using System;
using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Prefab.Extension;
using UnityEngine;

namespace Secyud.Ugf.Prefab
{
    public class PrefabManager : IPrefabManager, ISingleton
    {
        private readonly IDependencyProvider _dependencyProvider;
        private readonly Dictionary<Type, PrefabBase> _prefabs = new();
        private readonly PrefabRegister _prefabRegister;

        public PrefabManager(PrefabRegister prefabRegister, IDependencyProvider dependencyProvider)
        {
            _prefabRegister = prefabRegister;
            _dependencyProvider = dependencyProvider;
        }

        public TPrefab GetOrAdd<TPrefab>(GameObject parent = null)
            where TPrefab : PrefabBase
        {
            if (_prefabs.ContainsKey(typeof(TPrefab)))
                return _prefabs[typeof(TPrefab)] as TPrefab;
            
            var descriptor = _prefabRegister.GetDescriptor(typeof(TPrefab));

            descriptor.CreateSingleton(parent);

            var prefab = descriptor.Instance.GetComponent<TPrefab>();

            InitPrefab(descriptor, prefab);

            _prefabs.Add(typeof(TPrefab), prefab);
            
            return prefab;
        }

        public PrefabBase GetOrAdd(Type prefabType,GameObject parent = null)
        {
            if (_prefabs.ContainsKey(prefabType))
                return _prefabs[prefabType];
            
            var descriptor = _prefabRegister.GetDescriptor(prefabType);

            descriptor.CreateSingleton(parent);

            var prefab = descriptor.Instance.GetComponent(prefabType) as PrefabBase;

            InitPrefab(descriptor, prefab);

            _prefabs.Add(prefabType, prefab);
            
            return prefab;
        }

        private void InitPrefab(PrefabDescriptor descriptor, PrefabBase prefab)
        {
            foreach (var dependency in descriptor.Dependencies)
                dependency.SetValue(prefab,_dependencyProvider.GetDependency(dependency.PropertyType));

            prefab!.PrefabManager = this;
            prefab!.PrefabDescriptor = descriptor;
            
            prefab.OnInitialize();
        }

        public void Remove<TController>()
            where TController : PrefabBase
        {
            Remove(typeof(TController));
        }

        public void Remove(Type prefabType)
        {
            if (!_prefabs.ContainsKey(prefabType))
                return;

            var prefab = _prefabs[prefabType];
            _prefabs.Remove(prefabType);
            prefab.PrefabDescriptor.Destroy();
            prefab.PrefabDescriptor = null;
        }
    }
}