using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Unity.AssetBundles;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.Unity.Prefabs
{
    [ExposeType(typeof(IPrefabRegistrar), typeof(IPrefabProvider))]
    public class PrefabManager : IPrefabProvider, IPrefabRegistrar, ISingleton
    {
        private readonly GameObject _canvas = GameObject.Find("Canvas");
        
        private readonly IDependencyProvider _dependencyProvider;
        private readonly IAssetBundleProvider _assetBundleProvider;
            
        private readonly Dictionary<string, PrefabDescriptor> _uis = new();
        

        public PrefabManager(
            IDependencyProvider dependencyProvider,
            IAssetBundleProvider assetBundleProvider)
        {
            _dependencyProvider = dependencyProvider;
            _assetBundleProvider = assetBundleProvider;
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

        public void RegisterPrefabsInAssembly(Assembly prefabAssembly, bool isUi = true, string assetBundlePath = null)
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

            RegisterPrefabs(types, isUi, assetBundlePath);
        }

        public void RegisterPrefabs(IEnumerable<Type> prefabTypes, bool isUi = true, string assetBundlePath = null)
        {
            foreach (var ui in prefabTypes)
                RegisterPrefab(ui, isUi, assetBundlePath);
        }

        public void RegisterPrefab(Type prefabType, bool isUi = true, string assetBundlePath = null)
        {
            var descriptor = new PrefabDescriptor(prefabType, isUi, assetBundlePath);

            _uis[descriptor.Name] = descriptor;
        }

        private PrefabBase CreatePrefab(Type prefabType, Func<GameObject, PrefabBase> prefabGetter,
            GameObject parent = null)
        {
            PrefabDescriptor descriptor;

            if (_uis.ContainsKey(prefabType.Name))
                descriptor = _uis[prefabType.Name];
            else
            {
                descriptor = new PrefabDescriptor(prefabType);
                _uis[descriptor.Name] = descriptor;
            }

            if (parent is null && descriptor.IsUi)
                parent = _canvas;

            AssetBundle assetBundle = null;

            if (descriptor.UsingAssetBundle)
                assetBundle = _assetBundleProvider.GetByPath(descriptor.Path);
            
            var gameObj = descriptor.Create(parent,assetBundle);

            var prefab = prefabGetter(gameObj);

            prefab.DependencyProvider = _dependencyProvider;

            prefab.OnInitialize();

            return prefab;
        }
    }
}