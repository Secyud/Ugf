using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Secyud.Ugf.DependencyInjection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.Prefab
{
    internal class PrefabDescriptor
    {
        public readonly List<PropertyInfo> Dependencies;

        public readonly bool IsUi;

        public string Name { get; }

        public string Path { get; }
        
        public PrefabDescriptor(Type prefabType, bool isUi)
        {
            Path = prefabType.FullName!.Replace('.', '/');
            Name = prefabType.Name;
            IsUi = isUi;
            Dependencies = prefabType.GetProperties(
                    BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance)
                .Where(u => Attribute.IsDefined(u, typeof(DependencyAttribute)))
                .ToList();
        }

        public GameObject Create(GameObject parent)
        {
            var prefab = parent is null
                ? Object.Instantiate(
                    Resources.Load<GameObject>(Path))
                : Object.Instantiate(
                    Resources.Load<GameObject>(Path),
                    parent.transform);

            prefab.name = Name;

            return prefab;
        }
    }
}