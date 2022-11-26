using System;
using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.Prefab
{
    public class PrefabManager : IPrefabManager, ISingleton
    {
        private readonly IDependencyProvider _dependencyProvider;
        private readonly PrefabRegister _prefabRegister;

        public PrefabManager(PrefabRegister prefabRegister, IDependencyProvider dependencyProvider)
        {
            _prefabRegister = prefabRegister;
            _dependencyProvider = dependencyProvider;
        }

        public GameObject CreatePrefab<TPrefab>(GameObject parent = null)
            where TPrefab : PrefabBase
        {
            return CreatePrefab(typeof(TPrefab),
                o => o.GetComponent<TPrefab>(),
                parent);
        }

        public GameObject CreatePrefab(Type prefabType, GameObject parent = null)
        {
            return CreatePrefab(prefabType,
                o => o.GetComponent(prefabType) as PrefabBase,
                parent);
        }

        private GameObject CreatePrefab(Type prefabType, Func<GameObject, PrefabBase> prefabGetter,
            GameObject parent = null)
        {
            var descriptor = _prefabRegister.GetDescriptor(prefabType);

            var gameObj = descriptor.Create(parent);

            var prefab = prefabGetter(gameObj);

            foreach (var dependency in descriptor.Dependencies)
                dependency.SetValue(prefab, _dependencyProvider.GetDependency(dependency.PropertyType));

            prefab.OnInitialize();

            return gameObj;
        }

    }
}