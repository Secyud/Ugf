using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Secyud.Ugf.Prefab.Extension;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.Prefab
{
    internal class PrefabDescriptor
    {
        private readonly Func<PrefabDescriptor, GameObject, GameObject> _instanceFactory;

        public readonly List<PropertyInfo> Dependencies;

        public readonly bool IsUi;

        public string Name { get; }

        public string Path { get; }

        public GameObject Instance { get; private set; }

        public PrefabDescriptor(Type prefabType,
            Func<PrefabDescriptor, GameObject, GameObject> instanceFactory, 
            bool isUi)
        {
            Path = prefabType.FullName!.Replace('.', '/');
            Name = prefabType.Name;
            _instanceFactory = instanceFactory;
            IsUi = isUi;
            Dependencies = prefabType.GetProperties(
                    BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance)
                .Where(u => Attribute.IsDefined(u, typeof(DependencyAttribute)))
                .ToList();
        }

        public void CreateSingleton(GameObject parent)
        {
            Instance ??= _instanceFactory(this, parent);
        }

        public void Destroy()
        {
            if (Instance is null)
                return;
            Object.Destroy(Instance);
            Instance = null;
        }
    }
}