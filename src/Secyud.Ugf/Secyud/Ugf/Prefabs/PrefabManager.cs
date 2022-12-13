using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Secyud.Ugf.DependencyInjection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.Prefabs
{
    [ExposeType(typeof(IPrefabRegister), typeof(IPrefabProvider))]
    public class PrefabManager : IPrefabProvider, IPrefabRegister, ISingleton
    {
        private readonly GameObject _canvas = GameObject.Find("Canvas");
        private readonly IDependencyProvider _dependencyProvider;
        private readonly Dictionary<string, PrefabDescriptor> _uis = new();

        public PrefabManager(IDependencyProvider dependencyProvider)
        {
            _dependencyProvider = dependencyProvider;
            Object.DontDestroyOnLoad(_canvas);
        }

        public TPrefab CreatePrefab<TPrefab>(GameObject parent = null)
            where TPrefab : PrefabBase
        {
            return CreatePrefab(typeof(TPrefab),
                o => o.GetComponent<TPrefab>(),
                parent) as TPrefab;
        }

        public PrefabBase CreatePrefab(Type prefabType, GameObject parent = null)
        {
            return CreatePrefab(prefabType,
                o => o.GetComponent(prefabType) as PrefabBase,
                parent);
        }

        public void RegisterPrefabsInAssembly(Assembly prefabAssembly, bool isUi = false)
        {
            var types = prefabAssembly.GetTypes()
                .Where(type =>
                    type is
                    {
                        IsClass: true,
                        IsAbstract: false,
                        IsGenericType: false
                    } &&
                    typeof(PrefabBase).IsAssignableFrom(type));

            RegisterPrefabs(
                types,
                isUi
            );
        }

        public void RegisterPrefabs(IEnumerable<Type> prefabTypes, bool isUi = false)
        {
            foreach (var ui in prefabTypes)
                RegisterPrefab(ui, isUi);
        }

        public void RegisterPrefab(Type prefabType, bool isUi = false)
        {
            var descriptor = new PrefabDescriptor(prefabType, isUi);
            _uis[descriptor.Name] = descriptor;
        }

        private PrefabBase CreatePrefab(Type prefabType, Func<GameObject, PrefabBase> prefabGetter,
            GameObject parent = null)
        {
            var descriptor = _uis[prefabType.Name];

            if (parent is null && descriptor.IsUi)
                parent = _canvas;

            var gameObj = descriptor.Create(parent);

            var prefab = prefabGetter(gameObj);

            prefab.DependencyProvider = _dependencyProvider;
            
            foreach (var dependency in descriptor.Dependencies)
                dependency.SetValue(prefab, _dependencyProvider.GetDependency(dependency.PropertyType));

            prefab.OnInitialize();

            return prefab;
        }
    }
}