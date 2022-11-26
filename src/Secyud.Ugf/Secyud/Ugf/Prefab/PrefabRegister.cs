using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Prefab
{
    public class PrefabRegister : IPrefabRegister, ISingleton
    {
        private readonly Dictionary<string, PrefabDescriptor> _uis = new();

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

        internal PrefabDescriptor GetDescriptor(Type type)
        {
            return _uis[type.Name];
        }
    }
}